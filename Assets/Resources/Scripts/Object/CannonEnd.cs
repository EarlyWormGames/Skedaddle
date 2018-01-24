using UnityEngine;
using System.Collections;

public class CannonEnd : ActionObject
{
    //==================================
    //          Public Vars
    //==================================
    public Cannon m_cLauncher;
    public Trampampoline m_tLauncher;

    public bool m_bForAnimal = true;

    //==================================
    //          Internal Vars
    //==================================

    //==================================
    //          Private Vars
    //================================== 

    //Inherited functions

    //protected override void OnStart() { }
    protected override void OnUpdate() { }
    public override void AnimalEnter(Animal a_animal)
    {
        if (!m_bForAnimal)
            return;

        if (m_cLauncher != null)
            m_cLauncher.End();

        if(m_tLauncher != null)
            m_tLauncher.End();
    }

    protected override void ObjectEnter(Collider a_col)
    {
        if (m_bForAnimal)
            return;

        if (m_tLauncher != null)
            m_tLauncher.End();
    }
    //protected override void AnimalExit(Animal a_animal) { }
    //public override void DoAction() { }
    //public override void DoActionOn() { }
    //public override void DoActionOff() { }
}
