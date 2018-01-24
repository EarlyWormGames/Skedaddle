using UnityEngine;
using System.Collections;
using RootMotion.FinalIK;

namespace RootMotion.FinalIK {

	/// <summary>
	/// Raycasting to the ground to redirect upper-body animation based on ground topography.
	/// </summary>
	public class TerrainOffset : MonoBehaviour {
		
		public AimIK aimIK; // Reference to the AimIK component
		public Vector3 raycastOffset = new Vector3(0f, 2f, 1.5f); // Offset from the character, in local space, to raycast from
		public LayerMask raycastLayers; // The layers we want to raycast at
		public float min = -2f, max = 2f; // Min and max for the offset
		public float lerpSpeed = 10f; // The speed of lerping the IKPosition to make things nice and smooth
        public float heightOffset = 0f;

		private RaycastHit hit;
		private Vector3 offset;

		void LateUpdate() {
			// Find the raycastOffset in world space
			Vector3 worldOffset = transform.rotation * raycastOffset;

			// Find how much higher is the ground at worldOffset relative to the character position.
			Vector3 realOffset = GetGroundHeightOffset(transform.position + worldOffset);

			// Smoothly lerp the offset value so it would not jump on sudden raycast changes
			offset = Vector3.Lerp(offset, realOffset, Time.deltaTime * lerpSpeed);

			// The default offset point at the character's height
			Vector3 zeroOffsetPosition = transform.position + new Vector3(worldOffset.x, 0f, worldOffset.z);

			// Make the Aim Transform look at the default offset point (So when we are on planar ground there will be nothing for Aim IK to do)
			aimIK.solver.transform.LookAt(zeroOffsetPosition + offset);
		}

		private Vector3 GetGroundHeightOffset(Vector3 worldPosition) {

			// Raycast to find how much higher is the ground at worldPosition relative to the character.
			if (Physics.Raycast(worldPosition, Vector3.down, out hit, raycastOffset.y * 2f)) {
				return Mathf.Clamp(hit.point.y - transform.position.y + heightOffset, min, max) * Vector3.up;
			}

			// Raycast found nothing so return zero
			return Vector3.zero;
		}

        void OnDrawGizmos()
        {
            Vector3 origin = transform.position + transform.rotation * raycastOffset;
            Vector3 endpoint = origin + Vector3.down * raycastOffset.y * 2;
            Gizmos.color = Color.green;
            Gizmos.DrawLine(origin, endpoint);
        }
    }
}
