using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public float LerpSpeed = 10;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Vector3 movePoint = CameraSpline.CurrentPoint;
        movePoint.y += Animal.CurrentAnimal.m_fCameraY;
        transform.position = Vector3.Lerp(transform.position, movePoint, Time.deltaTime * LerpSpeed);
    }
}