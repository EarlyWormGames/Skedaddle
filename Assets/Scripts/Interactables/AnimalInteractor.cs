using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputNew;

/// <summary>
/// Base class for listening to keypresses when an <see cref="Animal"/> is nearby
/// </summary>
public abstract class AnimalInteractor : AnimalTrigger, IInteractable
{
    [Header("Interaction")]
    public List<InputAction> UsableKeys = new List<InputAction>();
    public Transform InteractPoint;

    protected bool keysRegistered = false;

    /// <summary>
    /// Default simply checks if the <paramref name="caller"/> works with <see cref="AnimalTrigger.AllowsAnimal(Animal)"/>
    /// </summary>
    public bool CheckInfo(InputControl input, Animal caller)
    {
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
        DoInteract(caller);
    }

    protected virtual bool ShouldIgnoreDistance() { return false; }
    protected abstract void DoInteract(Animal caller);

    public override void AnimalEnter(Animal animal)
    {
        if (keysRegistered)
            return;

        if (AnimalsIn.Count == 1)
        {
            InteractChecker.RegisterKeyListener(this, KeysToString());
            keysRegistered = true;
        }
    }

    public override void AnimalExit(Animal animal)
    {
        if (!keysRegistered)
            return;

        if (AnimalsIn.Count == 0)
        {
            InteractChecker.UnregisterKeyListener(this, KeysToString());
            keysRegistered = false;
        }
    }

    List<string> KeysToString()
    {
        List<string> l = new List<string>();
        foreach (var item in UsableKeys)
            l.Add(item.name);
        return l;
    }
}