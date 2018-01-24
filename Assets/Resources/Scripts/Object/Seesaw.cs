using UnityEngine;
using System.Collections;
using FMODUnity;

[RequireComponent(typeof(StudioEventEmitter))]
[RequireComponent(typeof(Rigidbody))]
public class Seesaw : MonoBehaviour
{
    private StudioEventEmitter m_Emitter;
    private Rigidbody m_Body;

    private Vector3 m_LastVel = Vector3.zero;

    // Use this for initialization
    void Start()
    {
        m_Emitter = GetComponent<StudioEventEmitter>();
        m_Body = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 velocity = m_Body.angularVelocity - m_LastVel;



        m_Emitter.SetParameter("SPEED", velocity.z);

        m_LastVel = m_Body.angularVelocity;
    }
}