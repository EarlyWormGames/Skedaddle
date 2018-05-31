using UnityEngine;
using System.Collections;

/// <summary>
/// seesaw based on physics
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class Seesaw : MonoBehaviour
{
    private Rigidbody m_Body;

    private Vector3 m_LastVel = Vector3.zero;

    // Use this for initialization
    void Start()
    {
        m_Body = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 velocity = m_Body.angularVelocity - m_LastVel;

        m_LastVel = m_Body.angularVelocity;
    }
}