using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupLoris : AnimalInteractor
{
    public Loris loris;

    protected override void DoInteract(Animal caller)
    {
        var poodle = (Poodle)caller;
        if(poodle.m_bHoldingLoris)
        {
            poodle.DropLoris();
        }
        else
        {
            poodle.PickupLoris(loris);
        }
    }
}
