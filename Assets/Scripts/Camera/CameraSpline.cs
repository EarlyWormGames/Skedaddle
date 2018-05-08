﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BezierSpline))]
public class CameraSpline : DefaultCameraMovement
{
    public SplineMovement AnimalSpline;
    [Tooltip("Should the spline use the Animal's \"Camera Y\" value?")]
    public bool UseAnimalYSettings = true;

    private BezierSpline MySpline;

    protected override void OnStart()
    {
        MySpline = GetComponent<BezierSpline>();
    }

    // Update is called once per frame
    protected override void OnUpdate()
    {
        
    }

    protected override void CalcPosition()
    {
        float distance = -1;
        int firstIndex = 0;
        for (int i = 0; i < AnimalSpline.points.Length; ++i)
        {
            float dist = Vector3.Distance(currentAnimal.transform.position, AnimalSpline.GetPosition(i));

            if (dist < distance || distance < 0)
            {
                firstIndex = i;
                distance = dist;
            }
        }

        SplineMovement.Point startPoint = null, endPoint = null;

        Vector3 AB = Vector3.zero;
        Vector3 XB = Vector3.zero;

        if (firstIndex != 0 && firstIndex < AnimalSpline.points.Length - 1)
        {
            AB = AnimalSpline.GetPosition(firstIndex) - AnimalSpline.GetPosition(firstIndex - 1);
            XB = AnimalSpline.GetPosition(firstIndex) - currentAnimal.transform.position;

            float dot = Vector3.Dot(AB.normalized, XB.normalized);

            if (dot < 0)
            {
                startPoint = AnimalSpline.points[firstIndex];
                endPoint = AnimalSpline.points[firstIndex + 1];
            }
            else
            {
                startPoint = AnimalSpline.points[firstIndex - 1];
                endPoint = AnimalSpline.points[firstIndex];
            }
        }
        else
        {
            if (firstIndex == 0)
            {
                startPoint = AnimalSpline.points[firstIndex];
                endPoint = AnimalSpline.points[firstIndex + 1];

                AB = AnimalSpline.GetPosition(endPoint) - AnimalSpline.GetPosition(startPoint);
                XB = AnimalSpline.GetPosition(endPoint) - currentAnimal.transform.position;

                float dot = Vector3.Dot(AB.normalized, XB.normalized);

                if (dot <= 0)
                {
                    SetCurrentPoint(0);
                    return;
                }
            }
            else
            {
                startPoint = AnimalSpline.points[firstIndex - 1];
                endPoint = AnimalSpline.points[firstIndex];

                AB = AnimalSpline.GetPosition(endPoint) - AnimalSpline.GetPosition(startPoint);
                XB = AnimalSpline.GetPosition(endPoint) - currentAnimal.transform.position;

                float dot = Vector3.Dot(AB.normalized, XB.normalized);

                if (dot <= 0)
                {
                    SetCurrentPoint(1);
                    return;
                }
            }
        }

        float firstDist = Vector3.Distance(currentAnimal.transform.position, AnimalSpline.GetPosition(startPoint));
        float secondDist = Vector3.Distance(currentAnimal.transform.position, AnimalSpline.GetPosition(endPoint));

        AB = AnimalSpline.GetPosition(endPoint) - AnimalSpline.GetPosition(startPoint);
        XB = AnimalSpline.GetPosition(endPoint) - currentAnimal.transform.position;

        float angle = Vector3.Angle(AB, XB);
        angle = Mathf.Cos(angle * Mathf.Deg2Rad);

        float a = angle * XB.magnitude;

        if (a < 0)
            a *= -1;

        float t = 1 - (a / AB.magnitude);
        //float coreDist = Vector3.Distance(AnimalSpline.GetPosition(startPoint.index), AnimalSpline.GetPosition(endPoint.index));
        //float ab = firstDist + secondDist;
        //float mult = (coreDist / ab);
        //t = t / mult;

        float splineT = Mathf.Lerp(startPoint.time, endPoint.time, t);
        SetCurrentPoint(splineT);
    }

    public override void SetCurrentPoint(object data)
    {
        if (data.GetType() != typeof(float))
            return;

        float time = (float)data;

        CurrentPoint = MySpline.GetPoint(time);
        LookAtPoint = AnimalSpline.m_Spline.GetPoint(time);

        if (UseAnimalYSettings)
        {
            CurrentPoint.y += Animal.CurrentAnimal.m_fCameraY;
            LookAtPoint.y += Animal.CurrentAnimal.m_fCameraY;
        }
    }
}