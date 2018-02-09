using UnityEngine;
using System.Collections;

public class AnimalTrigger : MonoBehaviour
{
    public Animal parent;

    void Awake()
    {
        parent = GetComponentInParent<Animal>();
    }
}