using UnityEngine;
using System.Collections.Generic;

public class WaterStep : MonoBehaviour
{
    public float m_Force = 50;
    public Transform[] m_Feet;
    public HeightfieldWater m_Water;


    private ParticleSystem m_ParticleSystem;
    private List<ParticleSystem.Particle> enter = new List<ParticleSystem.Particle>();

    void Start()
    {
        m_ParticleSystem = GetComponent<ParticleSystem>();
        enter = new List<ParticleSystem.Particle>();

        if (m_Water == null)
            m_Water = FindObjectOfType<HeightfieldWater>();
    }

    public void Push(int a_index)
    {
        if (m_Water != null)
        {
            m_Water.AddForce(m_Feet[a_index].position, m_Force);
        }
    }

    void OnParticleTrigger()
    {
        if (m_Water != null)
        {            
            int numEnter = m_ParticleSystem.GetTriggerParticles(ParticleSystemTriggerEventType.Enter, enter);
            if (numEnter > 0)
                m_Water.AddForce(transform.TransformPoint(enter[0].position), m_Force);
        }
    }
}
