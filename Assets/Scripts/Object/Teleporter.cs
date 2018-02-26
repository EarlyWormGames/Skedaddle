using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter : MonoBehaviour
{
    private List<Animal> currentAnimals = new List<Animal>();

    public void Teleport(Transform transform)
    {
        foreach (var animal in currentAnimals)
        {
            animal.transform.position = transform.position;
        }
        currentAnimals.Clear();
    }

    private void OnTriggerEnter(Collider other)
    {
        var animal = other.GetComponentInParent<Animal>();
        if (animal != null)
            currentAnimals.Add(animal);
    }

    private void OnTriggerExit(Collider other)
    {
        var animal = other.GetComponentInParent<Animal>();
        if (animal != null)
            currentAnimals.Remove(animal);
    }
}