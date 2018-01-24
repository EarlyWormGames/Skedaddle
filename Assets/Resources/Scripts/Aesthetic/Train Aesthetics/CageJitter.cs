using UnityEngine;
using System.Collections;

public class CageJitter : TrainEffects {

    //public variables
    public float m_fShakeIntensity;


    //Private variables
    private float m_fTimer;
    private Rigidbody m_rBody;
    private bool m_bStartTime;
    private bool m_bStarted;

    public override void OnStart()
    {
        m_rBody = GetComponent<Rigidbody>();
    }
    public override void OnUpdate()
    {
        if (m_bStartTime)
        {
            m_fTimer -= Time.deltaTime;
            if (m_fTimer <= 0)
            {
                if (!m_bStarted)
                {
                    m_fTimer = m_fMicroDuration;
                    m_bIsShaking = true;
                    m_bStarted = true;
                    m_rBody.AddForce(Vector3.up * m_fShakeIntensity);
                }
                else
                {
                    m_bIsShaking = false;
                    m_bStarted = false;
                    m_bStartTime = false;
                }
            }
        }

    }
    public override void OnActivation()
    {
        m_fTimer = m_fMicroOffset;
        m_bStartTime = true;
    }
}
