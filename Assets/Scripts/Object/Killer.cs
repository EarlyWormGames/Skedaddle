using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Killer : MonoBehaviour
{
    [System.Serializable]
    public class KillEvent : UnityEvent<Animal, DEATH_TYPE> { };
    
    public DEATH_TYPE KillType;

    public KillEvent OnKill;

    void OnTriggerEnter(Collider other)
    {
        var animal = other.attachedRigidbody.GetComponent<Animal>();

        if (animal == null)
            return;

        if (!animal.Alive)
            return;

        animal.Kill(KillType);
        OnKill.Invoke(animal, KillType);
    }
}