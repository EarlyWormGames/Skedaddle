using UnityEngine;
using System.Collections;
using RootMotion.FinalIK;

namespace RootMotion.FinalIK
{
    public class TerrainDetection : MonoBehaviour
    {
        public bool showDebug = true;
        public bool overrideTarget = true;
        public IK effectedIK;
        public Vector3 raycastOffset = new Vector3(0f, 2f, 1.5f); // Offset from the character, in local space, to raycast from
        public LayerMask raycastLayers; // The layers we want to raycast at
        public float min = -2f, max = 2f; // Min and max for the offset
        public float lerpSpeed = 10f; // The speed of lerping the IKPosition to make things nice and smooth
        public float heightOffset = 0f;
        public float angleBuffer = 1;

        private RaycastHit hit;
        private Vector3 offset;
        private IKSolverHeuristic detectedIK; // Reference to the IK component
        private float IKtoggle = 1;

        void Awake()
        {
            detectedIK = (IKSolverHeuristic)effectedIK.GetIKSolver();
            if(detectedIK == null)
            {
                Debug.Log("IK needs to be Heuristic");
                Debug.Break();
            }
        }

        void LateUpdate()
        {
            // Find the raycastOffset in world space
            Vector3 worldOffset = transform.rotation * raycastOffset;

            GetGroundHeightOffset(transform.position + worldOffset) ;

            if (overrideTarget)
            {
                // Move Target To Ground Point
                if (IKtoggle == 1)
                {
                    detectedIK.target.transform.position = Vector3.Lerp(detectedIK.target.transform.position, hit.point + Vector3.up * heightOffset, Time.deltaTime * lerpSpeed);
                }

                detectedIK.IKPositionWeight = Mathf.Lerp(detectedIK.IKPositionWeight, IKtoggle, Time.deltaTime * lerpSpeed);
            }
        }

        private void GetGroundHeightOffset(Vector3 worldPosition)
        {
            if (Physics.Raycast(worldPosition, (Vector3.down - new Vector3(transform.rotation.x * angleBuffer, 0, 0)), out hit, raycastOffset.y * 2f, raycastLayers))
            {
                IKtoggle = 1;
                return;
            }

            IKtoggle = 0;
        }

        void OnDrawGizmos()
        {
            if (showDebug)
            {
                Vector3 origin = transform.position + transform.rotation * raycastOffset;
                Vector3 endpoint = origin + (Vector3.down - new Vector3(transform.rotation.x * angleBuffer, 0, 0)) * raycastOffset.y * 2;
                Gizmos.color = Color.green;
                Gizmos.DrawLine(origin, endpoint);
            }
        }
    }
}
