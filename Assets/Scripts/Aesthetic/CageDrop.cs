using UnityEngine;
using System.Collections;

public class CageDrop : ActionObject
{

    public HingeJoint[] m_hjRope;
    public GameObject m_goBalancer;

    public Collider m_Floor;
    public Collider[] m_MyColliders = new Collider[0];
    public Collider[] m_CanCollide = new Collider[0];

    private Animator m_aAnim;
    private CageJitter m_cjEffects;

    private bool m_bDropped = false;

    protected override void OnStart()
    {
        m_aAnim = GetComponent<Animator>();
        m_aAnim.enabled = false;
        m_rBody = GetComponent<Rigidbody>();
        m_cjEffects = GetComponent<CageJitter>();


        //Collider[] allCols = FindObjectsOfType<Collider>();
        //
        //for (int i = 0; i < allCols.Length; ++i)
        //{
        //    bool col = true;
        //    for (int j = 0; j < m_CanCollide.Length; ++j)
        //    {
        //        if (m_CanCollide[j] == allCols[i])
        //        {
        //            col = false;
        //            break;
        //        }
        //    }
        //
        //    if (col)
        //    {
        //        for (int j = 0; j < m_MyColliders.Length; ++j)
        //        {
        //            Physics.IgnoreCollision(m_MyColliders[j], allCols[i]);
        //        }
        //    }
        //}
    }

    protected override void OnCanTrigger()
    {
        if (!m_bDropped)
        {
            //if (Keybinding.AnyKeyDown() || Controller.AnyButtonDown())
            //{
                m_bDropped = true;

                m_aAnim.enabled = true;
                m_aAnim.SetBool("Release", true);

                //StartCoroutine(StopRig());
                //PlaySound(SOUND_EVENT.ROPE_SNAP);
                //for (int i = 0; i < m_hjRope.Length; i++)
                //{
                //    if (i == 0)
                //    {
                //        m_hjRope[i].connectedBody = m_rBody;
                //    }
                //    else
                //    {
                //        m_hjRope[i].connectedBody = m_hjRope[i - 1].GetComponent<Rigidbody>();
                //    }
                //}
                //
                if (GameTimer.InstanceExists())
                    GameTimer.StartTimer();
                //
                //Destroy(m_cjEffects);
                //Destroy(GetComponent<FixedJoint>());
                //Destroy(m_goBalancer);
                //Destroy(this);
            //}
        }
    }

    IEnumerator StopRig()
    {
        yield return new WaitForSeconds(2);

        m_rBody.isKinematic = true;
        m_rBody.useGravity = false;
    }

    void OnCollisionEnter(Collision a_col)
    {
        //if (a_col.collider == m_Floor)
        //{
        //    //m_rBody.isKinematic = true;
        //    //m_rBody.useGravity = false;
        //    m_aAnim.enabled = true;
        //    m_aAnim.SetBool("Release", true);
        //}
    }
}
