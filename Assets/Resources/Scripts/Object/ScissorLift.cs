using UnityEngine;
using System.Collections;

public class ScissorLift : ActionObject
{
    //==================================
    //          Public Vars
    //==================================
    public bool m_bStartUp = true;
    public float m_fBlendSpeed = 1f;
    public bool m_bLook = true;
    public Transform m_tLookPoint;

    //==================================
    //          Internal Vars
    //==================================

    //==================================
    //          Private Vars
    //================================== 
    private Animator m_aController;
    private bool m_bUp = true;
    private float m_fBlend = 0f;
    private bool m_bStopped = false;

    //Inherited functions

    protected override void OnStart()
    {
        m_aController = GetComponent<Animator>();

        m_fBlend = m_bStartUp ? 0 : 1.1f;
        m_aController.SetFloat("ScissorBlend", m_fBlend);
        m_bUp = m_bStartUp;
    }

    protected override void OnUpdate()
    {
        if (!m_bUp)
        {
            if (m_fBlend < 1.1f)
            {
                m_fBlend += Time.deltaTime * m_fBlendSpeed;
            }
            else
            {
                m_fBlend = 1.1f;
                if (!m_bStopped)
                    PlaySound(SOUND_EVENT.SCISSOR_STOP);
                m_bStopped = true;
            }
        }
        else
        {
            if (m_fBlend > 0f)
            {
                m_fBlend -= Time.deltaTime * m_fBlendSpeed;
            }
            else
            {
                m_fBlend = 0f;
                if (!m_bStopped)
                    PlaySound(SOUND_EVENT.SCISSOR_STOP);
                m_bStopped = true;
            }
        }

        m_aController.SetFloat("ScissorBlend", m_fBlend);
    }

    //protected override void AnimalEnter(Animal a_animal) { }
    //protected override void AnimalExit(Animal a_animal) { }
    //public override void DoAction() { }
    public override void DoActionOn()
    {
        m_bUp = !m_bStartUp;
        m_bStopped = false;

        PlaySound(m_bUp ? SOUND_EVENT.SCISSOR_UP : SOUND_EVENT.SCISSOR_DOWN);

        if (m_bLook)
        {
            m_bLook = false;
            CameraController.Instance.ViewObject(m_tLookPoint.gameObject, 2, 1);//, 1, m_tLookPoint);
        }
    }

    public override void DoActionOff()
    {
        m_bUp = m_bStartUp;
        m_bStopped = false;

        PlaySound(m_bUp ? SOUND_EVENT.SCISSOR_UP : SOUND_EVENT.SCISSOR_DOWN);
    }
}
