using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// display the eye tracking GUI if it is active.
/// </summary>
public class EyeTrackImage : MonoBehaviour
{
    private Image image;

    // Use this for initialization
    void Start()
    {
        image = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!EWEyeTracking.active)
        {
            image.enabled = false;
        }
        else
        {
            image.enabled = true;
            transform.position = EWEyeTracking.position;
        }
    }
}
