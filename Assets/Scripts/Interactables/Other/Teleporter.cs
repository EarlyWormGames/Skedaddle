using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// teleport an animal from point A to B
/// </summary>
public class Teleporter : MonoBehaviour
{
    public bool m_StartClosed;
    public Animator m_Anim;
    private List<Animal> currentAnimals = new List<Animal>();
    private bool Closed;

    void Start()
    {
        Closed = m_StartClosed;
        if(m_Anim != null)
        m_Anim.SetBool("Closed", Closed);
    }

    public void Teleport(Transform transform)
    {
        ChangeState();
        foreach (var animal in currentAnimals)
        {
            animal.transform.position = transform.position;
        }
        currentAnimals.Clear();
    }

    public void ChangeState()
    {
        Closed = !Closed;
        if (m_Anim != null)
            m_Anim.SetBool("Closed", Closed);
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