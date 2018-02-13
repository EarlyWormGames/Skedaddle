using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputNew;

public class SplineMovement : MonoBehaviour
{
    [System.Serializable]
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
    [Tooltip("Will regenerate keypoints if set to true")]
    public bool RegeneratePoints = false;
    public bool HighExit = true;
    public bool LowExit = true;
    public AxisAction MoveAxisKey;
    public bool InvertAxis = false;

    public Vector3 IgnoreAxis = new Vector3(0, 1, 0);
    public bool DisableGravity;
    
    [HideInInspector]
    public Point[] points;

    public void Start()
    {
        if (MoveAxisKey.action != null)
            MoveAxisKey.Bind(GameManager.Instance.GetComponent<PlayerInput>().handle);

        GeneratePoints();
    }

    public void GeneratePoints()
    {
        if (m_Spline == null)
            return;

        points = new Point[NumPoints + 1];
        Vector3 last = m_Spline.GetPoint(0);
        last = m_Spline.transform.InverseTransformPoint(last);

        float MaxLength = 0;
        for (int t = 0; t <= NumPoints; t++)
        {
            Vector3 current = m_Spline.GetPoint(t / (float)NumPoints);
            current = m_Spline.transform.InverseTransformPoint(current);
            points[t] = new Point();
            MaxLength = points[t].SetPoint(last, current, MaxLength);
            last = current;
        }

        RegeneratePoints = false;
    }

    public int GetClosestPoint(Vector3 position)
    {
        float distance = -1;
        int index = 0;
        for (int i = 0; i < points.Length; ++i)
        {
            float d = Vector3.Distance(position, GetPosition(i));
            if (d < distance || distance < 0)
            {
                index = i;
                distance = d;
            }
        }
        return index;
    }

    public Vector3 GetPosition(int index)
    {
        if (index >= points.Length)
            return m_Spline.transform.position;

        return m_Spline.transform.TransformPoint(points[index].current);
    }

    private void OnDrawGizmosSelected()
    {
        if (m_Spline == null)
            return;

        if (RegeneratePoints)
        {
            GeneratePoints();
        }

        if (points == null)
            return;

        Gizmos.color = Color.blue;
        foreach (var item in points)
        {
            Gizmos.DrawIcon(m_Spline.transform.TransformPoint(item.current), "Blue.png", true);
        }
    }
}
