using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplineStopper : MonoBehaviour
{
    public void OnTriggerEnter(Collider other)
    {
        Animal trig = other.GetComponentInParent<Animal>();
        if (trig == null)
            return;

        trig.m_aMovement.StopSpline();
    }
}