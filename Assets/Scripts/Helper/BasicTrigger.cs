using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Custon Trigger event caller
/// Use:
/// attach this triggerscript to any triggerable object and call whichever function you want
/// </summary>
public class BasicTrigger : MonoBehaviour
{
    [System.Serializable]
    public class ColliderEvent : UnityEvent<Collider> { }

    public bool AllowTriggers = false;
    public ColliderEvent TriggerEnter, TriggerExit;

    private void OnTriggerEnter(Collider other)
    {
        if (!AllowTriggers && other.isTrigger)
            return;

        TriggerEnter.Invoke(other);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!AllowTriggers && other.isTrigger)
            return;

        TriggerExit.Invoke(other);
    }
}