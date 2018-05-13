using UnityEngine;
using System.Collections;

public class Elephant : Animal
{
    //==================================
    //          Public Vars
    //==================================
    //public WaterHolder m_wHolder;

    //==================================
    //          Internal Vars
    //==================================

    //==================================
    //          Private Vars
    //================================== 

    //Inherited functions
    protected override void OnStart()
    {
        m_eName = ANIMAL_NAME.ELEPHANT;
        m_eSize = ANIMAL_SIZE.XL;
    }
    //protected override void OnUpdate() { }
    //protected override void OnFixedUpdate() { }
    //protected override void OnDeath(DEATH_TYPE a_type) { }
}
