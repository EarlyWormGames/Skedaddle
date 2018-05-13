using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputNew;

public abstract class AttachableInteract : Attachable, IInteractable
{
    public List<ButtonAction> UsableKeys = new List<ButtonAction>();
    public List<AxisAction> UsableAxes = new List<AxisAction>();
    public Transform InteractPoint;

    public bool CheckInfo(ActionSlot input, Animal caller)
    {
        return CheckInput(input, caller);
    }

    protected virtual bool CheckInput(ActionSlot input, Animal caller)
    {
        if (AllowsAnimal(caller))
            return true;

        return false;
    }

    public float GetDistance(Vector3 point)
    {
        return CheckDistance(point);
    }

    protected virtual float CheckDistance(Vector3 point)
    {
        if (InteractPoint == null)
            return 0;

        return Vector3.Distance(point, InteractPoint.position);
    }

    public bool IgnoreDistance()
    {
        return ShouldIgnoreDistance();
    }

    public void Interact(Animal caller)
    {
        if (AttachedAnimal != null)
            return;

        DoInteract(caller);
    }

    protected void RegisterKeys()
    {
        InteractChecker.RegisterKeyListener(this, UsableKeys);
        InteractChecker.RegisterKeyListener(this, UsableAxes);
    }

    protected void UnregisterKeys()
    {
        InteractChecker.UnregisterKeyListener(this, UsableKeys);
        InteractChecker.UnregisterKeyListener(this, UsableAxes);
    }

    protected override void OnAttach()
    {
        base.OnAttach();
        UnregisterKeys();
    }

    protected override void OnAnimalEnter(Animal animal)
    {
        base.OnAnimalEnter(animal);
        RegisterKeys();
    }

    protected override void OnAnimalExit(Animal animal)
    {
        base.OnAnimalExit(animal);
        UnregisterKeys();
    }

    protected virtual bool ShouldIgnoreDistance() { return false; }
    protected abstract void DoInteract(Animal caller);
}
