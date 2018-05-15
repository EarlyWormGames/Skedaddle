using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputNew;

public abstract class MonoInteracter : MonoBehaviour, IInteractable
{
    public List<InputAction> UsableKeys = new List<InputAction>();
    public Transform InteractPoint;

    protected bool keysRegistered = false;
    private List<string> keyStrings = null;

    /// <summary>
    /// Default simply returns true. Keys are already registered
    /// </summary>
    public bool CheckInfo(InputControl input, Animal caller)
    {
        return CheckInput(input, caller);
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
        DoInteract(caller);
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

    protected virtual bool CheckInput(InputControl input, Animal caller) { return true; }
    protected virtual bool ShouldIgnoreDistance() { return false; }
    protected abstract void DoInteract(Animal caller);
}
