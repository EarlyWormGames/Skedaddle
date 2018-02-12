using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeverAnimated : Lever
{
    public bool IsPlug;
    public Animator AnimController;

    private bool m_bIsAnimating;

    public override void DoAnimation()
    {
        m_bIsAnimating = true;
        if (!IsPlug)
        {
            AnimController.SetFloat("Position", m_aCurrentAnimal.m_aAnimalAnimator.GetFloat("Root_Curve_Y"));
        }
        else
        {
            if (AnimController.GetBool("Is_On"))
            {
                AnimController.SetBool("Is_On", false);
            }
            else
            {
                AnimController.SetBool("Is_On", true);
            }
            m_bIsAnimating = false;
        }
    }
}