using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputNew;

public abstract class AttachableInteract : Attachable, IInteractable
{
    [Header("Interaction")]
    public List<InputAction> UsableKeys = new List<InputAction>();
    public Transform InteractPoint;

    protected bool keysRegistered = false;
    private List<string> keyStrings = null;

    /// <summary>
    /// Default simply checks if the <paramref name="caller"/> works with <see cref="AnimalTrigger.AllowsAnimal(Animal)"/>
    /// </summary>
    public bool CheckInfo(InputControl input, Animal caller)
    {
        return CheckInput(input, caller);
    }

    /// <summary>
    /// Default simply checks if the <paramref name="caller"/> works with <see cref="AnimalTrigger.AllowsAnimal(Animal)"/>
    /// </summary>
    protected virtual bool CheckInput(InputControl input, Animal caller)
    {
        if (!caller.CanAttach() && !CanDetach)
            return false;

        return AllowsAnimal(caller) && AnimalsIn.Contains(caller);
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
            return Vector3.Distance(point, transform.position);

        return Vector3.Distance(point, InteractPoint.position);
    }

    public bool IgnoreDistance()
    {
        return ShouldIgnoreDistance();
    }

    public void Interact(Animal caller)
    {
        if (AttachedAnimal != null && caller != AttachedAnimal)
            return;

        DoInteract(caller);
    }

    /// <summary>
    /// Register the <see cref="UsableAxes"/> and <see cref="UsableKeys"/> to the <see cref="InteractChecker"/>
    /// </summary>
    protected void RegisterKeys()
    {
        if (keysRegistered)
            return;

        if (keyStrings == null)
            KeysToString();

        InteractChecker.RegisterKeyListener(this, keyStrings);
        keysRegistered = true;
    }

    /// <summary>
    /// Unregister the <see cref="UsableAxes"/> and <see cref="UsableKeys"/> from the <see cref="InteractChecker"/>
    /// </summary>
    protected void UnregisterKeys()
    {
        if (!keysRegistered)
            return;

        InteractChecker.UnregisterKeyListener(this, keyStrings);
        keysRegistered = false;
    }

    /// <summary>
    /// Loads <see cref="UsableKeys"/> into <see cref="keyStrings"/>
    /// </summary>
    protected void KeysToString()
    {
        List<string> l = new List<string>();
        foreach (var item in UsableKeys)
            l.Add(item.name);
        keyStrings = l;
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

    protected virtual bool ShouldIgnoreDistance() { return false; }
    protected abstract void DoInteract(Animal caller);
}
