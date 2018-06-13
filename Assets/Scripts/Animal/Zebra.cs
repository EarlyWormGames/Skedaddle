using UnityEngine;
using System.Collections;

public class Zebra : Animal
{
    protected override void OnAwake()
    {
        m_eName = ANIMAL_NAME.ZEBRA;
        m_eSize = ANIMAL_SIZE.L;
    }

    protected override void OnUpdate()
    {
        if (m_bSelected)
        {
            if(m_bWalkingLeft || m_bWalkingRight)
            {
                m_aAnimalAnimator.SetBool("Walking", true);
            } else
            {
                m_aAnimalAnimator.SetBool("Walking", false);
            }
        }
    }
    public override void OnSelectChange()
    {
        base.OnSelectChange();

        m_aAnimalAnimator.SetBool("Controlled", m_bSelected);
    }
    public override ANIMAL_NAME GetName()
    {
        return ANIMAL_NAME.ZEBRA;
    }
}
