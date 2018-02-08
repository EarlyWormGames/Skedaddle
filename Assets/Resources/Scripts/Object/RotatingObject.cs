using UnityEngine;
using System.Collections;

public class RotatingObject : ActionObject
{
    //==================================
    //          Public Vars
    //==================================
    public Vector3 m_v3EndRotation;
    public float m_fSlideTime = 1f;

    public AnimationCurve m_aCurve;
    public AnimationCurve m_aReverseCurve;
    public bool m_bUseReverse = false;

    public string OpenEvent, CloseEvent;
    //==================================
    //          Internal Vars
    //==================================

    //==================================
    //          Private Vars
    //================================== 
    private bool m_bDown = false;
    private bool m_bRequiredState = false;
    private Vector3 m_v3StartRot;
    private bool m_bMove = false;
    private float m_fTimer;

    private bool m_bStartMove = false;

    //Inherited functions

    protected override void OnStart()
    {
        m_v3StartRot = transform.eulerAngles;
    }

    protected override void OnCanTrigger()
    {
        if (m_bMove)
        {
            if (m_bStartMove)
            {
                m_bStartMove = false;
            }

            m_fTimer += Time.deltaTime;
            Vector3 start = m_bDown ? m_v3StartRot : m_v3EndRotation;
            Vector3 end = m_bDown ? m_v3EndRotation : m_v3StartRot;
            float t = Mathf.Min(1, m_fTimer / m_fSlideTime);
            t = m_bUseReverse && !m_bDown ? m_aReverseCurve.Evaluate(t) : m_aCurve.Evaluate(t);
            transform.rotation = Quaternion.Euler(Vector3.Lerp(start, end, t));

            if (m_fTimer >= m_fSlideTime)
            {
                OnSlideEnd();
            }
        }
    }
    //protected override void AnimalEnter(Animal a_animal) { }
    //protected override void AnimalExit(Animal a_animal) { }
    public override void DoAction()
    {        
        if (m_bMove)
            return;

        if (transform.rotation == Quaternion.Euler(m_v3StartRot))
        {
            m_bRequiredState = true;
            m_bDown = true;
        }
        else
        {
            m_bRequiredState = false;
            m_bDown = false;
        }

        NamedEvent.TriggerEvent(m_bDown ? OpenEvent : CloseEvent, m_aSoundEvents);
        m_bMove = true;
        m_fTimer = 0f;
    }

    public override void DoActionOn()
    {
        if (m_bMove)
            return;
        m_bRequiredState = true;
        m_bDown = true;
        m_bMove = true;
        m_fTimer = 0f;
        NamedEvent.TriggerEvent(m_bDown ? OpenEvent : CloseEvent, m_aSoundEvents);
    }

    public override void DoActionOff()
    {
        if (m_bMove)
            return;
        m_bRequiredState = false;
        m_bDown = false;
        m_bMove = true;
        m_fTimer = 0f;
        NamedEvent.TriggerEvent(m_bDown ? OpenEvent : CloseEvent, m_aSoundEvents);
    }

    void OnSlideEnd()
    {
        m_bStartMove = true;

        m_fTimer = 0f;
        m_bMove = false;

        if (m_bRequiredState != m_bDown)
        {
            m_bDown = m_bRequiredState;
            m_bMove = true;
            m_fTimer = 0f;
            NamedEvent.TriggerEvent(m_bDown ? OpenEvent : CloseEvent, m_aSoundEvents);
        }
    }
}
