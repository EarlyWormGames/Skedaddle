using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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