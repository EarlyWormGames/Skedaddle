using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LorisPickup : AttachableInteract
{
    public Loris loris;

    protected override void DoInteract(Animal caller)
    {
        var poodle = caller as Poodle;
        Attach(poodle);

        poodle.PickupLoris(loris);
    }
}
