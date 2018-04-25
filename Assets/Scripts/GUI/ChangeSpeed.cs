using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeSpeed : ActionObject {

    public float SpeedChange;
    private static float InitialSpeed;

    public override void AnimalEnter(Animal a_animal)
    {
        if (InitialSpeed == 0) InitialSpeed = a_animal.m_fTopSpeed;
        a_animal.m_fTopSpeed = SpeedChange;
    }
}
