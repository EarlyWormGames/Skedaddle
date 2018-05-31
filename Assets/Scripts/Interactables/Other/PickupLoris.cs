using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// animal interactions
/// Poodle picking up Laurie
/// </summary>
public class PickupLoris : AnimalInteractor
{
    public Loris loris;
    protected override bool HeadTriggerOnly { get { return true; } set { } }

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
