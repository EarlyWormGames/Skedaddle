using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AnimalEventTrigger : AnimalTrigger
{
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