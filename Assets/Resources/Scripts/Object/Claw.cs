using UnityEngine;
using System.Collections;

public class Claw : ActionObject
{
    //==================================
    //          Public Vars
    //==================================

    public GameObject m_goObjectToDrop;
    public float m_fOpenDuration;

    //==================================
    //          Internal Vars
    //==================================

    internal Animator m_aAnimator;

    //==================================
    //          Private Vars
    //==================================

    private float m_fTimer;
    private bool m_bStartTimer;

    //Inherited functions

    protected override void OnStart()
    {
        m_aAnimator = GetComponent<Animator>();
        m_aAnimator.SetBool("Grabbing", true);
    }
    protected override void OnUpdate()
    {
        if (m_bStartTimer)
        {
            m_fTimer -= Time.deltaTime;
            if (m_fTimer < 0)
            {
                m_aAnimator.SetBool("Closed", true);
                m_bStartTimer = false;
            }
        }

        
    }
    //protected override void AnimalEnter(Animal a_animal) { }
    //protected override void AnimalExit(Animal a_animal) { }
    //public override void DoAction() { }
    public override void DoActionOn()
    {
        m_aAnimator.SetBool("Open", true);
        m_fTimer = m_fOpenDuration;
        m_bStartTimer = true;

        m_goObjectToDrop.transform.parent = null;
        m_goObjectToDrop.GetComponent<Rigidbody>().isKinematic = false;
        m_goObjectToDrop.GetComponent<Rigidbody>().useGravity = true;
    }
    //public override void DoActionOff() { }
}
