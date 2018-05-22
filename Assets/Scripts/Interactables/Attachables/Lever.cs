using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Lever : AttachableInteract
{
    public FACING_DIR Direction;
    public bool OnlyOnce;
    public UnityEvent OnSwitchOn, OnSwitchOff;
    protected override bool HeadTriggerOnly { get; set; }

    protected bool IsOn;
    protected bool triggered;

    protected override void OnStart()
    {
        base.OnStart();

        CanDetach = false;
        BlocksMovement = true;
        BlocksTurn = true;
    }

    protected override bool CheckDetach()
    {
        return false;
    }

    protected override void DoInteract(Animal caller)
    {
        if (AttachedAnimal != null)
            return;

        if (triggered && OnlyOnce)
            return;

        if (!TryDetachOther())
            return;

        Attach(caller);
        triggered = true;

        AttachedAnimal.SetDirection(Direction, false);

        IsOn = !IsOn;
    }

    protected override void OnUpdate()
    {
        if (AttachedAnimal == null)
            return;
        
        DoAnimation();
    }

    void DoAnimation()
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
        Detach(this);
    }

    protected virtual void DoSwitchOn()
    {
        OnSwitchOn.Invoke();
        Detach(this);
    }

    protected override void OnDetach(Animal anim)
    {
        AttachedAnimal.SetDirection(FACING_DIR.NONE, false);
    }
}