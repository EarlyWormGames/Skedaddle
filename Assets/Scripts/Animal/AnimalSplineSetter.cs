using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalSplineSetter : MonoBehaviour
{
    public ANIMAL_NAME RequiredAnimal;
    public SplineMovement Spline;

    private void OnTriggerEnter(Collider other)
    {
        Animal trig = other.GetComponentInParent<Animal>();
        if (trig == null)
            return;

        if (trig.m_eName != RequiredAnimal)
            return;

        trig.GetComponent<AnimalMovement>().SetSpline(Spline);
    }
}
