using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectJitter : TrainEffects {

    public Vector3 m_v3ConstantForce;
    public Vector3 m_v3AddBounds;
    public Vector3 m_v3SubBounds;
    public float m_fShakeAmplitude;
    public float m_fShakeFrequency;
    public float m_fShakeOffset;

    private Rigidbody m_rBody;
    private float m_fTimer;

    void Start()
    {
        m_rBody = GetComponent<Rigidbody>();  
    }

    public override void Active()
    {
        m_fTimer += Time.deltaTime;
        Vector3 difference = m_fShakeAmplitude * Vector3.one * Mathf.Sin(m_fTimer * m_fShakeFrequency + m_fShakeOffset);
        if(difference.x >= m_v3AddBounds.x)
        {
            difference.x = m_v3AddBounds.x;
        }
        else if (difference.x <= m_v3SubBounds.x)
        {
            difference.x = m_v3SubBounds.x;
        }
        if (difference.y >= m_v3AddBounds.y)
        {
            difference.y = m_v3AddBounds.y;
        }
        else if (difference.y <= m_v3SubBounds.y)
        {
            difference.y = m_v3SubBounds.y;
        }
        if (difference.z >= m_v3AddBounds.z)
        {
            difference.z = m_v3AddBounds.z;
        }
        else if (difference.z <= m_v3SubBounds.z)
        {
            difference.z = m_v3SubBounds.z;
        }
        m_rBody.AddForce(m_v3ConstantForce + difference);
    }
}
