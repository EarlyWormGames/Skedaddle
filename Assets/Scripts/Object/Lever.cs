using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Lever : ActionObject
{
    public bool OnlyOnce;
    public UnityEvent OnSwitchOn, OnSwitchOff;

    protected bool IsOn;
    protected bool triggered;

    public override void DoAction()
    {
        if (Animal.CurrentAnimal.m_oCurrentObject != null && Animal.CurrentAnimal.m_oCurrentObject != this)
            return;

        if (triggered && OnlyOnce)
            return;
        triggered = true;

        m_aCurrentAnimal = Animal.CurrentAnimal;
        m_aCurrentAnimal.m_oCurrentObject = this;

        IsOn = !IsOn;
    }

    protected override void OnUpdate()
    {
        if (m_aCurrentAnimal == null)
            return;
        
        DoAnimation();
    }

    public override void DoAnimation()
    {
        if (IsOn)
            SwitchOn();
        else
            SwitchOff();
    }

     
    public void SwitchOff() { DoSwitchOff(); }
    public void SwitchOn() { DoSwitchOn(); }

    protected virtual void DoSwitchOff()
    {
        OnSwitchOff.Invoke();
        Detach();
    }

    protected virtual void DoSwitchOn()
    {
        OnSwitchOn.Invoke();
        Detach();
    }

    public override void Detach()
    {
        m_aCurrentAnimal.m_oCurrentObject = null;
        m_aCurrentAnimal = null;
    }
}