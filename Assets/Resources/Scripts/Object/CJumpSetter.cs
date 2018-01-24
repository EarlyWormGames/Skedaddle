using UnityEngine;
using System.Collections;

public class CJumpSetter : MonoBehaviour
{
    public LadderStopper m_lStopper;
    public ClimbJump m_cJump;

    void OnTriggerEnter(Collider a_col)
    {
        if (a_col.gameObject == m_lStopper.gameObject)
        {
            m_lStopper.m_cEndClimb = m_cJump;
        }
    }

    void OnTriggerExit(Collider a_col)
    {
        if (a_col.gameObject == m_lStopper.gameObject)
        {
            m_lStopper.m_cEndClimb = null;
        }
    }
}
