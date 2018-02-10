using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalSplineSetter : MonoBehaviour
{
    public ANIMAL_NAME RequiredAnimal;
    public SplineMovement Spline;

    private void OnTriggerEnter(Collider other)
    {
        AnimalTrigger trig = other.GetComponent<AnimalTrigger>();
        if (trig == null)
            return;

        if (trig.parent.m_eName != RequiredAnimal)
            return;

        trig.parent.GetComponent<AnimalMovement>().SetSpline(Spline);
    }
}
