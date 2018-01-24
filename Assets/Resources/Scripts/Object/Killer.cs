using UnityEngine;
using System.Collections;

public class Killer : ActionObject
{
    //==================================
    //          Public Vars
    //==================================
    public DEATH_TYPE m_eDeathType;
    public bool m_bOnActivate = false;
    //==================================
    //          Internal Vars
    //==================================

    //==================================
    //          Private Vars
    //================================== 

    //Inherited functions

    //protected override void OnStart() { }
    protected override void OnUpdate()
    {
        if (!m_bOnActivate)
            return;

        if (m_aCurrentAnimal == null)
            return;
        else if ((m_aCurrentAnimal.m_oCurrentObject != this && m_aCurrentAnimal.m_oCurrentObject != null) || !m_aCurrentAnimal.m_bSelected)
            return;

        if (Keybinding.GetKeyDown("Action") || Controller.GetButtonDown(ControllerButtons.A))
        {
            DoAction();
        }
    }

    public override void AnimalEnter(Animal a_animal)
    {
        if(!m_bOnActivate)
            a_animal.Kill(m_eDeathType);
    }
    //protected override void AnimalExit(Animal a_animal) { }
    public override void DoAction()
    {
        m_aCurrentAnimal.Kill(m_eDeathType);
    }
    //public override void DoActionOn() { }
    //public override void DoActionOff() { }
}
