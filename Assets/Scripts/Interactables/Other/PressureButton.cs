using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Pressure button management
/// </summary>
public class PressureButton : MonoBehaviour
{
    public bool OnlyOnce = false;
    public UnityEvent OnPressed;
    public UnityEvent OnReleased;
    public UnityEvent OnPressBad;
    public UnityEvent OnReleaseBad;

    private List<Rigidbody> inBodies = new List<Rigidbody>();
    private bool wasPressed;
    private bool hasBeenPressed = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
            return;

        var rig = other.GetComponentInParent<Rigidbody>();

        if (!inBodies.Contains(rig))
            inBodies.Add(rig);
    }

    private void OnTriggerExit(Collider other)
    {
        var rig = other.GetComponentInParent<Rigidbody>();

        if (inBodies.Contains(rig))
            inBodies.Remove(rig);
    }

    private void Update()
    {
        //if there is more than one RigidBody (Animal) on the pressure button, it will stay activated
        if (inBodies.Count > 0 && !wasPressed)
        {
            wasPressed = true;

            if (hasBeenPressed && OnlyOnce)
                OnPressBad.Invoke();
            else
                OnPressed.Invoke();
        }
        else if (inBodies.Count <= 0 && wasPressed)
        {
            wasPressed = false;

            if (hasBeenPressed && OnlyOnce)
                OnReleaseBad.Invoke();
            else
                OnReleased.Invoke();

            hasBeenPressed = true;
        }
    }
}