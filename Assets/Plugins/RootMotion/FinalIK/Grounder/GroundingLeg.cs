using UnityEngine;
using System.Collections;

namespace RootMotion.FinalIK {

	public partial class Grounding {

		/// <summary>
		/// The %Grounding %Leg.
		/// </summary>
		public class Leg {
            
            /// <summary>
			/// Returns the layer of the ground
			/// </summary>
			public int groundLayer { get; private set; }
            /// <summary>
            /// Returns the name of the ground object
            /// </summary>
            public string groundObject { get; private set; }
            /// <summary>
            /// Returns true distance from foot to ground is less that maxStep
            /// </summary>
            public bool isGrounded { get; private set; }
			/// <summary>
			/// Gets the current IK position of the foot.
			/// </summary>
			public Vector3 IKPosition { get; private set; }
			/// <summary>
			/// Gets the current rotation offset of the foot.
			/// </summary>
            public Transform IKRaycast { get; private set; }
			public Quaternion rotationOffset { get; private set; }
			/// <summary>
			/// Returns true, if the leg is valid and initiated
			/// </summary>
			public bool initiated { get; private set; }
			/// <summary>
			/// The height of foot from ground.
			/// </summary>
			public float heightFromGround { get; private set; }
			/// <summary>
			/// Velocity of the foot
			/// </summary>
			public Vector3 velocity { get; private set; }
			/// <summary>
			/// Gets the foot Transform.
			/// </summary>
			public Transform transform { get; private set; }
			/// <summary>
			/// Gets the current IK offset.
			/// </summary>
			public float IKOffset { get; private set; }
            public Vector3 IKOffsetPosition { get; private set; }
            public bool UseYOnly { get; private set; }

            private Grounding grounding;
            
            private float switchlerp = 0;
			private Vector3 lastPosition;
			private Quaternion toHitNormal, r;
			private RaycastHit heelHit;
			private Vector3 up = Vector3.up;
			
			// Initiates the Leg
			public void Initiate(Grounding grounding, Transform transform) {
				initiated = false;
				this.grounding = grounding;
				this.transform = transform;
				up = Vector3.up;
				IKPosition = transform.position;
                IKRaycast = new GameObject("IKRaycast").transform;
				
				initiated = true;
				OnEnable();
			}

			// Should be called each time the leg is (re)activated
			public void OnEnable() {
				if (!initiated) return;
				
				lastPosition = transform.position;
			}

			// Set everything to 0
			public void Reset() {
				lastPosition = transform.position;
				IKOffset = 0f;
                IKOffsetPosition = Vector3.zero;
				IKPosition = transform.position;
				rotationOffset = Quaternion.identity;
			}

			// Raycasting, processing the leg's position
			public void Process() {
				if (!initiated) return;
				if (grounding.maxStep <= 0) return;

				if (Time.deltaTime == 0f) return;

                UseYOnly = true;
				up = grounding.up;
				heightFromGround = Mathf.Infinity;
				
				// Calculating velocity
				velocity = (transform.position - lastPosition) / Time.deltaTime;
				velocity = grounding.Flatten(velocity);
				lastPosition = transform.position;

				Vector3 prediction = velocity * grounding.prediction;
				
				if (grounding.footRadius <= 0) grounding.quality = Grounding.Quality.Fastest;
				
				// Raycasting
				switch(grounding.quality) {

				    // The fastest, single raycast
				    case Grounding.Quality.Fastest:

					    RaycastHit predictedHit = GetRaycastHit(prediction);
					    SetFootToPoint(predictedHit.normal, predictedHit.point);
                        groundLayer = predictedHit.collider.gameObject.layer;
                        groundObject = predictedHit.collider.gameObject.name;
                        break;

				    // Medium, 3 raycasts
				    case Grounding.Quality.Simple:

					    heelHit = GetRaycastHit(Vector3.zero);
					    RaycastHit toeHit = GetRaycastHit(grounding.root.forward * grounding.footRadius + prediction);
					    RaycastHit sideHit = GetRaycastHit(grounding.root.right * grounding.footRadius * 0.5f);
					
					    Vector3 planeNormal = Vector3.Cross(toeHit.point - heelHit.point, sideHit.point - heelHit.point).normalized;
					    if (Vector3.Dot(planeNormal, up) < 0) planeNormal = -planeNormal;
					
					    SetFootToPlane(planeNormal, heelHit.point, heelHit.point);
                        groundLayer = heelHit.collider.gameObject.layer;
                        groundObject = heelHit.collider.gameObject.name;
                        break;
				
				    // The slowest, raycast and a capsule cast
				    case Grounding.Quality.Best:

                        StepTry(Vector3.zero, prediction);
                        if(groundObject == "")
                        {
                            if (grounding.PrioritiseForward)
                            {
                                if(!HoriztonalGapScan(grounding.root.forward, prediction, Color.blue))
                                {
                                    HoriztonalGapScan(-grounding.root.forward, prediction, Color.red);
                                }
                            }
                            else
                            {
                                if (!HoriztonalGapScan(-grounding.root.forward, prediction, Color.red))
                                {
                                    HoriztonalGapScan(grounding.root.forward, prediction, Color.blue);
                                }
                            }
                        }
                        SetFootToPoint(heelHit.normal, heelHit.point);
                        break;
				}

				// Is the foot grounded?
				isGrounded = heightFromGround < grounding.GroundTheshold /*grounding.maxStep*/;
                if (!isGrounded)
                {
                    groundLayer = -1;
                }

                float offsetTarget = stepHeightFromGround;
                Vector3 offsetPosition = IKRaycast.position;
				//if (!grounding.rootGrounded) offsetTarget = 0f;

                //IKOffset = Interp.LerpValue(IKOffset, offsetTarget, grounding.footSpeed, grounding.footSpeed);
                IKOffset = Mathf.Lerp(IKOffset, offsetTarget, Time.deltaTime * grounding.footSpeed);
                IKOffsetPosition = Vector3.Lerp(IKOffsetPosition, offsetPosition, Time.deltaTime * grounding.footSpeed);

                float legHeight = grounding.GetVerticalOffset(transform.position, grounding.root.position);
				float currentMaxOffset = Mathf.Clamp(grounding.maxStep - legHeight, 0f, grounding.maxStep);

				IKOffset = Mathf.Clamp(IKOffset, -currentMaxOffset, IKOffset);
                IKOffsetPosition = new Vector3(IKOffsetPosition.x, Mathf.Clamp(IKOffsetPosition.y, -currentMaxOffset, IKOffsetPosition.y), IKOffsetPosition.z);

				RotateFoot();
                if (UseYOnly)
                {
                    if(switchlerp > 0)
                    {
                        switchlerp -= Time.deltaTime * grounding.GapSnapSpeed;
                    }
                }
                else
                {
                    if(switchlerp < 1)
                    {
                        switchlerp += Time.deltaTime * grounding.GapSnapSpeed;
                    }
                }

                switchlerp = Mathf.Clamp01(switchlerp);
				// Update IK values
				IKPosition = Vector3.Lerp(transform.position - up * IKOffset, IKOffsetPosition, switchlerp);
                

				float rW = grounding.footRotationWeight;
				rotationOffset = rW >= 1? r: Quaternion.Slerp(Quaternion.identity, r, rW);
			}

            void StepTry(Vector3 Offset, Vector3 prediction)
            {
                heelHit = GetRaycastHit(Offset);
                RaycastHit capsuleHit = GetCapsuleHit(prediction);

                
                if (heelHit.collider != null)
                {
                    groundLayer = heelHit.collider.gameObject.layer;
                    groundObject = heelHit.collider.gameObject.name;
                }
                else
                {
                    groundObject = "";
                }
            }

			// Gets the height from ground clamped between min and max step height
			public float stepHeightFromGround {
				get {
					return Mathf.Clamp(heightFromGround, -grounding.maxStep, grounding.maxStep);
				}
			}

			// Get predicted Capsule hit from the middle of the foot
			private RaycastHit GetCapsuleHit(Vector3 offsetFromHeel) {
				RaycastHit hit = new RaycastHit();
				Vector3 origin = transform.position + grounding.root.forward * grounding.footRadius;
				hit.point = origin - up * grounding.maxStep * 2f;
				hit.normal = up;
				
				// Start point of the capsule
				Vector3 capsuleStart = origin + grounding.maxStep * up;
				// End point of the capsule depending on the foot's velocity.
				Vector3 capsuleEnd = capsuleStart + offsetFromHeel;

				Physics.CapsuleCast(capsuleStart, capsuleEnd, grounding.footRadius, -up, out hit, grounding.maxStep * 3, grounding.layers);
				return hit;
			}
			
			// Get simple Raycast from the heel
			private RaycastHit GetRaycastHit(Vector3 offsetFromHeel) {
				RaycastHit hit = new RaycastHit();
				Vector3 origin = transform.position + offsetFromHeel;
				hit.point = origin - up * grounding.maxStep * 2f;
				hit.normal = up;
				if (grounding.maxStep <= 0f) return hit;

				Physics.Raycast(origin + grounding.maxStep * up, -up, out hit, grounding.maxStep * 3, grounding.layers);
				return hit;
			}
			
			// Rotates ground normal with respect to maxFootRotationAngle
			private Vector3 RotateNormal(Vector3 normal) {
				if (grounding.quality == Grounding.Quality.Best) return normal;
				return Vector3.RotateTowards(up, normal, grounding.maxFootRotationAngle * Mathf.Deg2Rad, Time.deltaTime);
			}
			
			// Set foot height from ground relative to a point
			private void SetFootToPoint(Vector3 normal, Vector3 point) {
				toHitNormal = Quaternion.FromToRotation(up, RotateNormal(normal));

                IKRaycast.position = point;
                heightFromGround = GetHeightFromGround(point);
			}
			
			// Set foot height from ground relative to a plane
			private void SetFootToPlane(Vector3 planeNormal, Vector3 planePoint, Vector3 heelHitPoint) {
				planeNormal = RotateNormal(planeNormal);
				toHitNormal = Quaternion.FromToRotation(up, planeNormal);

                

                Vector3 pointOnPlane = V3Tools.LineToPlane(transform.position + up * grounding.maxStep, -up, planeNormal, planePoint);

                

                // Get the height offset of the point on the plane
                heightFromGround = GetHeightFromGround(pointOnPlane);
				
				// Making sure the heel doesn't penetrate the ground
				float heelHeight = GetHeightFromGround(heelHitPoint);
				heightFromGround = Mathf.Clamp(heightFromGround, -Mathf.Infinity, heelHeight);
			}

			// Calculate height offset of a point
			private float GetHeightFromGround(Vector3 hitPoint) {
				return grounding.GetVerticalOffset(transform.position, hitPoint) - rootYOffset;
			}
			
			// Adding ground normal offset to the foot's rotation
			private void RotateFoot() {
				// Getting the full target rotation
				Quaternion rotationOffsetTarget = GetRotationOffsetTarget();
				
				// Slerping the rotation offset
				r = Quaternion.Slerp(r, rotationOffsetTarget, Time.deltaTime * grounding.footRotationSpeed);
			}
			
			// Gets the target hit normal offset as a Quaternion
			private Quaternion GetRotationOffsetTarget() {
				if (grounding.maxFootRotationAngle <= 0f) return Quaternion.identity;
				if (grounding.maxFootRotationAngle >= 180f) return toHitNormal;
				return Quaternion.RotateTowards(Quaternion.identity, toHitNormal, grounding.maxFootRotationAngle);
			}
			
			// The foot's height from ground in the animation
			private float rootYOffset {
				get {
					return grounding.GetVerticalOffset(transform.position, grounding.root.position - up * grounding.heightOffset);
				}
			}
            
            bool HoriztonalGapScan(Vector3 direction, Vector3 prediction, Color DebugColor)
            {
                RaycastHit hit = new RaycastHit();
                Quaternion orientation = Quaternion.LookRotation(grounding.root.forward, grounding.root.up);
                Vector3 boxCastSize = new Vector3(0.001f, grounding.maxStep * 0.5f, 0.001f);
                Vector3 BoxCastCenter = transform.position - up * grounding.maxStep * 0.5f;
                Physics.BoxCast(BoxCastCenter, boxCastSize, direction, out hit, orientation, grounding.maxGapDistance, grounding.layers);
                if (hit.collider != null)
                {
                    float dist = Vector3.Distance(transform.position - up * grounding.maxStep * 0.5f, hit.point);
                    StepTry(direction * (dist + 0.01f), prediction);
                    if (groundObject != "")
                    {
                        if (isGrounded)
                        {
                            UseYOnly = false;
                            return true;
                        }
                    }
                }
                return false;
            }
		}
	}
}
