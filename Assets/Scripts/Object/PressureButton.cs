using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PressureButton : MonoBehaviour
{
    public UnityEvent OnPressed;
    public UnityEvent OnReleased;

    private List<Rigidbody> inBodies = new List<Rigidbody>();
    private bool wasPressed;

    private void OnTriggerEnter(Collider other)
    {
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
        if (inBodies.Count > 0 && !wasPressed)
        {
            wasPressed = true;
            OnPressed.Invoke();
        }
        else if (inBodies.Count <= 0 && wasPressed)
        {
            wasPressed = false;
            OnReleased.Invoke();
        }
    }
}