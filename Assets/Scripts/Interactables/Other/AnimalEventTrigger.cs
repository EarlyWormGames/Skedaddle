using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Events triggered by the animal
/// </summary>
public class AnimalEventTrigger : AnimalTrigger
{
    public bool HeadOnly = false;
    protected override bool HeadTriggerOnly { get { return HeadOnly; } set { HeadOnly = value; } }


    /// <summary>
    /// nested class for animal events 
    /// </summary>
    [System.Serializable]
    public class AnimalEvent : UnityEvent<Animal> { }

    public AnimalEvent OnAnimalEnter, OnAnimalExit;

    public override void AnimalEnter(Animal animal)
    {
        OnAnimalEnter.Invoke(animal);
    }

    public override void AnimalExit(Animal animal)
    {
        OnAnimalExit.Invoke(animal);
    }
}