using UnityEngine;
using System.Collections;


/// <summary>
/// Management of the "Settings Trunk" in the menu
/// </summary>
public class TrunkFolder : MonoBehaviour
{
    public Transform m_tUpPoint;
    public float m_fUpSpeed = 1f;
    public float m_fDownSpeed = 2f;

    private Vector3 m_v3Start;
    private Vector3 m_v3Target;
    private bool m_bUp;

    void Awake()
    {
        m_v3Start = transform.localPosition;
        m_v3Target = m_v3Start;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_v3Target != m_v3Start)
            m_v3Target = m_tUpPoint.localPosition;

        transform.localPosition = Vector3.Lerp(transform.localPosition, m_v3Target, Time.deltaTime * (m_bUp? m_fUpSpeed : m_fDownSpeed));
    }

    public void SwitchDirection()
    {
        m_v3Target = m_v3Target == m_v3Start ? m_tUpPoint.position : m_v3Start;
    }

    public void SetDirection(bool a_up)
    {
        m_bUp = a_up;
        if (a_up)
        {
            m_v3Target = m_tUpPoint.localPosition;
        }
        else
        {
            m_v3Target = m_v3Start;
        }
    }
}
