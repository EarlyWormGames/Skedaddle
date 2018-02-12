using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeverAnimated : Lever
{
    public bool IsPlug;
    public Animator AnimController;

    private int waitFrames = 0;

    public override void DoAction()
    {
        base.DoAction();
        if (!IsPlug)
        {
            m_aCurrentAnimal.m_bPullingLeverOn = IsOn;
            m_aCurrentAnimal.m_bPullingLeverOff = !IsOn;

            if (!IsOn)
                waitFrames = 15;
        }
    }

    public void LateUpdate()
    {
        if (m_aCurrentAnimal == null)
            return;

        if (waitFrames > 0)
        {
            --waitFrames;
            return;
        }

            if (!IsPlug)
        {
            float curve = m_aCurrentAnimal.m_aAnimalAnimator.GetFloat("Root_Curve_Y");
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
            m_aCurrentAnimal.m_bPullingLeverOn = false;
            m_aCurrentAnimal.m_bPullingLeverOff = false;
        }

        Detach();
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