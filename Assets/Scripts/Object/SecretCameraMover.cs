using UnityEngine;
using System.Collections;

public class SecretCameraMover : ActionObject
{
    //==================================
    //          Public Vars
    //==================================

    public Camera m_cMain;
    public Vector2 m_newXLimits;
    public Vector2 m_newYLimits;

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

        m_cMain.GetComponent<CameraController>().m_v2XLimits = m_newXLimits;
        m_cMain.GetComponent<CameraController>().m_v2YLimits = m_newYLimits;
    }
    //protected override void AnimalExit(Animal a_animal) { }
    //public override void DoAction() { }
    //public override void DoActionOn() { }
    //public override void DoActionOff() { }
}
