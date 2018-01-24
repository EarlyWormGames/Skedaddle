using UnityEngine;
using System.Collections;

public class Lever : LeverPlug
{
    public Animator m_aController;
    public bool m_bReverseAnims = false;

    public override void DoAnimation()
    {
        if (m_fSoundTimer >= 0f)
        {
            m_fSoundTimer += Time.deltaTime;
        }

        m_bIsAnimating = true;
        if (!m_bIsPlug)
        {
            m_aController.SetFloat("Position", m_aCurrentAnimal.m_aAnimalAnimator.GetFloat("Root_Curve_Y"));

            if (m_fSoundTimer >= m_fSoundTime)
            {
                m_fSoundTimer = -1f;
                PlaySound(SOUND_EVENT.LEVER_PULL);
            }
        }
        else
        {
            if (m_aController.GetBool("Is_On"))
            {
                m_aController.SetBool("Is_On", false);
            }
            else
            {
                m_aController.SetBool("Is_On", true);
            }
            m_bIsAnimating = false;
        }
    }
}
