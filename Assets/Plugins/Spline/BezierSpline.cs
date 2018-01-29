using UnityEngine;
using System;
using System.Collections.Generic;

public class BezierSpline : MonoBehaviour
{
    public int ArcIncrements = 100;
    public int ArcSubDivs = 5;

    public bool DrawTangents = false;
    public bool DrawIncrements = false;
    public bool LowExit = true;
    public bool HighExit = true;
    public bool AllowReverse = true;

    [SerializeField]
    private Vector3[] points;

    [SerializeField]
    private BezierControlPointMode[] modes;

    [SerializeField]
    private bool loop;

    private float[] arcLengths;
    private Vector3[] arcPoints;
    private int[] arcSubDivs;

    public float MaxSplineLength
    {
        get
        {
            return arcLengths[arcLengths.Length - 1];
        }
    }

    public bool Loop
    {
        get
        {
            return loop;
        }
        set
        {
            loop = value;
            if (value == true)
            {
                modes[modes.Length - 1] = modes[0];
                SetControlPoint(0, points[0]);
            }
        }
    }

    private void Awake()
    {
        ArcLength();
        //for(int i = 0; i < CurveCount; ++i)
        //{
        //    GetLength(i);
        //}
    }

    public void Reset()
    {
        points = new Vector3[] {
            new Vector3(1f, 0f, 0f),
            new Vector3(2f, 0f, 0f),
            new Vector3(3f, 0f, 0f),
            new Vector3(4f, 0f, 0f)
        };
        modes = new BezierControlPointMode[] {
            BezierControlPointMode.Free,
            BezierControlPointMode.Free
        };
    }

    public Vector3 GetPoint(float t)
    {
        int i;
        if (t >= 1f)
        {
            t = 1f;
            i = points.Length - 4;
        }
        else
        {
            t = Mathf.Clamp01(t) * CurveCount;
            i = (int)t;
            t -= i;
            i *= 3;
        }
        return transform.TransformPoint(Bezier.GetPoint(
            points[i], points[i + 1], points[i + 2], points[i + 3], t));
    }

    public Vector3 GetVelocity(float t)
    {
        int i;
        if (t >= 1f)
        {
            t = 1f;
            i = points.Length - 4;
        }
        else
        {
            t = Mathf.Clamp01(t) * CurveCount;
            i = (int)t;
            t -= i;
            i *= 3;
        }
        return transform.TransformPoint(Bezier.GetFirstDerivative(
            points[i], points[i + 1], points[i + 2], points[i + 3], t)) - transform.position;
    }

    public Vector3 GetDirection(float t)
    {
        return (transform.TransformPoint(Bezier.GetFirstDerivative(points[0], points[1], points[2], t)) -
            transform.position).normalized;
    }

    public void ArcLength()
    {
        float maxLength = 0;

        float mult = 1 / (float)ArcIncrements;
        arcLengths = new float[ArcIncrements + 1];
        arcPoints = new Vector3[ArcIncrements + 1];
        arcSubDivs = new int[ArcSubDivs];

        Vector3 pp = GetPoint(0); //PreviousPoint
        arcPoints[0] = pp;
        for(int i = 1; i <= ArcIncrements; ++i)
        {
            Vector3 point = GetPoint(i * mult);
            maxLength += Vector3.Distance(pp, point);
            arcLengths[i] = maxLength;

            arcPoints[i] = point;
            pp = point;
        }

        float size = ArcIncrements / (float)(ArcSubDivs + 1);
        int index = 0;
        for(int i = 0; i < arcLengths.Length; ++i)
        {
            if(i >= size * (index + 1))
            {
                arcSubDivs[index] = i;

                ++index;

                if (index == arcSubDivs.Length)
                    break;
            }
        }
    }

    public float GetArcDist(float dist, bool subDivCheck = true)
    {
        dist -= 0.00001f;
        if (arcLengths == null)
            ArcLength();

        int start = 0;

        if (subDivCheck)
        {
            for (int i = 0; i < arcSubDivs.Length; ++i)
            {
                if (dist > arcLengths[arcSubDivs[i]])
                {
                    if (i == arcSubDivs.Length - 1)
                    {
                        start = arcSubDivs[i];
                    }
                    else if (dist < arcLengths[arcSubDivs[i + 1]])
                    {
                        start = arcSubDivs[i];
                        break;
                    }
                }
            }
        }

        for(int i = start; i < arcLengths.Length; ++i)
        {
            if(arcLengths[i] > dist)
            {
                return i / (float)(arcLengths.Length - 1);
            }
        }
        return 0;
    }

    public float GetArcLength(Vector3 point, bool a_ignoreY = false)
    {
        if (arcPoints == null)
            ArcLength();

        int closest = 0;
        float distance = -1;
        for (int i = 0; i < arcPoints.Length; ++i)
        {
            if (a_ignoreY)
                point.y = arcPoints[i].y;

            float dist = Vector3.Distance(arcPoints[i], point);
            if (dist < distance || distance < 0)
            {
                distance = dist;
                closest = i;
            }
        }
        return arcLengths[closest];
    }

    public void AddCurve()
    {
        Vector3 point = points[points.Length - 1];
        Array.Resize(ref points, points.Length + 3);
        point.x += 1f;
        points[points.Length - 3] = point;
        point.x += 1f;
        points[points.Length - 2] = point;
        point.x += 1f;
        points[points.Length - 1] = point;

        Array.Resize(ref modes, modes.Length + 1);
        modes[modes.Length - 1] = modes[modes.Length - 2];
        EnforceMode(points.Length - 4);

        if (loop)
        {
            points[points.Length - 1] = points[0];
            modes[modes.Length - 1] = modes[0];
            EnforceMode(0);
        }
    }

    public void RemoveCurve()
    {
        if (points.Length > 4)
        {                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                            
            Array.Resize(ref points, points.Length - 3);
        }
    }

    public int CurveCount
    {
        get
        {
            return (points.Length - 1) / 3;
        }
    }

    public int ControlPointCount
    {
        get
        {
            return points.Length;
        }
    }

    public Vector3 GetControlPoint(int index)
    {
        return points[index];
    }

    public void SetControlPoint(int index, Vector3 point)
    {
        if (index % 3 == 0)
        {
            Vector3 delta = point - points[index];
            if (loop)
            {
                if (index == 0)
                {
                    points[1] += delta;
                    points[points.Length - 2] += delta;
                    points[points.Length - 1] = point;
                }
                else if (index == points.Length - 1)
                {
                    points[0] = point;
                    points[1] += delta;
                    points[index - 1] += delta;
                }
                else
                {
                    points[index - 1] += delta;
                    points[index + 1] += delta;
                }
            }
            else
            {
                if (index > 0)
                {
                    points[index - 1] += delta;
                }
                if (index + 1 < points.Length)
                {
                    points[index + 1] += delta;
                }
            }
        }
        points[index] = point;
        EnforceMode(index);
    }

    public BezierControlPointMode GetControlPointMode(int index)
    {
        return modes[(index + 1) / 3];
    }

    public void SetControlPointMode(int index, BezierControlPointMode mode)
    {
        int modeIndex = (index + 1) / 3;
        modes[modeIndex] = mode;
        if (loop)
        {
            if (modeIndex == 0)
            {
                modes[modes.Length - 1] = mode;
            }
            else if (modeIndex == modes.Length - 1)
            {
                modes[0] = mode;
            }
        }
        EnforceMode(index);
    }

    private void EnforceMode(int index)
    {
        int modeIndex = (index + 1) / 3;
        BezierControlPointMode mode = modes[modeIndex];
        if (mode == BezierControlPointMode.Free || !loop && (modeIndex == 0 || modeIndex == modes.Length - 1))
        {
            return;
        }

        int middleIndex = modeIndex * 3;
        int fixedIndex, enforcedIndex;
        if (index <= middleIndex)
        {
            fixedIndex = middleIndex - 1;
            if (fixedIndex < 0)
            {
                fixedIndex = points.Length - 2;
            }
            enforcedIndex = middleIndex + 1;
            if (enforcedIndex >= points.Length)
            {
                enforcedIndex = 1;
            }
        }
        else
        {
            fixedIndex = middleIndex + 1;
            if (fixedIndex >= points.Length)
            {
                fixedIndex = 1;
            }
            enforcedIndex = middleIndex - 1;
            if (enforcedIndex < 0)
            {
                enforcedIndex = points.Length - 2;
            }
        }

        Vector3 middle = points[middleIndex];
        Vector3 enforcedTangent = middle - points[fixedIndex];
        if (mode == BezierControlPointMode.Aligned)
        {
            enforcedTangent = enforcedTangent.normalized * Vector3.Distance(middle, points[enforcedIndex]);
        }
        points[enforcedIndex] = middle + enforcedTangent;
    }

    public void SetControlPointWorld(int index, Vector3 point)
    {
        SetControlPoint(index, transform.InverseTransformPoint(point));
    }

    public Vector3 GetControlPointWorld(int index)
    {
        return transform.TransformPoint(GetControlPoint(index));
    }

    public void Reverse()
    {
        Vector3[] newpoints = new Vector3[points.Length];
        int x = 0;
        for (int i = points.Length - 1; i >= 0; --i)
        {
            newpoints[x] = points[i];
            ++x;
        }
        points = newpoints;

        BezierControlPointMode[] newmodes = new BezierControlPointMode[modes.Length];
        x = 0;
        for (int i = modes.Length - 1; i >= 0; --i)
        {
            newmodes[x] = modes[i];
            ++x;
        }
        modes = newmodes;
    }

    #if UNITY_EDITOR
    void OnDrawGizmos()
    {
        Gizmos.color = Color.gray;
        Vector3 p0 = transform.TransformPoint(GetControlPoint(0));
        for (int i = 1; i < ControlPointCount; i += 3)
        {
            Vector3 p1 = transform.TransformPoint(GetControlPoint(i));
            Vector3 p2 = transform.TransformPoint(GetControlPoint(i + 1));
            Vector3 p3 = transform.TransformPoint(GetControlPoint(i + 2));

            UnityEditor.Handles.DrawBezier(p0, p3, p1, p2, Color.white, null, 2f);
            p0 = p3;
        }

        if (arcSubDivs != null)
        {
            UnityEditor.Handles.color = Color.blue;
            for (int i = 0; i < arcSubDivs.Length; ++i)
            {
                Vector3 pos = GetPoint(arcSubDivs[i] / (float)(arcLengths.Length - 1));
                Vector3 dir = Camera.current.gameObject.transform.position - pos;
                UnityEditor.Handles.DrawSolidDisc(pos, dir, UnityEditor.HandleUtility.GetHandleSize(pos) * 0.04f);
            }
        }

        if (DrawIncrements && arcLengths != null)
        {
            UnityEditor.Handles.color = Color.red;
            for (int i = 0; i < arcLengths.Length; ++i)
            {
                Vector3 pos = GetPoint(i / (float)(arcLengths.Length - 1));
                Vector3 dir = Camera.current.gameObject.transform.position - pos;
                UnityEditor.Handles.DrawSolidDisc(pos, dir, UnityEditor.HandleUtility.GetHandleSize(pos) * 0.012f);
            }
        }
    }
#endif
}