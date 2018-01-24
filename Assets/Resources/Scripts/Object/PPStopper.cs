using UnityEngine;
using System.Collections;

public class PPStopper : MonoBehaviour
{
    public bool m_bIsRight = false;

    void OnTriggerEnter(Collider a_col)
    {
        PPStopBox stopbox = a_col.GetComponent<PPStopBox>();
        if (stopbox == null)
        {
            return;
        }

        Animal anim = stopbox.m_pConnectedObject.m_aPushingAnimal;
        if (anim != null)
        {
            if (anim.m_bPullingObject)
            {
                anim.m_rBody.velocity = Vector3.zero;
                if (!m_bIsRight)
                {
                    anim.m_bCanWalkLeft = false;
                    stopbox.m_pConnectedObject.m_bStopLeft = true;
                }
                else
                {
                    anim.m_bCanWalkRight = false;
                    stopbox.m_pConnectedObject.m_bStopRight = true;
                }
            }
        }
    }

    void OnTriggerExit(Collider a_col)
    {
        PPStopBox stopbox = a_col.GetComponent<PPStopBox>();
        if (stopbox == null)
        {
            return;
        }

        Animal anim = stopbox.m_pConnectedObject.m_aPushingAnimal;
        if (anim != null)
        {
            if (anim.m_bPullingObject)
            {
                if (!m_bIsRight)
                {
                    anim.m_bCanWalkLeft = true;
                    stopbox.m_pConnectedObject.m_bStopLeft = false;
                }
                else
                {
                    anim.m_bCanWalkRight = true;
                    stopbox.m_pConnectedObject.m_bStopRight = false;
                }
            }
        }
    }
}
