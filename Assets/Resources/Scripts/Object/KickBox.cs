using UnityEngine;
using System.Collections;

public class KickBox : ActionObject
{
    //==================================
    //          Public Vars
    //==================================
    public KickObject[] m_akConnections;

    //==================================
    //          Internal Vars
    //==================================

    //==================================
    //          Private Vars
    //================================== 

    //Inherited functions

    protected override void OnStart()
    {
        m_aRequiredAnimal = ANIMAL_NAME.ZEBRA;
    }
    //protected override void OnUpdate() { }
    //protected override void AnimalEnter(Animal a_animal) { }
    //protected override void AnimalExit(Animal a_animal) { }
    public override void DoAction()
    {
        float towards = (m_aCurrentAnimal.transform.position - transform.position).normalized.x;
        float anim = m_aCurrentAnimal.transform.forward.x;

        if ((towards > 0 && anim > 0) || (towards < 0 && anim < 0))
        {
            //Tell zebra to animate kicking
        }
        else
        {
            //Tell zebra to animate turning around and kicking
        }

        for (int i = 0; i < m_akConnections.Length; ++i)
        {
            m_akConnections[i].Kick();
        }
    }
    //public override void DoActionOn() { }
    //public override void DoActionOff() { }
}
