using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Lever : ActionObject
{
    public FACING_DIR Direction;
    public bool OnlyOnce;
    public UnityEvent OnSwitchOn, OnSwitchOff;

    protected bool IsOn;
    protected bool triggered;

    protected override void OnStart()
    {
        base.OnStart();

        m_CanBeDetached = false;
        m_CanDetach = false;
        m_bBlocksMovement = true;
        m_bBlocksTurn = true;
    }

    public override void DoAction()
    {
        if (!TryDetach())
            return;

        if (m_aCurrentAnimal != null)
            return;

        if (triggered && OnlyOnce)
            return;
        base.DoAction();
        triggered = true;

        m_aCurrentAnimal = Animal.CurrentAnimal;
        m_aCurrentAnimal.m_oCurrentObject = this;

        m_aCurrentAnimal.SetDirection(Direction, false);

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
        Detach(m_aCurrentAnimal);
    }

    protected virtual void DoSwitchOn()
    {
        OnSwitchOn.Invoke();
        Detach(m_aCurrentAnimal);
    }

    public override void Detach(Animal anim, bool destroy = false)
    {
        base.Detach(anim, destroy);
        m_aCurrentAnimal.SetDirection(FACING_DIR.NONE, false);
        m_aCurrentAnimal.m_oCurrentObject = null;
        m_aCurrentAnimal = null;
    }
}