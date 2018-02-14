using UnityEngine;
using System.Collections;

public class AnimalReveal : ActionObject
{
    //==================================
    //          Public Vars
    //==================================

    public bool Done = false;
    public Animal m_aUnlockedAnimal;
    public Transform m_tCenter;

    public AnimalCardShow m_CardShow;
    public RotatingObject m_Rotator;

    //==================================
    //          Internal Vars
    //==================================


    //==================================
    //          Private Vars
    //================================== 
    private Animator m_aAnimator;


    //Inherited functions

    protected override void OnStart()
    {
        m_aAnimator = GetComponent<Animator>();
    }


    protected override void OnCanTrigger()
    {
        if (!Done)
        {
            base.OnCanTrigger();
        }
    }
    //protected override void AnimalEnter(Animal a_animal) { }
    //protected override void AnimalExit(Animal a_animal) { }
    public override void DoAction()
    {
        DoActionOn();
    }
    public override void DoActionOn()
    {
        if (!Done)
        {
            Done = true;
        }

        m_aUnlockedAnimal.m_bCanBeSelected = true;

        if (m_Rotator == null)
        {
            m_aAnimator.SetBool("Raise", true);
        }
        else
        {
            m_Rotator.DoActionOn();
        }

        SaveManager.UnlockAnimal(m_aUnlockedAnimal.m_eName);

        if (m_CardShow != null)
        {
            m_CardShow.Show();
        }
    }
    //public override void DoActionOff() { }
}
