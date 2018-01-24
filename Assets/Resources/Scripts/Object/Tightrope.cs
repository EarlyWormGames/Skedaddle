using UnityEngine;
using System.Collections;

public class Tightrope : ActionObject
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

    protected override void OnStart()
    {
        m_aRequiredAnimal = ANIMAL_NAME.LORIS;
    }
    protected override void OnUpdate()
    {
        if (Keybinding.GetKeyDown("Down") || Controller.GetDpadDown(ControllerDpad.Down) || Controller.GetStickPositionDown(true, ControllerDpad.Down))
        {
            DoAction();
        }
    }
    public override void AnimalEnter(Animal a_animal)
    {
        m_aCurrentAnimal = a_animal;
        ((Loris)a_animal).m_bHorizontalRope = true;
    }
    protected override void AnimalExit(Animal a_animal)
    {
        ((Loris)a_animal).m_bHorizontalRope = false;
    }
    public override void DoAction()
    {
        gameObject.layer = 20;
    }
    //public override void DoActionOn() { }
    //public override void DoActionOff() { }
}
