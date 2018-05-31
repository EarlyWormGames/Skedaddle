using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// interaction with an event obbject
/// </summary>
public class EventInteract : AnimalInteractor
{
    public UnityEvent OnInteract;
    protected override bool HeadTriggerOnly { get; set; }

    protected override void DoInteract(Animal caller)
    {
        OnInteract.Invoke();
    }
}