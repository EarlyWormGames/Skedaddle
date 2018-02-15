using UnityEngine;
using System.Collections;

public class ScissorLift : MonoBehaviour
{
    //==================================
    //          Public Vars
    //==================================
    public bool m_bStartUp = true;
    public float m_fBlendSpeed = 1f;
    public bool m_bLook = true;
    public Transform m_tLookPoint;

    [Header("Sounds")]
    public NamedEvent[] m_aSoundEvents;
    public string RaiseEventKey;
    public string LowerEventKey;
    public string StopEventKey;

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

    void Start()
    {
        m_aController = GetComponent<Animator>();

        m_fBlend = m_bStartUp ? 0 : 1.1f;
        m_aController.SetFloat("ScissorBlend", m_fBlend);
        m_bUp = m_bStartUp;
    }

    void Update()
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
                    NamedEvent.TriggerEvent(StopEventKey, m_aSoundEvents);
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
                    NamedEvent.TriggerEvent(StopEventKey, m_aSoundEvents);
                m_bStopped = true;
            }
        }

        m_aController.SetFloat("ScissorBlend", m_fBlend);
    }

    public void Raise()
    {
        m_bUp = true;
        m_bStopped = false;

        NamedEvent.TriggerEvent(RaiseEventKey, m_aSoundEvents);

        if (m_bLook)
        {
            m_bLook = false;
            CameraController.Instance.ViewObject(m_tLookPoint.gameObject, 2, 1);//, 1, m_tLookPoint);
        }
    }

    public void Lower()
    {
        m_bUp = false;
        m_bStopped = false;
        
        NamedEvent.TriggerEvent(LowerEventKey, m_aSoundEvents);
    }
}
