using UnityEngine;
using System.Collections;

public class Tunnel : ActionObject
{
    //==================================
    //          Public Vars
    //==================================
    public Transform m_tEndPoint;

    //==================================
    //          Internal Vars
    //==================================

    //==================================
    //          Private Vars
    //================================== 

    //Inherited functions

    protected override void OnStart()
    {
        m_aRequiredAnimal = ANIMAL_NAME.ANTEATER;
    }
    //protected override void OnUpdate() { }
    //protected override void AnimalEnter(Animal a_animal) { }
    //protected override void AnimalExit(Animal a_animal) { }
    public override void DoAction()
    {
        m_aCurrentAnimal.transform.position = m_tEndPoint.position;
    }
    //public override void DoActionOn() { }
    //public override void DoActionOff() { }
}
