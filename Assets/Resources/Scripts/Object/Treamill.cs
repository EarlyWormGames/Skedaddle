using UnityEngine;
using System.Collections;

public class Treamill : ActionObject
{
    //==================================
    //          Public Vars
    //==================================
    public Vector3 m_v3Force;
    public Animator m_mrAnimator;
    public float m_fAnimationSpeedMultiplier;

    //==================================
    //          Internal Vars
    //==================================
    public bool m_bOn = true;
    //==================================
    //          Private Vars
    //================================== 
    private float m_fAnimationTime = 0;
    //Inherited functions

    //protected override void OnStart() { }

    void OnDisable()
    {
        m_mrAnimator.SetFloat("Speed", 0);
    }
    void FixedUpdate() 
    {
        
        m_fAnimationTime = m_v3Force.x * -m_fAnimationSpeedMultiplier;
        m_mrAnimator.SetFloat("Speed", m_fAnimationTime);

        if (m_aCurrentAnimal == null)
            return;
        m_aCurrentAnimal.m_rBody.AddForce(m_v3Force);
        
    }
}
