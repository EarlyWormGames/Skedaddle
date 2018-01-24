using UnityEngine;
using System.Collections;

public class Slide : ActionObject
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
    public override void AnimalEnter(Animal a_animal)
    {
        Animator a = a_animal.GetComponentInChildren<Animator>();
        a.SetBool("OnSlide", true);
    }
    protected override void AnimalExit(Animal a_animal)
    {
        Animator a = a_animal.GetComponentInChildren<Animator>();
        a.SetBool("OnSlide", false);
    }
    //public override void DoAction() { }
    //public override void DoActionOn() { }
    //public override void DoActionOff() { }
}
