using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tightrope : MonoBehaviour {

    private void OnTriggerEnter(Collider other)
    {
        Animal anim = other.transform.parent.GetComponentInParent<Animal>();
        if(anim != null)
        {
            anim.m_bHorizontalRope = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Animal anim = other.transform.parent.GetComponentInParent<Animal>();
        if (anim != null)
        {
            anim.m_bHorizontalRope = false;
        }
    }
}
