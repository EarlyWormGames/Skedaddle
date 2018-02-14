using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSpline : MonoBehaviour
{
    public BezierSpline MySpline;
    public SplineMovement AnimalSpline;
    public Animal MyAnimal;

    public static Vector3 CurrentPoint;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (MyAnimal == null)
            return;

        if (!MyAnimal.m_bSelected)
            return;

        float distance = -1;
        int firstIndex = 0;
        for (int i = 0; i < AnimalSpline.points.Length; ++i)
        {
            float dist = Vector3.Distance(Animal.CurrentAnimal.transform.position, AnimalSpline.GetPosition(i));

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
            XB = AnimalSpline.GetPosition(firstIndex) - Animal.CurrentAnimal.transform.position;

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
                XB = AnimalSpline.GetPosition(endPoint) - Animal.CurrentAnimal.transform.position;

                float dot = Vector3.Dot(AB.normalized, XB.normalized);

                if (dot <= 0)
                {
                    CurrentPoint = MySpline.GetPoint(0);
                    return;
                }
            }
            else
            {
                startPoint = AnimalSpline.points[firstIndex - 1];
                endPoint = AnimalSpline.points[firstIndex];

                AB = AnimalSpline.GetPosition(endPoint) - AnimalSpline.GetPosition(startPoint);
                XB = AnimalSpline.GetPosition(endPoint) - Animal.CurrentAnimal.transform.position;

                float dot = Vector3.Dot(AB.normalized, XB.normalized);

                if (dot <= 0)
                {
                    CurrentPoint = MySpline.GetPoint(1);
                    return;
                }
            }
        }

        float firstDist = Vector3.Distance(Animal.CurrentAnimal.transform.position, AnimalSpline.GetPosition(startPoint));
        float secondDist = Vector3.Distance(Animal.CurrentAnimal.transform.position, AnimalSpline.GetPosition(endPoint));

        AB = AnimalSpline.GetPosition(endPoint) - AnimalSpline.GetPosition(startPoint);
        XB = AnimalSpline.GetPosition(endPoint) - Animal.CurrentAnimal.transform.position;

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
        CurrentPoint = MySpline.GetPoint(splineT);
    }
}