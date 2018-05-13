using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplineStopper : MonoBehaviour
{
    /// <summary>
    /// stops the animal from following the spline
    /// </summary>
    /// <param name="other"></param>
    public void OnTriggerEnter(Collider other)
    {
        Animal trig = other.GetComponentInParent<Animal>();
        if (trig == null)
            return;

        trig.m_aMovement.StopSpline();
    }
}