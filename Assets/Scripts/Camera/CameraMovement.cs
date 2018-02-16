using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public float LerpSpeed = 10;
    public float LookLerpSpeed = 3;
    public bool LookAtAnimal = true;
    public bool UseSpline = true;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (UseSpline)
        {
            Vector3 movePoint = CameraSpline.CurrentPoint;
            movePoint.y += Animal.CurrentAnimal.m_fCameraY;
            transform.position = Vector3.Lerp(transform.position, movePoint, Time.deltaTime * LerpSpeed);
        }

        if (LookAtAnimal && Animal.CurrentAnimal != null)
        {
            Vector3 dir = Animal.CurrentAnimal.m_tCameraPivot.position - transform.position;
            Quaternion rot = Quaternion.LookRotation(dir.normalized, Vector3.up);
            transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * LerpSpeed);
        }
    }
}