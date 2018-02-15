using UnityEngine;
using System.Collections;

public class TrainLightFlicker : TrainEffects {

    public Light[] m_laLightSources;
    public float m_fLightStrength;
    public float m_fLightFlickerSpeed;
    public float m_fLightBounceHeight;
    public float m_fLightSwingRestMulti;
    public float m_fLightSwingRestSpeed;
    public float m_fLightSwingBounceMulti;
    public float m_fLightSwingBounceSpeed;

    private float m_fTimer;
    private float m_fSwingTimer;
    private float m_LightSwingLeftRight;
    private float m_LightSwingForwardBack;
    private float m_LightSwingHeight;
    private float[] m_fLightInitalIntensity;
    private bool m_bStarted;
    private bool m_bStartTime;
    private Animator m_anim;

    public override void OnStart()
    {
        m_fLightInitalIntensity = new float[m_laLightSources.Length];
        for (int i = 0; i < m_fLightInitalIntensity.Length; i++)
        {
            if (i > m_laLightSources.Length - 1)
                break;

            if (m_laLightSources[i] == null)
                continue;

            m_fLightInitalIntensity[i] = m_laLightSources[i].intensity;
        }
        m_anim = GetComponent<Animator>();
        m_fSwingTimer = Random.Range(0, 10);
        m_fTimer = m_fMicroOffset;
        m_bStartTime = false;

        m_LightSwingForwardBack = 0f;
        m_LightSwingHeight = 0f;
    }

    public override void OnUpdate()
    {
        if (m_bStartTime)
        {
            m_fTimer -= Time.deltaTime;
            if (m_fTimer < 0)
            {
                if (!m_bStarted)
                {
                    m_fTimer = m_fMicroDuration;
                    m_bStarted = true;
                    m_bIsShaking = true;
                }
                else
                {
                    m_bStarted = false;
                    m_bIsShaking = false;
                    m_fTimer = m_fMicroOffset;
                    m_bStartTime = false;
                }
            }
        }
        m_fSwingTimer += Time.deltaTime;
        m_anim.SetFloat("Height", m_LightSwingHeight);
        m_anim.SetFloat("ForwardBack Swing", m_LightSwingLeftRight);
        m_anim.SetFloat("LeftRight Swing", m_LightSwingForwardBack);
    }

    public override void Active()
    {
        m_LightSwingLeftRight = Mathf.Lerp( m_LightSwingLeftRight, m_fLightSwingBounceMulti * Mathf.Sin(m_fLightSwingBounceSpeed * m_fSwingTimer), Time.deltaTime);
        float rand;
        if (m_fLightStrength <= 1)
        {
            rand = Random.Range(m_fLightStrength, 1);
        } 
        else
        {
            rand = Random.Range(1, m_fLightStrength);
        }
        for (int i = 0; i < m_fLightInitalIntensity.Length; i++)
        {
            if (i > m_laLightSources.Length - 1)
                break;

            if (m_laLightSources[i] == null)
                continue;

            m_laLightSources[i].intensity = Mathf.Lerp(m_laLightSources[i].intensity, rand * m_fLightInitalIntensity[i], m_fLightFlickerSpeed);
        }
    }

    public override void Unactive()
    {
        m_LightSwingLeftRight = Mathf.Lerp(m_LightSwingLeftRight, m_fLightSwingRestMulti * Mathf.Sin(m_fLightSwingRestSpeed * m_fSwingTimer), Time.deltaTime);
        for (int i = 0; i < m_fLightInitalIntensity.Length; i++)
        {
            if (i > m_laLightSources.Length - 1)
                break;

            if (m_laLightSources[i] == null)
                continue;

            m_laLightSources[i].intensity = Mathf.Lerp(m_laLightSources[i].intensity, m_fLightInitalIntensity[i], m_fLightFlickerSpeed);
        }
    }

    public override void OnActivation()
    {
        m_bStartTime = true;
        m_fTimer = m_fMicroOffset;
    }

    public override void OnDeactivation()
    {
    }
}
