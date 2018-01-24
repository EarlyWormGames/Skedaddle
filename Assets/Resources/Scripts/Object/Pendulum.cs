using UnityEngine;
using System.Collections.Generic;
using System;

public class Pendulum : ActionObject
{
    //==================================
    //          Public Vars
    //==================================
    public bool m_showDebug;
    public bool m_bSwing = true;
    public float m_fSwingSpeed = 1f;
    public float m_fKillThreshold = 1f;
    public float m_fRestTime = 2f;
    public float m_fRestAngle = 0f;
    public AnimationCurve m_aRestCurve;
    public GameObject m_goHead;
    public ParticleSystem m_psExplosion;

    public Transform m_tAnchorPoint;

    public bool m_bOnce = false;
    public bool m_bDestroyOnSmash = false;
    public float m_fForwardMult = 0.8f;
    public float m_fShardForce = 2;

    public AnimationCurve m_SwingCurve;
    public float m_SwingTime = 1;
    public Vector3 m_EndRotation;

    public float m_Force = 50;
    public float m_DeathForce = 10;
    public float m_DeathUpForce = 3;

    public Transform m_tDebugCenter;
    public Vector3 m_fDebugSize;

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

    private Quaternion m_qRestRotation;
    private Quaternion m_qEndRotation;
    private Rigidbody[] m_rShards;

    private Quaternion m_qStartRotation;
    private Vector3 m_v3StartPos;

    private Quaternion m_qPoleStartRotation;
    private Vector3 m_v3PoleStartPos;

    private float m_Timer = 0f;

    protected override void OnStart()
    {
        m_qStartRotation = transform.localRotation;
        m_v3StartPos = transform.localPosition;

        m_qRestRotation = Quaternion.identity;
        //m_qEndRotation = Quaternion.Euler(m_tAnchorPoint.eulerAngles.x, m_tAnchorPoint.eulerAngles.y, m_EndRotation);

        if (m_goHead != null)
        {
            m_rShards = m_goHead.GetComponentsInChildren<Rigidbody>();
        }
    }

    protected override void OnFixedUpdate()
    {
        //if (m_Left && m_rBody.velocity.x >= 0)
        //    m_Left = false;
        //else if (!m_Left && m_rBody.velocity.x <= 0)
        //    m_Left = true;
        //
        //if (m_bSwing)
        //{
        //    m_rBody.AddForce(m_Left ? Vector3.left : -Vector3.left * m_fSwingSpeed);
        //}
    }

    protected override void OnUpdate()
    {
        if (!m_bSwing && m_tAnchorPoint != null)
        {
            m_RestLerp += Time.deltaTime;
            float val = m_aRestCurve.Evaluate(Mathf.Min(m_RestLerp / m_fRestTime, 1));
            m_tAnchorPoint.rotation = Quaternion.Lerp(m_tAnchorPoint.rotation, m_qRestRotation, val);
        }
        else if (m_bSwing && m_tAnchorPoint != null)
        {
            if (m_Left)
            {
                m_Timer += Time.deltaTime;
                m_tAnchorPoint.rotation = Quaternion.Euler(Vector3.Lerp(m_qRestRotation.eulerAngles, m_EndRotation, m_SwingCurve.Evaluate(m_Timer / m_SwingTime)));

                if (m_Timer >= m_SwingTime)
                    m_Left = false;
            }
            else
            {
                m_Timer -= Time.deltaTime;
                m_tAnchorPoint.rotation = Quaternion.Euler(Vector3.Lerp(m_qRestRotation.eulerAngles, m_EndRotation, m_SwingCurve.Evaluate(m_Timer / m_SwingTime)));

                if (m_Timer <= 0)
                    m_Left = true;
            }
        }
    }

    public override void DoAction()
    {
        m_bSwing = !m_bSwing;

        if (m_bSwing)
        {
        }
        else
        {
            m_rBody.isKinematic = true;
        }
    }

    public override void DoActionOn()
    {
        if (m_bDone && m_bOnce)
            return;

        m_qRestRotation = Quaternion.Euler(m_tAnchorPoint.eulerAngles.x, m_tAnchorPoint.eulerAngles.y, m_fRestAngle);
        m_bDone = true;
        m_bSwing = true;
    }

    public override void DoActionOff()
    {
        if (m_bOnce)
            return;

        m_bSwing = false;
        m_RestLerp = 0;

        m_qRestRotation = Quaternion.Euler(m_tAnchorPoint.eulerAngles.x, m_tAnchorPoint.eulerAngles.y, m_fRestAngle);

        m_tAnchorPoint.rotation = Quaternion.Euler(m_tAnchorPoint.eulerAngles.x, m_tAnchorPoint.eulerAngles.y, transform.eulerAngles.z);
        transform.localRotation = m_qStartRotation;
        transform.localPosition = m_v3StartPos;
    }

    void OnCollisionEnter(Collision a_col)
    {
        a_col.contacts[0].thisCollider.enabled = false;

        if (a_col.collider.GetComponent<BreakableObject>())
        {
            a_col.collider.GetComponent<BreakableObject>().DoAction();
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
                Destroy(gameObject);
            }
        }
        else if (a_col.collider.GetComponentInParent<Animal>())
        {
            if (a_col.collider.GetComponentInParent<Animal>().Alive)
            {
                a_col.collider.GetComponentInParent<Animal>().Kill(DEATH_TYPE.SQUASH);

                if (m_bSwing)
                {
                    float mass = a_col.collider.GetComponentInParent<Animal>().m_rBody.mass;
                    a_col.collider.GetComponentInParent<Animal>().m_rBody.AddForce(new Vector3((m_Left ? -1 : 1) * m_DeathForce, m_DeathUpForce, 0) * mass, ForceMode.Impulse);
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (m_showDebug && m_tDebugCenter != null)
        {
            Gizmos.color = new Color(1, 0, 0, 0.25f);
            Gizmos.DrawCube(m_tDebugCenter.position, m_fDebugSize);
        }
    }
}
