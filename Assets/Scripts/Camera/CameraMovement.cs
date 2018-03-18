using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public float LerpSpeed = 10;
    public float LookLerpSpeed = 3;
    public bool LookAtAnimal = true;
    public bool UseSpline = true;

    [Tooltip("Only use this if you want 0 rotation to happen")]
    public bool IgnoreAllRotation = false;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!CameraSplineManager.HasAnySplines)
            return;

        Vector3 target = transform.position + transform.forward;
        if (UseSpline)
        {
            target = CameraSpline.LookAtPoint;
            Vector3 movePoint = CameraSpline.CurrentPoint;
            transform.position = Vector3.Lerp(transform.position, movePoint, Time.deltaTime * LerpSpeed);
        }

        if (LookAtAnimal && Animal.CurrentAnimal != null)
        {
            target = Animal.CurrentAnimal.transform.position;
        }

        if (!IgnoreAllRotation)
        {
            Vector3 dir = target - transform.position;
            Quaternion rot = Quaternion.LookRotation(dir.normalized, Vector3.up);
            transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * LerpSpeed);
        }
    }
}