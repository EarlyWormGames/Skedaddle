using UnityEngine;
using System.Collections.Generic;
using System;

/// <summary>
/// Swinging object from a point above.
/// </summary>
public class Pendulum : MonoBehaviour
{
    //==================================
    //          Public Vars
    //==================================
    [Header("Misc")]
    public bool m_bSwing = true;
    public Transform m_tSwingObject;
    public bool m_bDoOnce = false;

    [Header("Curves")]
    public AnimationCurve m_aRestCurve;
    public AnimationCurve m_aSwingCurve;

    [Header("Rotation")]
    public float LeftRotation;
    public float RightRotation;
    public float m_fSwingTime = 2f;
    public float m_fRestAngle;
    public float m_fRestTime = 2f;
    [Tooltip("Will the pendulum swing to the right after rest?")]
    public bool m_bRightOnRest = false;

    [Header("Smash")]
    public GameObject m_HeadObject;
    public GameObject m_goHeadShatter;
    public ParticleSystem m_psExplosion;
    public bool m_bDestroyOnSmash = false;
    public float m_fShardForce = 2;
    public float m_Force = 50;
    public float m_DeathForce = 10;
    public float m_DeathUpForce = 3;

    //==================================
    //          Internal Vars
    //==================================

    //==================================
    //          Private Vars
    //==================================
    private bool m_Left = true;
    private Vector3 m_TempVel = Vector3.zero;
    private bool m_bDone = false;
    private float m_RestLerp;
    private bool m_bWasRest = false;
    private bool m_bSwingDone = false;

    private Rigidbody[] m_rShards;
    
    private Vector3 m_v3StartPos;
    private Quaternion m_qSwingStart, m_qRestStartRotation;

    private float m_Timer = 0f;

    void Start()
    {
        m_qSwingStart = transform.rotation;

        m_bSwingDone = !m_bSwing;

        if (m_goHeadShatter != null)
        {
            m_rShards = m_goHeadShatter.GetComponentsInChildren<Rigidbody>();
        }
    }

    void Update()
    {
        if (!m_bSwing && m_bSwingDone)
        {
            if (!m_bWasRest)
            {
                //Reset some data, mostly in case m_bSwing was changed in the editor
                m_bWasRest = true;
                m_qRestStartRotation = m_tSwingObject.rotation;
                m_Timer = 0;
                m_Left = !m_bRightOnRest;
            }

            //Lerp to IDLE
            m_RestLerp += Time.deltaTime;
            float val = m_aRestCurve.Evaluate(Mathf.Min(m_RestLerp / m_fRestTime, 1));
            m_tSwingObject.rotation = Quaternion.Lerp(m_qRestStartRotation, Quaternion.Euler(0, 0, m_fRestAngle), val);
        }
        else
        {
            m_bSwingDone = false;
            if (m_bWasRest)
            {
                //Start lerping from the current position, instead of from the left/right
                m_bWasRest = false;
                m_RestLerp = 0;
                m_qSwingStart = transform.rotation;
            }

            //Increase timer, calculate from swing curve
            m_Timer = Mathf.Clamp(m_Timer + Time.deltaTime, 0, m_fSwingTime);
            float t = m_aSwingCurve.Evaluate(m_Timer / m_fSwingTime);
            Quaternion end = Quaternion.Euler(new Vector3(0, 0, m_Left ? LeftRotation : RightRotation));

            //Lerp to swing position
            m_tSwingObject.rotation = Quaternion.Lerp(m_qSwingStart, end, t);

            if (m_Timer >= m_fSwingTime)
            {
                //Change where the lerp starts, reset timer
                m_bSwingDone = true;
                m_Timer = 0;
                m_Left = !m_Left;
                m_qSwingStart = Quaternion.Euler(new Vector3(0, 0, !m_Left ? LeftRotation : RightRotation));
            }
        }
    }
	//LACHLAN'S CHANGES START
	public void SwingTimeChange(float newSwingTime)
	{
		m_fSwingTime = m_fSwingTime + newSwingTime;
	}
	//LACHLAN'S CHANGES END
    public void Switch()
    {
        m_bSwing = !m_bSwing;
    }

    public void ResetTimer()
    {
        m_Timer = 0;
        m_Left = false;
    }

    public void Swing()
    {
        if (m_bDone && m_bDoOnce)
            return;
        m_bDone = true;
        m_bSwing = true;
    }

    public void Stop()
    {
        if (m_bDoOnce)
            return;

        m_bSwing = false;
        m_RestLerp = 0;
    }

    void OnCollisionEnter(Collision a_col)
    {
        a_col.contacts[0].thisCollider.enabled = false;

        if (a_col.collider.GetComponent<BreakableObject>())
        {
            //Smash any breakable objects
            a_col.collider.GetComponent<BreakableObject>().Break();
        }
        else if (a_col.collider.GetComponentInParent<Animal>())
        {
            if (a_col.collider.GetComponentInParent<Animal>().Alive)
            {
                //DIE DIE DIE
                a_col.collider.GetComponentInParent<Animal>().Kill(DEATH_TYPE.SQUASH);

                if (m_bSwing)
                {
                    //Add force to the animal if the pendulum is swinging
                    float mass = a_col.collider.GetComponentInParent<Animal>().m_rBody.mass;
                    a_col.collider.GetComponentInParent<Animal>().m_rBody.AddForce(new Vector3((m_Left ? -1 : 1) * m_DeathForce, m_DeathUpForce, 0) * mass, ForceMode.Impulse);
                }
            }
        }

        if (m_bDestroyOnSmash)
        {
            m_psExplosion.Play();
            foreach (Rigidbody x in m_rShards)
            {
                x.isKinematic = false;
                x.useGravity = true;
                x.GetComponent<Transform>().parent = null;
                x.GetComponent<Collider>().enabled = true;
                x.AddForce(Vector3.left * m_Force, ForceMode.Impulse);
                x.AddForce(Vector3.forward * m_Force * m_fShardForce, ForceMode.Impulse);
            }
            Destroy(m_HeadObject);
        }
    }
}
