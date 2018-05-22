using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// pretty self explanatory 
/// </summary>
public class DisableOnStart : MonoBehaviour
{
    // Use this for initialization
    void Start()
    {
        gameObject.SetActive(false);
    }
}