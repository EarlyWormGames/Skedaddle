using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Treadmill : MonoBehaviour
{
    public Vector3 force;
    public bool Active = true;

    private List<Rigidbody> bodiesIn = new List<Rigidbody>();

    private void OnTriggerEnter(Collider other)
    {
        Rigidbody rig = other.GetComponentInParent<Rigidbody>();

        if (!bodiesIn.Contains(rig))
            bodiesIn.Add(rig);
    }

    private void OnTriggerExit(Collider other)
    {
        Rigidbody rig = other.GetComponentInParent<Rigidbody>();

        if (bodiesIn.Contains(rig))
            bodiesIn.Remove(rig);
    }

    private void Update()
    {
        if (!Active)
            return;

        foreach (var item in bodiesIn)
        {
            item.transform.position += (force * Time.deltaTime);
        }
    }
}