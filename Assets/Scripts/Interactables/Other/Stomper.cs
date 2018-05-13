using UnityEngine;
using System.Collections;

public class Stomper : MonoBehaviour
{
    public enum eStompState
    {
        FALL,
        TOP_WAIT,
        LAUNCH,
        WAIT,
        CLIMB
    }

    //==================================
    //          Public Vars
    //==================================
    public Rigidbody m_rBody;

    public float m_fStompSpeed = 10f;
    public Transform m_tBottom;
    public float m_fBottomWaitTime = 1f;
    public float m_fLerpTime = 6f;
    public float m_fTopWaitTime = 1f;
    public AnimationCurve m_aCurve;
    public ParticleSystem m_psDustParticle;

    public float m_fStartWait = 0f;

    //==================================
    //          Internal Vars
    //==================================

    //==================================
    //          Private Vars
    //==================================
    private float m_fTimer = 0f;
    private Vector3 m_v3TopPoint;
    private Vector3 m_v3StartPoint;
    private eStompState m_eState = eStompState.LAUNCH;
    private bool m_bLaunched = false;

    //Inherited functions

    void Start()
    {
        m_v3TopPoint = transform.position;
        if (m_fStartWait > 0)
        {
            m_rBody.isKinematic = true;
            m_eState = eStompState.TOP_WAIT;
        }
    }

    void Update()
    {
        if (m_eState == eStompState.TOP_WAIT)
        {
            m_fTimer += Time.deltaTime;

            if (m_fTimer > (m_bLaunched? m_fTopWaitTime : m_fStartWait))
            {
                m_rBody.isKinematic = false;
                m_eState = eStompState.LAUNCH;
            }
        }
        if (m_eState == eStompState.WAIT)
        {
            m_fTimer += Time.deltaTime;
            if (m_fTimer >= m_fBottomWaitTime)
            {
                m_fTimer = 0f;
                m_eState = eStompState.CLIMB;
            }
        }
        else if (m_eState == eStompState.CLIMB)
        {
            m_fTimer += Time.deltaTime;
            float t = Mathf.Min(m_fTimer / m_fLerpTime, 1f);
            transform.position = Vector3.Lerp(m_v3StartPoint, m_v3TopPoint, t);

            if (t >= 1f)
            {
                m_fTimer = 0f;
                m_eState = eStompState.TOP_WAIT;
            }
        }
    }

    void FixedUpdate()
    {
        if (m_eState == eStompState.LAUNCH)
        {
            m_rBody.AddForce(Vector3.down * m_fStompSpeed, ForceMode.Impulse);
            m_eState = eStompState.FALL;
            m_bLaunched = true;
        }
    }

    void OnCollisionEnter(Collision a_col)
    {
        if (m_eState != eStompState.FALL)
            return;

        m_psDustParticle.Play();

        m_eState = eStompState.WAIT;
        m_fTimer = 0f;
        m_v3StartPoint = transform.position;
        m_rBody.isKinematic = true;
    }

    void OnTriggerEnter(Collider a_col)
    {
        if (m_eState != eStompState.FALL)
            return;

        if (a_col.gameObject.GetComponentInParent<Animal>())
        {
            a_col.gameObject.GetComponentInParent<Animal>().Kill(DEATH_TYPE.SQUASH);
        }
    }
}
