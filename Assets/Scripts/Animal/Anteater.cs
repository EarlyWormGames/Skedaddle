using UnityEngine;
using System.Collections;
using RootMotion.FinalIK;

public class Anteater : Animal
{
    //==================================
    //          Public Vars
    //==================================

    public float            m_fIdleSwapTime;
    public float            m_fIdleSwapVarience;
    public int              m_iNumberOfIdles;
    public AimIK            m_ikHead;

    //==================================
    //          Internal Vars
    //==================================

    //==================================
    //          Private Vars
    //================================== 

    private float           m_fIdleTimer;
    private int             m_iCurrentIdle;
    

    //Inherited functions
    protected override void OnAwake()
    {
        m_eName = ANIMAL_NAME.ANTEATER;
        m_eSize = ANIMAL_SIZE.M;
        m_fIdleTimer = Random.Range(m_fIdleSwapTime - m_fIdleSwapVarience, m_fIdleSwapTime + m_fIdleSwapVarience);
        m_iCurrentIdle = 1;
    }

    protected override void OnUpdate()
    {
        //Animator Setting
        m_fIdleTimer -= Time.deltaTime;
        m_aAnimalAnimator.SetBool("Controlled", m_bSelected);
        m_aAnimalAnimator.SetBool("OnGround", m_bOnGround);
        m_aAnimalAnimator.SetBool("Dead", !Alive);
        m_aAnimalAnimator.SetFloat("Vertical Velocity", m_rBody.velocity.y);
        if ((m_bWalkingLeft ^ m_bWalkingRight))
        {
            m_aAnimalAnimator.SetBool("Walking", true);
        }
        else
        {
            m_aAnimalAnimator.SetBool("Walking", false);
        }
 

        if(m_fIdleTimer < 0)
        {
            if(m_iCurrentIdle == 1)
            {
                float randomIdle = Random.Range(1.6f, m_iNumberOfIdles + 0.4f);
                m_iCurrentIdle = Mathf.RoundToInt(randomIdle);
                m_fIdleTimer = 3f;
            } else
            {
                m_iCurrentIdle = 1;
                m_fIdleTimer = Random.Range(m_fIdleSwapTime - m_fIdleSwapVarience, m_fIdleSwapTime + m_fIdleSwapVarience);
            }
        }

        //m_ikHead.solver.axis.z = m_bFacingLeft ? 1 : -1;

        m_aAnimalAnimator.SetInteger("IdleNo", m_iCurrentIdle);
    }
    //protected override void OnFixedUpdate() { }
    //protected override void OnDeath(DEATH_TYPE a_type) { }
}