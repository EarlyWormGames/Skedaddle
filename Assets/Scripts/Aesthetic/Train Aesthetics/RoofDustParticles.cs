using UnityEngine;
using System.Collections;

public class RoofDustParticles : TrainEffects {

    //////////////////////////////
    ///     Public Variables   ///
    //////////////////////////////

    public Vector2 m_v2NumberPlayedRange;
    public ParticleSystem[] m_psParticles;

    protected int[] m_iParticlesPlayed;
    protected int m_iNoParticlesPlayed;

    private float m_fTimer;
    private bool m_bStartTime;
    private bool m_bStarted;
    private bool m_bCycle;
    

    public override void OnStart()
    {
        m_bStartTime = false;
        m_fTimer = m_fMicroOffset;
    }
    public override void OnUpdate()
    {
        if (m_bStartTime)
        {
            m_fTimer -= Time.deltaTime;
            if(m_fTimer < 0)
            {
                if (!m_bStarted)
                {
                    m_fTimer = m_fMicroDuration;
                    m_bStarted = true;
                    m_bIsShaking = true;
                    m_bCycle = true;
                    m_iNoParticlesPlayed = Mathf.RoundToInt(Random.Range(m_v2NumberPlayedRange.x - 0.49f, m_v2NumberPlayedRange.y + 0.49f));
                    m_iParticlesPlayed = new int[m_iNoParticlesPlayed];
                    for (int i = 0; i < m_iParticlesPlayed.Length; i++)
                    {
                        DecidePlaying(i);
                    }
                }
                else
                {
                    m_bStarted = false;
                    m_bIsShaking = false;
                    m_bStartTime = false;
                }
            }
        }
    }
    public override void Active()
    {
        if (m_bCycle)
        {
            for(int i = 0; i < m_psParticles.Length; i++)
            {

            }
            m_bCycle = false;
        }
    }
    public override void Unactive()
    {

    }
    public override void OnActivation()
    {
        m_bStartTime = true;
        m_fTimer = m_fMicroOffset;
    }
    public override void OnDeactivation()
    {
    }

    public void DecidePlaying(int number)
    {
        int numberCheck = Mathf.RoundToInt(Random.Range(-0.49f, m_psParticles.Length - 0.51f)); 
    }

}
