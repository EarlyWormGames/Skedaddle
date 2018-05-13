using UnityEngine;
using System.Collections;
using UnityEngine.InputNew;
using System.Collections.Generic;

public class AnimalReveal : MonoInteracter
{
    //==================================
    //          Public Vars
    //==================================
    public bool Done = false;
    public Animal m_aUnlockedAnimal;
    public Transform m_tCenter;

    public AnimalCardShow m_CardShow;
    //public RotatingObject m_Rotator;

    //==================================
    //          Private Vars
    //================================== 
    private Animator m_aAnimator;
    
    void Start()
    {
        m_aAnimator = GetComponent<Animator>();
    }

    protected override void DoInteract(Animal caller)
    {
        if (!Done)
            Done = true;
        else
            return;

        m_aUnlockedAnimal.m_bCanBeSelected = true;

        if (true)//m_Rotator == null)
        {
            m_aAnimator.SetBool("Raise", true);
        }
        else
        {
            //m_Rotator.DoActionOn();
        }

        SaveManager.UnlockAnimal(m_aUnlockedAnimal.m_eName);

        if (m_CardShow != null)
        {
            m_CardShow.Show();
        }
    }
}