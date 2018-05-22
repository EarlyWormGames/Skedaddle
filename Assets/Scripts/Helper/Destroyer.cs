using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Destroy an item
/// Used with UnityEvent calls 
/// </summary>
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