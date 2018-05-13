using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeverAnimated : Lever
{
    public bool IsPlug;
    public Animator AnimController;

    private int waitFrames = 0;

    protected override void DoInteract(Animal caller)
    {
        if (!IsPlug)
        {
            AttachedAnimal.m_bPullingLeverOn = IsOn;
            AttachedAnimal.m_bPullingLeverOff = !IsOn;

            waitFrames = 15;
        }
    }

    public void LateUpdate()
    {
        if (AttachedAnimal == null)
            return;

        if (waitFrames > 0)
        {
            --waitFrames;
            return;
        }

        if (!IsPlug)
        {
            float curve = AttachedAnimal.m_aAnimalAnimator.GetFloat("Root_Curve_Y");

            if (AttachedAnimal.m_eName != ANIMAL_NAME.LORIS)
                curve = IsOn? 1 : 0;

            AnimController.SetFloat("Position", curve);

            if (IsOn && curve >= 1)
            {
                SwitchOn();
                FinishAnimation();
            }
            else if (!IsOn && curve <= 0)
            {
                SwitchOff();
                FinishAnimation();
            }
        }
        else
        {
            AnimController.SetBool("Is_On", !IsOn);
        }
    }

    public void FinishAnimation()
    {
        if (!IsPlug)
        {
            AttachedAnimal.m_bPullingLeverOn = false;
            AttachedAnimal.m_bPullingLeverOff = false;
        }

        Detach(this);
    }

    protected override void DoSwitchOff()
    {
        if (IsOn)
            return;

        OnSwitchOff.Invoke();
    }

    protected override void DoSwitchOn()
    {
        if (!IsOn)
            return;

        OnSwitchOn.Invoke();
    }
}