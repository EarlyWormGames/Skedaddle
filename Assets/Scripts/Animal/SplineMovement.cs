using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputNew;

[RequireComponent(typeof(BezierSpline))]
public class SplineMovement : MonoBehaviour
{
    [System.Serializable]
    public class Point
    {
        public Vector3 current;
        public float totalDistance;
        public float time;
        public int index;

        public float SetPoint(Vector3 current, Vector3 next, float distanceTraveled, float time, int index)
        {
            this.current = next;
            float distance = (next - current).magnitude;
            this.totalDistance = distanceTraveled + distance;
            this.time = time;
            this.index = index;

            return distanceTraveled + distance;
        }
    }

    public int NumPoints = 30;
    [Tooltip("Will regenerate keypoints if set to true")]
    public bool RegeneratePoints = false;
    public bool ConstantRegenerate = false;
    public bool HighExit = true;
    public bool LowExit = true;
    public AxisAction MoveAxisKey;
    public bool InvertAxis = false;
    public bool ForceMovement = false;

    [EnumFlag] public IgnoreAxis AxesToIgnore = IgnoreAxis.Y;
    public bool DisableGravity;
    public bool DisableGrounder;

    [HideInInspector]
    public Point[] points;

    [HideInInspector]
    public BezierSpline m_Spline;

    public void Start()
    {
        m_Spline = GetComponent<BezierSpline>();

        if (MoveAxisKey.action != null)
            MoveAxisKey.Bind(GameManager.Instance.input.handle);

        GeneratePoints();
    }

    private void Update()
    {
        if (RegeneratePoints || ConstantRegenerate)
        {
            GeneratePoints();
            RegeneratePoints = false;
        }
    }

    /// <summary>
    /// create the list of points for the slpline
    /// </summary>
    public void GeneratePoints()
    {
        if (m_Spline == null)
            return;

        points = new Point[NumPoints + 1];
        Vector3 last = m_Spline.GetPoint(0);
        last = m_Spline.transform.InverseTransformPoint(last);

        float MaxLength = 0;
        for (int i = 0; i <= NumPoints; i++)
        {
            Vector3 current = m_Spline.GetPoint(i / (float)NumPoints);
            current = m_Spline.transform.InverseTransformPoint(current);
            points[i] = new Point();
            MaxLength = points[i].SetPoint(last, current, MaxLength, i / (float)NumPoints, i);
            last = current;
        }

        RegeneratePoints = false;
    }

    /// <summary>
    /// return the closets point to a given position
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    public int GetClosestPoint(Vector3 position)
    {
        float distance = -1;
        int index = 0;

        for (int i = 0; i < points.Length; ++i)
        {
            var p = IgnoreUtils.Calculate(AxesToIgnore, transform.position, GetPosition(i));
            float d = Vector3.Distance(position, p);
            if (d < distance || distance < 0)
            {
                index = i;
                distance = d;
            }
        }
        return index;
    }

    /// <summary>
    /// returns a point in the spline at a given index
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public Vector3 GetPosition(int index)
    {
        if (index >= points.Length)
            return m_Spline.transform.position;

        return m_Spline.transform.TransformPoint(points[index].current);
    }
    
    /// <summary>
    /// returns a specific point in the spline
    /// </summary>
    /// <param name="point"></param>
    /// <returns></returns>
    public Vector3 GetPosition(Point point)
    {
        return GetPosition(point.index);
    }

    /// <summary>
    /// Draw the spline created
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        if (m_Spline == null)
        {
            m_Spline = GetComponent<BezierSpline>();
        }

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
