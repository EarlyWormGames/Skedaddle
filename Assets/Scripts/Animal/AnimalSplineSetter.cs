using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputNew;

public class AnimalSplineSetter : MonoBehaviour
{
    public ANIMAL_NAME RequiredAnimal;
    public SplineMovement Spline;
    public ButtonAction RequiredKey;

    private Animal animalIn;

    private void OnTriggerEnter(Collider other)
    {
        Animal trig = other.GetComponentInParent<Animal>();
        if (trig == null)
            return;

        if (trig.m_eName != RequiredAnimal)
            return;

        if (RequiredKey.control == null)
            trig.GetComponent<AnimalMovement>().SetSpline(Spline);
        else
            animalIn = trig;
    }

    private void OnTriggerExit(Collider other)
    {
        if (animalIn == null)
            return;

        Animal trig = other.GetComponentInParent<Animal>();
        if (trig == null)
            return;

        if (trig!= animalIn)
            return;

        animalIn = null;
    }

    private void Start()
    {
        if (RequiredKey.action != null)
            RequiredKey.Bind(GameManager.Instance.input.handle);
    }

    private void Update()
    {
        if (animalIn != null)
        {
            if (RequiredKey.control.isHeld)
            {
                animalIn.GetComponent<AnimalMovement>().SetSpline(Spline);
                animalIn = null;
            }
        }
    }
}
