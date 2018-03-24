using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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