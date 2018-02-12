using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeverAnimated : Lever
{
    public bool IsPlug;
    public Animator AnimController;

    public override void DoAnimation()
    {
        if (!IsPlug)
        {
            AnimController.SetFloat("Position", m_aCurrentAnimal.m_aAnimalAnimator.GetFloat("Root_Curve_Y"));
        }
        else
        {
            AnimController.SetBool("Is_On", !IsOn);
        }
    }

    public void FinishAnimation()
    {
        Detach();
    }

    protected override void DoSwitchOff()
    {
        OnSwitchOff.Invoke();
    }

    protected override void DoSwitchOn()
    {
        OnSwitchOn.Invoke();
    }
}