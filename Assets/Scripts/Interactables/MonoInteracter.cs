using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputNew;

public abstract class MonoInteracter : MonoBehaviour, IInteractable
{
    public List<ButtonAction> UsableKeys = new List<ButtonAction>();
    public Transform InteractPoint;

    /// <summary>
    /// Default simply returns true. Keys are already registered
    /// </summary>
    public bool CheckInfo(ActionSlot input, Animal caller)
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

    protected virtual bool CheckInput(ActionSlot input, Animal caller) { return true; }
    protected virtual bool ShouldIgnoreDistance() { return false; }
    protected abstract void DoInteract(Animal caller);
}
