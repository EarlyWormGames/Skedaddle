using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplineMovement : MonoBehaviour
{
    public class Point
    {
        public Vector3 current;
        public float totalDistance;

        public float SetPoint(Vector3 current, Vector3 next, float distanceTraveled)
        {
            this.current = next;
            float distance = (next - current).magnitude;
            this.totalDistance = distanceTraveled + distance;
            return distanceTraveled + distance;
        }
    }

    public BezierSpline m_Spline;

    public int NumPoints = 100;
    public bool HighExit = true;
    public bool LowExit = true;

    public Vector3 IgnoreAxis = new Vector3(0, 1, 0);
    public bool DisableGravity;

    public float MaxLength;
    public Point[] points;

    public void Start()
    {
        points = new Point[NumPoints + 1];
        Vector3 last = m_Spline.GetPoint(0);
        MaxLength = 0;
        for (int t = 0; t <= NumPoints; t++)
        {
            Vector3 current = m_Spline.GetPoint(t / (float)NumPoints);
            points[t] = new Point();
            MaxLength = points[t].SetPoint(last, current, MaxLength);
            last = current;
        }
    }

    public int GetClosestPoint(Vector3 position)
    {
        float distance = -1;
        int index = 0;
        for (int i = 0; i < points.Length; ++i)
        {
            float d = Vector3.Distance(position, points[i].current);
            if (d < distance || distance < 0)
            {
                index = i;
                distance = d;
            }
        }
        return index;
    }
}
