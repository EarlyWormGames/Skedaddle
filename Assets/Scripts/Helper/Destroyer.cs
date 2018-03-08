using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroyer : MonoBehaviour
{
    public void Destroy(GameObject item)
    {
        Object.Destroy(item);
    }

    public void Destroy(FixedJoint item)
    {
        Object.Destroy(item);
    }
}