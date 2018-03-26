using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class CameraSplineSetter : MonoBehaviour
{
    public CameraSpline Spline;
    public ANIMAL_NAME RequiredAnimal;
    public LayerMask RequiredLayer;

    private void OnTriggerEnter(Collider other)
    {
        if (other.attachedRigidbody == null || other.isTrigger || !RequiredLayer.Contains(other.gameObject.layer))
            return;
        Animal animal = other.attachedRigidbody.GetComponent<Animal>();

        if (animal == null)
            return;

        if (animal.m_eName != RequiredAnimal && RequiredAnimal != ANIMAL_NAME.NONE)
            return;

        CameraSplineManager.instance.EnableSpline(animal.m_eName, Spline);
    }
}
