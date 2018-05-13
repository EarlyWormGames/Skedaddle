using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputNew;

public abstract class AttachableInteract : Attachable, IInteractable
{
    public List<ButtonAction> UsableKeys = new List<ButtonAction>();
    public List<AxisAction> UsableAxes = new List<AxisAction>();
    public Transform InteractPoint;

    /// <summary>
    /// Default simply checks if the <paramref name="caller"/> works with <see cref="AnimalTrigger.AllowsAnimal(Animal)"/>
    /// </summary>
    public bool CheckInfo(ActionSlot input, Animal caller)
    {
        return CheckInput(input, caller);
    }

    /// <summary>
    /// Default simply checks if the <paramref name="caller"/> works with <see cref="AnimalTrigger.AllowsAnimal(Animal)"/>
    /// </summary>
    protected virtual bool CheckInput(ActionSlot input, Animal caller)
    {
        return AllowsAnimal(caller);
    }

    public float GetDistance(Vector3 point)
    {
        return CheckDistance(point);
    }

    /// <summary>
    /// Default checks the distance between <paramref name="point"/> and <see cref="InteractPoint"/>
    /// </summary>
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

    /// <summary>
    /// Register the <see cref="UsableAxes"/> and <see cref="UsableKeys"/> to the <see cref="InteractChecker"/>
    /// </summary>
    protected void RegisterKeys()
    {
        InteractChecker.RegisterKeyListener(this, UsableKeys);
        InteractChecker.RegisterKeyListener(this, UsableAxes);
    }

    /// <summary>
    /// Unregister the <see cref="UsableAxes"/> and <see cref="UsableKeys"/> from the <see cref="InteractChecker"/>
    /// </summary>
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

    sealed protected override void OnDetach(Animal animal)
    {
        base.OnDetach(animal);
        OnDetaching(animal);
        if (AnimalsIn.Count > 0)
            RegisterKeys();
    }

    protected override void OnAnimalEnter(Animal animal)
    {
        base.OnAnimalEnter(animal);

        if (AnimalsIn.Count == 1)
            RegisterKeys();
    }

    protected override void OnAnimalExit(Animal animal)
    {
        base.OnAnimalExit(animal);

        if (AnimalsIn.Count == 0)
            UnregisterKeys();
    }

    protected virtual void OnDetaching(Animal animal) { }
    protected virtual bool ShouldIgnoreDistance() { return false; }
    protected abstract void DoInteract(Animal caller);
}
