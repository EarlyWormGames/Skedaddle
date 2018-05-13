using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventInteract : AnimalInteractor
{
    public UnityEvent OnInteract;

    protected override void DoInteract(Animal caller)
    {
        OnInteract.Invoke();
    }
}