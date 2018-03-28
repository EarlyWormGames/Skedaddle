using UnityEngine;
using System.Collections;
using RootMotion.FinalIK;

namespace RootMotion.FinalIK
{
    public class TerrainDetection : MonoBehaviour
    {
        public bool showDebug = true;
        public bool overrideTarget = true;
        public bool UseLocal = false;
        public Transform FirstBone;
        public Transform ForwardDirection;
        public float IKDistance;
        public IK effectedIK;
        public Vector3 raycastOffset = new Vector3(0f, 2f, 1.5f); // Offset from the character, in local space, to raycast from
        public LayerMask raycastLayers; // The layers we want to raycast at
        public float lerpSpeed = 10f; // The speed of lerping the IKPosition to make things nice and smooth
        public float heightOffset = 0f;
        public float angleBuffer = 1;

        private RaycastHit hit;
        private Vector3 direction;
        private Vector3 FirstHit;
        private Vector3 finalHit;
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
            detectedIK.target.transform.position = transform.position + Vector3.down * raycastOffset.y;
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
                    detectedIK.target.transform.position = Vector3.Lerp(detectedIK.target.transform.position, finalHit + Vector3.up * heightOffset, Time.deltaTime * lerpSpeed);
                }

                detectedIK.IKPositionWeight = Mathf.Lerp(detectedIK.IKPositionWeight, IKtoggle, Time.deltaTime * lerpSpeed);
            }
        }

        private void GetGroundHeightOffset(Vector3 worldPosition)
        {
            RaycastHit preHit;
            Physics.Raycast(FirstBone.position, -ForwardDirection.right + raycastOffset, out preHit, IKDistance, raycastLayers);
            FirstHit = preHit.point;
            if (preHit.collider != null)
            {
                float pythag = Mathf.Pow(IKDistance, 2) - Mathf.Pow(Vector3.Distance(FirstBone.position, preHit.point), 2);
                float newHeight = Mathf.Sqrt(pythag >= 0 ? pythag : 0);
                IKtoggle = 1;
                if (Physics.Raycast(preHit.point + new Vector3(0, newHeight, 0) + -ForwardDirection.right * 0.01f, UseLocal ? (Vector3.down - new Vector3(transform.rotation.x * angleBuffer, 0, 0)) : Vector3.down, out hit, newHeight * 2, raycastLayers))
                {
                    finalHit = hit.point;
                }
                else
                {
                    finalHit = preHit.point + new Vector3(0, newHeight, 0);
                }
            }
            else
            {
                IKtoggle = 0.0001f;
            }
        }

        void OnDrawGizmos()
        {
            if (showDebug)
            {
                Vector3 origin = FirstBone.position;
                Vector3 endpoint = origin + Vector3.Normalize(-ForwardDirection.right + raycastOffset) * IKDistance;
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(hit.point, 0.01f);
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(origin, endpoint);
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(FirstHit, 0.01f);
                if (Application.isPlaying)
                {
                    Gizmos.color = Color.blue;
                    Gizmos.DrawSphere(detectedIK.target.transform.position, 0.01f);
                }
            }
        }
    }
}
