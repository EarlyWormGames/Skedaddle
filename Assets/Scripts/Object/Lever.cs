using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lever : ActionObject
{
    public bool AllowBackwards;
    public bool IsOn;
    public Animator AnimController;

    public override void DoAction()
    {
        if (Animal.CurrentAnimal.m_oCurrentObject != null && Animal.CurrentAnimal.m_oCurrentObject != this)
            return;

        m_aCurrentAnimal = Animal.CurrentAnimal;
        m_aCurrentAnimal.m_oCurrentObject = this;

        IsOn = !IsOn;

    }

    public void SwitchOn()
    {

    }

    public void SwitchOff()
    {

    }
}