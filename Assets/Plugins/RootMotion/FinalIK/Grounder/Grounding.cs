using UnityEngine;
using System.Collections;

namespace RootMotion.FinalIK {

	/// <summary>
	/// Foot placement system.
	/// </summary>
	[System.Serializable]
	public partial class Grounding {
		
		#region Main Interface

		/// <summary>
		/// The raycasting quality. Fastest is a single raycast per foot, Simple is three raycasts, Best is one raycast and a capsule cast per foot.
		/// </summary>
		[System.Serializable]
		public enum Quality {
			Fastest,
			Simple,
			Best
		}

		/// <summary>
		/// Layers to ground the character to. Make sure to exclude the layer of the character controller.
		/// </summary>
		[Tooltip("Layers to ground the character to. Make sure to exclude the layer of the character controller.")]
		public LayerMask layers;
        /// <summary>
        /// Maximum distance from the ground before the ground is no longer detected.
        /// </summary>
        [Tooltip("Maximum distance from the ground before the ground is no longer detected.")]
        public float GroundTheshold = 0.5f;
        /// <summary>
        /// Max step height. Maximum vertical distance of Grounding from the root of the character.
        /// </summary>
        [Tooltip("Max step height. Maximum vertical distance of Grounding from the root of the character.")]
		public float maxStep = 0.5f;
		/// <summary>
		/// The height offset of the root.
		/// </summary>
		[Tooltip("The height offset of the root.")]
		public float heightOffset;
		/// <summary>
		/// The speed of moving the feet up/down.
		/// </summary>
		[Tooltip("The speed of moving the feet up/down.")]
		public float footSpeed = 2.5f;
		/// <summary>
		/// CapsuleCast radius. Should match approximately with the size of the feet.
		/// </summary>
		[Tooltip("CapsuleCast radius. Should match approximately with the size of the feet.")]
		public float footRadius = 0.15f;
		/// <summary>
		/// Amount of velocity based prediction of the foot positions.
		/// </summary>
		[Tooltip("Amount of velocity based prediction of the foot positions.")]
		public float prediction = 0.05f;
        /// <summary>
        /// The maximum distance a platform can be from the foot before the IK no longer finds it
        /// </summary>
        [Tooltip("The maximum distance a platform can be from the foot before the IK no longer finds it.")]
        public float maxGapDistance = 0.1f;
        /// <summary>
        /// Speed which the IK snaps to edges over gaps
        /// </summary>
        [Tooltip("Speed which the IK snaps to edges over gaps.")]
        public float GapSnapSpeed = 2;
        /// <summary>
        /// When over a gap, prioritise front raycast over back raycast.
        /// </summary>
        [Tooltip("When over a gap, prioritise front raycast over back raycast.")]
        public bool PrioritiseForward;
        public float maxGapPelvisMaintain;
        public float maxGapPelvisDecrease;
        public float maxgapPelvisDecreaseDistance;
        public Vector3 pelvisRaycastSize;
        public float pelvisRaycastDistance;
        public AnimationCurve PelvisLowerAmount;
        public Vector3 pelvisRaycastOffset;
        public float pevlisLowerOffset;
        /// <summary>
        /// Weight of rotating the feet to the ground normal offset.
        /// </summary>
        [Tooltip("Weight of rotating the feet to the ground normal offset.")]
		[Range(0f, 1f)]
		public float footRotationWeight = 1f;
		/// <summary>
		/// Speed of slerping the feet to their grounded rotations.
		/// </summary>
		[Tooltip("Speed of slerping the feet to their grounded rotations.")]
		public float footRotationSpeed = 7f;
		/// <summary>
		/// Max Foot Rotation Angle, Max angular offset from the foot's rotation (Reasonable range: 0-90 degrees).
		/// </summary>
		[Tooltip("Max Foot Rotation Angle. Max angular offset from the foot's rotation.")]
		[Range(0f, 90f)]
		public float maxFootRotationAngle = 45f;
		/// <summary>
		/// If true, solver will rotate with the character root so the character can be grounded for example to spherical planets. 
		/// For performance reasons leave this off unless needed.
		/// </summary>
		[Tooltip("If true, solver will rotate with the character root so the character can be grounded for example to spherical planets. For performance reasons leave this off unless needed.")]
		public bool rotateSolver;
		/// <summary>
		/// The speed of moving the character up/down.
		/// </summary>
		[Tooltip("The speed of moving the character up/down.")]
		public float pelvisSpeed = 5f;
		/// <summary>
		/// Used for smoothing out vertical pelvis movement (range 0 - 1).
		/// </summary>
		[Tooltip("Used for smoothing out vertical pelvis movement (range 0 - 1).")]
		[Range(0f, 1f)]
		public float pelvisDamper;
		/// <summary>
		/// The weight of lowering the pelvis to the lowest foot.
		/// </summary>
		[Tooltip("The weight of lowering the pelvis to the lowest foot.")]
		public float lowerPelvisWeight = 1f;
		/// <summary>
		/// The weight of lifting the pelvis to the highest foot. This is useful when you don't want the feet to go too high relative to the body when crouching.
		/// </summary>
		[Tooltip("The weight of lifting the pelvis to the highest foot. This is useful when you don't want the feet to go too high relative to the body when crouching.")]
		public float liftPelvisWeight;
		/// <summary>
		/// The radius of the spherecast from the root that determines whether the character root is grounded.
		/// </summary>
		[Tooltip("The radius of the spherecast from the root that determines whether the character root is grounded.")]
		public float rootSphereCastRadius = 0.1f;
		/// <summary>
		/// The raycasting quality. Fastest is a single raycast per foot, Simple is three raycasts, Best is one raycast and a capsule cast per foot.
		/// </summary>
		[Tooltip("The raycasting quality. Fastest is a single raycast per foot, Simple is three raycasts, Best is one raycast and a capsule cast per foot.")]
		public Quality quality = Quality.Best;

		/// <summary>
		/// The %Grounding legs.
		/// </summary>
		public Leg[] legs { get; private set; }
		/// <summary>
		/// The %Grounding pelvis.
		/// </summary>
		public Pelvis pelvis { get; private set; }
		/// <summary>
		/// Gets a value indicating whether any of the legs are grounded
		/// </summary>
		public bool isGrounded { get; private set; }
        /// <summary>
        /// Gets the name of the ground collider which is currently being used;
        /// </summary>
        public string currentGround { get; private set; }
        /// <summary>
        /// Gets the layer of the ground collider which is currently being used;
        /// </summary>
        public int currentGroundLayer { get; private set; }
        /// <summary>
        /// finds Location where IK will go
        /// </summary>
        public Transform[] LegRaycast { get; private set; }
        public float lowestOffset { get; private set; }
        public float highestOffset { get; private set; }
        /// <summary>
        /// The root Transform
        /// </summary>
        public Transform root { get; private set; }
		/// <summary>
		/// Ground height at the root position.
		/// </summary>
		public RaycastHit rootHit { get; private set; }
        public bool useYOnly { get; private set; }
        /// <summary>
        /// Is the RaycastHit from the root grounded?
        /// </summary>
		public bool rootGrounded {
			get {
                if (!useYOnly) return true;
				return rootHit.distance < maxStep * 2f;
			}
		}

		/// <summary>
		/// Raycasts or sphereCasts to find the root ground point. Distance of the Ray/Sphere cast is maxDistanceMlp x maxStep. Use this instead of rootHit if the Grounder is weighed out/disabled and not updated.
		/// </summary>
		public RaycastHit GetRootHit() {
			RaycastHit h = new RaycastHit();
			Vector3 _up = up;
			
			Vector3 legsCenter = Vector3.zero;
			foreach (Leg leg in legs) legsCenter += leg.transform.position;
			legsCenter /= legs.Length;
			
			h.point = legsCenter - _up * maxStep * 10f;
			float distMlp = 10 + 1;
			h.distance = maxStep * distMlp;
			
			if (maxStep <= 0f) return h;
			
			if (quality != Quality.Best) Physics.Raycast(legsCenter + _up * maxStep, -_up, out h, maxStep * distMlp, layers);
			else Physics.SphereCast(legsCenter + _up * maxStep, rootSphereCastRadius, -up, out h, maxStep * distMlp, layers);
			
			return h;
		}

		/// <summary>
		/// Gets a value indicating whether this <see cref="Grounding"/> is valid.
		/// </summary>
		public bool IsValid(ref string errorMessage) {
			if (root == null) {
				errorMessage = "Root transform is null. Can't initiate Grounding.";
				return false;
			}
			if (legs == null) {
				errorMessage = "Grounding legs is null. Can't initiate Grounding.";
				return false;
			}
			if (pelvis == null) {
				errorMessage = "Grounding pelvis is null. Can't initiate Grounding.";
				return false;
			}
			
			if (legs.Length == 0) {
				errorMessage = "Grounding has 0 legs. Can't initiate Grounding.";
				return false;
			}
			return true;
		}
		
		/// <summary>
		/// Initiate the %Grounding as an integrated solver by providing the root Transform, leg solvers, pelvis Transform and spine solver.
		/// </summary>
		public void Initiate(Transform root, Transform[] feet) {
			this.root = root;
			initiated = false;

			rootHit = new RaycastHit();

			// Constructing Legs
			if (legs == null) legs = new Leg[feet.Length];
            LegRaycast = new Transform[feet.Length];
			if (legs.Length != feet.Length) legs = new Leg[feet.Length];
			for (int i = 0; i < feet.Length; i++) if (legs[i] == null) legs[i] = new Leg();
			
			// Constructing pelvis
			if (pelvis == null) pelvis = new Pelvis();
			
			string errorMessage = string.Empty;
			if (!IsValid(ref errorMessage)) {
				Warning.Log(errorMessage, root, false);
				return;
			}
			
			// Initiate solvers only if application is playing
			if (Application.isPlaying) {
				for (int i = 0; i < feet.Length; i++) legs[i].Initiate(this, feet[i]);
				pelvis.Initiate(this);
				
				initiated = true;
			}
		}

		/// <summary>
		/// Updates the Grounding.
		/// </summary>
		public void Update() {
			if (!initiated) return;

			if (layers == 0) LogWarning("Grounding layers are set to nothing. Please add a ground layer.");

			maxStep = Mathf.Clamp(maxStep, 0f, maxStep);
			footRadius = Mathf.Clamp(footRadius, 0.0001f, maxStep);
			pelvisDamper = Mathf.Clamp(pelvisDamper, 0f, 1f);
			rootSphereCastRadius = Mathf.Clamp(rootSphereCastRadius, 0.0001f, rootSphereCastRadius);
			maxFootRotationAngle = Mathf.Clamp(maxFootRotationAngle, 0f, 90f);
			prediction = Mathf.Clamp(prediction, 0f, prediction);
			footSpeed = Mathf.Clamp(footSpeed, 0f, footSpeed);

			// Root hit
			rootHit = GetRootHit();

			lowestOffset = Mathf.NegativeInfinity;
			highestOffset = Mathf.Infinity;
			isGrounded = false;
            currentGroundLayer = -1;
            int raycastIndex = 0;
            useYOnly = true;

            // Process legs
            foreach (Leg leg in legs)
            {
                leg.Process();
                if (leg.UseYOnly) { 
                    if (leg.IKOffset > lowestOffset) lowestOffset = leg.IKOffset;
                    if (leg.IKOffset < highestOffset) highestOffset = leg.IKOffset;
                }
                else
                {
                    if (leg.IKPosition.y > lowestOffset) lowestOffset = leg.transform.position.y - leg.PositionPelivsOffset;
                    if (leg.IKPosition.y < highestOffset) highestOffset = leg.transform.position.y - leg.PositionPelivsOffset;
                    useYOnly = false;
                }
                LegRaycast[raycastIndex] = leg.IKRaycast;
                raycastIndex++;

                if (leg.groundLayer != -1)
                {
                    currentGroundLayer = leg.groundLayer;
                }
                if (leg.isGrounded)
                {
                    isGrounded = true;
                }
                if (leg.groundObject != "") currentGround = leg.groundObject;
            }

            // Precess pelvis
            pelvis.Process(-lowestOffset * lowerPelvisWeight, -highestOffset * liftPelvisWeight, isGrounded);
		}

		// Calculate the normal of the plane defined by leg positions, so we know how to rotate the body
		public Vector3 GetLegsPlaneNormal() {
			if (!initiated) return Vector3.up;

			Vector3 _up = up;
			Vector3 normal = _up;
			
			// Go through all the legs, rotate the normal by it's offset
			for (int i = 0; i < legs.Length; i++) {
				// Direction from the root to the leg
				Vector3 legDirection = legs[i].IKPosition - root.position; 
				
				// Find the tangent
				Vector3 legNormal = _up;
				Vector3 legTangent = legDirection;
				Vector3.OrthoNormalize(ref legNormal, ref legTangent);
				
				// Find the rotation offset from the tangent to the direction
				Quaternion fromTo = Quaternion.FromToRotation(legTangent, legDirection);

				// Rotate the normal
				normal = fromTo * normal;
			}
			
			return normal;
		}

		// Set everything to 0
		public void Reset() {
			if (!Application.isPlaying) return;
			pelvis.Reset();
			foreach (Leg leg in legs) leg.Reset();
		}

		#endregion Main Interface
		
		private bool initiated;

		// Logs the warning if no other warning has beed logged in this session.
		public void LogWarning(string message) {
			Warning.Log(message, root);
		}
		
		// The up vector in solver rotation space.
		public Vector3 up {
			get {
				return (useRootRotation? root.up: Vector3.up);
			}
		}
		
		// Gets the vertical offset between two vectors in solver rotation space
		public float GetVerticalOffset(Vector3 p1, Vector3 p2) {
			if (useRootRotation) {
				Vector3 v = Quaternion.Inverse(root.rotation) * (p1 - p2);
				return v.y;
			}
			
			return p1.y - p2.y;
		}
		
		// Flattens a vector to ground plane in solver rotation space
		public Vector3 Flatten(Vector3 v) {
			if (useRootRotation) {
				Vector3 tangent = v;
				Vector3 normal = root.up;
				Vector3.OrthoNormalize(ref normal, ref tangent);
				return Vector3.Project(v, tangent);
			}
			
			v.y = 0;
			return v;
		}
		
		// Determines whether to use root rotation as solver rotation
		private bool useRootRotation {
			get {
				if (!rotateSolver) return false;
				if (root.up == Vector3.up) return false;
				return true;
			}
		}

        public float ForwardRaycastRoof(Transform pelvis, Color cast)
        {
            RaycastHit hit = new RaycastHit();
            Physics.BoxCast(pelvis.position + pelvisRaycastOffset, pelvisRaycastSize, root.forward, out hit, Quaternion.LookRotation(root.forward, root.up), pelvisRaycastDistance, layers);
            //ExtDebug.DrawBoxCastBox(pelvis.position + pelvisRaycastOffset, pelvisRaycastSize, Quaternion.LookRotation(root.forward, root.up), root.forward, pelvisRaycastDistance, cast);
            float Distancecast;
            if (hit.collider != null)
            {
                float amountDecrease = Vector3.Distance(pelvis.position + pelvisRaycastOffset, hit.point) / pelvisRaycastDistance;
                
                Distancecast = RaycastUp(pelvis.position + pelvisRaycastOffset + root.forward * Vector3.Distance(pelvis.position, hit.point), cast) * PelvisLowerAmount.Evaluate(amountDecrease);
            }
            else
            {
                Distancecast = 0;
            }
            float upCast = RaycastUp(pelvis.position + pelvisRaycastOffset, cast) * PelvisLowerAmount.Evaluate(0);
            if(upCast >= Distancecast)
            {
                return upCast;
            }
            else
            {
                return Distancecast;
            }
        }

        float RaycastUp(Vector3 center, Color cast)
        {
            RaycastHit hit = new RaycastHit();
            Vector3 newCenter = center - new Vector3(0, pelvisRaycastSize.y, 0);
            Vector3 endCast = new Vector3(newCenter.x, newCenter.y + pelvisRaycastSize.y * 2, newCenter.z);
            Physics.Raycast(newCenter, root.up, out hit, pelvisRaycastSize.y * 2, layers);
            Debug.DrawLine(newCenter, endCast, cast);
            if(hit.collider != null)
            {
                return Vector3.Distance(endCast,hit.point) - pevlisLowerOffset;
            }
            return 0;
        }
	}
}


