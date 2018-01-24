using UnityEngine;
using System.Collections;

public class FallObject : ActionObject
{
    //==================================
    //          Public Vars
    //==================================

    //==================================
    //          Internal Vars
    //==================================

    //==================================
    //          Private Vars
    //================================== 

    //Inherited functions

    //protected override void OnStart() { }
    //protected override void OnUpdate() { }
    //protected override void AnimalEnter(Animal a_animal) { }
    //protected override void AnimalExit(Animal a_animal) { }

    public override void DoActionOn()
    {
        if (m_rBody != null)
            m_rBody.isKinematic = false;
    }

    public override void DoActionOff()
    {

    }
}
