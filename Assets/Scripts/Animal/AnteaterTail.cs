using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnteaterTail : MonoBehaviour {

    public Animator m_AnteaterAnimator;
    public Transform m_Origin;
    public Transform m_ForwardDirection;
    public Vector3 m_BoxDimensions;
    public LayerMask m_raycastLayers;
    public float m_RaycastDistance;
    public float m_LerpSpeed;

    private float CurlAmount;

    void Update()
    {
        if(m_RaycastDistance != 0)
        {
            RaycastHit hit;
            Physics.BoxCast(m_Origin.position - new Vector3(0, m_BoxDimensions.y / 2, 0), m_BoxDimensions, -m_ForwardDirection.forward, out hit, Quaternion.LookRotation(m_ForwardDirection.forward, m_ForwardDirection.up), m_RaycastDistance, m_raycastLayers);
            ExtDebug.DrawBoxCastBox(m_Origin.position - new Vector3(0, m_BoxDimensions.y / 2, 0), m_BoxDimensions, Quaternion.LookRotation(m_ForwardDirection.forward, m_ForwardDirection.up), -m_ForwardDirection.forward, m_RaycastDistance, Color.red);
            if (hit.collider != null)
            {
                CurlAmount = 1 - (hit.distance / m_RaycastDistance);
            }
            else
            {
                CurlAmount = 0;
            }
            m_AnteaterAnimator.SetFloat("CurlTail", Mathf.Lerp(m_AnteaterAnimator.GetFloat("CurlTail"), CurlAmount, Time.deltaTime * m_LerpSpeed));
        }
    }
}
