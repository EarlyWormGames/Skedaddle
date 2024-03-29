﻿using UnityEngine;
using System;

/// <summary>
///  This class exsists purley for any small extras that animations might need to call.
///  create extra methods as needed.
/// </summary>

public class AnimationExtras : MonoBehaviour
{
    public void LeftFoot()
    {

    }

    public void RightFoot()
    {

    }

    public void TurnEnd()
    {
        GetComponentInParent<Animal>().FinishTurning();
    }

    public void IKPoodleStepCorection()
    {
        GetComponentInParent<Poodle>();
    }

    public void StartDig()
    {
        var dig = (Dig)GetComponentInParent<Animal>().currentAttached;
        dig.StartSpline();
    }

    public void FinishDig()
    {
        var dig = (Dig)GetComponentInParent<Animal>().currentAttached;
        dig.Finish();
    }

    public void BuildBridge()
    {
        var bridge = (BridgeMaker)GetComponentInParent<Animal>().currentAttached;
        if (bridge == null)
            return;

        bridge.BuildBridge();
    }
}
