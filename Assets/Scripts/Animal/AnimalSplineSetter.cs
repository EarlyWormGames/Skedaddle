﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputNew;

public class AnimalSplineSetter : MonoBehaviour
{
    public ANIMAL_NAME RequiredAnimal;
    public SplineMovement Spline;
    public ButtonAction RequiredKey;
    public bool StopOnExit;

    private Animal animalIn;

    private void OnTriggerEnter(Collider other)
    {
        Animal trig = other.GetComponentInParent<Animal>();
        if (trig == null)
            return;

        if (trig.m_eName != RequiredAnimal && RequiredAnimal != ANIMAL_NAME.NONE)
            return;

        if (trig.m_aMovement.FollowSpline == this)
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

        if (StopOnExit)
            trig.m_aMovement.StopSpline();

        if (trig != animalIn)
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
            if (animalIn.m_aMovement.FollowSpline != Spline)
            {
                if (RequiredKey.control.isHeld)
                {
                    animalIn.GetComponent<AnimalMovement>().SetSpline(Spline);
                }
            }
        }
    }
}
