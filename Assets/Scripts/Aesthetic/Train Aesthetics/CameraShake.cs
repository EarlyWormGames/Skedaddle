using UnityEngine;
using System.Collections;

public class CameraShake : TrainEffects
{
    //public variables
    public float m_fShakeIntensity;
    public float m_fShakeSpeed;
    public float m_fNoiseFrequency;
    public float m_fNoiseIntensity;

    //Private variables
    private float m_fHeightOffset;
    private float m_fTimer;
    private float m_fShakePosition;
    private bool m_bStartTime;
    private bool m_bStarted;
    private CameraController m_Camera;

    public override void OnStart()
    {
        m_Camera = GetComponent<CameraController>();
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
                }
                else
                {
                    m_bIsShaking = false;
                    m_bStarted = false;
                    m_bStartTime = false;
                }
            }
        }
        if (EWEyeTracking.active)
            Unactive();
    }

    public override void OnActivation()
    {
        m_fTimer = m_fMicroOffset;
        m_bStartTime = true;
    }

    public override void OnDeactivation()
    {
        m_Camera.m_fYAdd = 0;
        m_fShakePosition = 0;
    }

    public override void Unactive()
    {
        m_Camera.m_fYAdd = 0;
    }

    public override void Active()
    {
        m_fShakePosition += Time.deltaTime;
        m_Camera.m_fYAdd = m_fHeightOffset + m_fShakeIntensity * Mathf.Sin(m_fShakeSpeed * m_fShakePosition + (Random.Range(-1, 1) * m_fNoiseFrequency)) * Random.Range(1, m_fNoiseIntensity > 1 ? m_fNoiseIntensity : 1);

        if (EWEyeTracking.active)
            Unactive();
    }
}
