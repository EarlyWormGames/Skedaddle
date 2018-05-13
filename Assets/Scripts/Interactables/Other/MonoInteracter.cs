using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputNew;

public abstract class MonoInteracter : MonoBehaviour, IInteractable
{
    public List<ButtonAction> UsableKeys = new List<ButtonAction>();
    public Transform InteractPoint;

    public bool CheckInfo(ActionSlot input, Animal caller)
    {
        return true;
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
        DoInteract(caller);
    }

    protected virtual bool ShouldIgnoreDistance() { return false; }
    protected abstract void DoInteract(Animal caller);
}
