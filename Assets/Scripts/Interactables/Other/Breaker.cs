using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// the object that breaks the "BreakableObject"
/// </summary>
public class Breaker : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        BreakableObject br = other.GetComponent<BreakableObject>();
        if (br == null)
        {
            if (other.attachedRigidbody != null)
            {
                br = other.attachedRigidbody.GetComponent<BreakableObject>();
            }
        }
        if (br == null)
            return;

        br.Break();
    }
}