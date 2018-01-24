using UnityEngine;
using System.Collections;

public class DebugObject : ActionObject
{
    private bool m_bIsOn = false;

    //Inherited functions

    //protected override void OnStart() { }
    //protected override void OnUpdate() { }
    //protected override void AnimalEnter(Animal a_animal) { }
    //protected override void AnimalExit(Animal a_animal) { }
    public override void DoAction()
    {
        m_bIsOn = !m_bIsOn;
        EWDebug.Log(gameObject.name + " Debug Object " + (m_bIsOn? "Activated!" : "Deactivated!"));
    }

    public override void DoActionOn()
    {
        m_bIsOn = true;
        EWDebug.Log(gameObject.name + " Debug Object " + "Activated!");
    }

    public override void DoActionOff()
    {
        m_bIsOn = false;
        EWDebug.Log(gameObject.name + " Debug Object " + "Deactivated!");
    }
}
