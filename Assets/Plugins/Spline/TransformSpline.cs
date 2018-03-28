using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TransformDirection
{
    FORWARD,
    BACKWARD,
    UP,
    DOWN,
    RIGHT,
    LEFT,
}

[ExecuteInEditMode]
public class TransformSpline : MonoBehaviour
{
    public Transform PointA, PointB;
    public float aStartDistance = 1, bStartDistance = 1;
    public TransformDirection aDirection, bDirection;
    public Vector3 aCurveDistance, bCurveDistance;

    private BezierSpline spline;

    // Update is called once per frame
    void Update()
    {
        if (spline == null)
            spline = GetComponent<BezierSpline>();

        if (PointA != null)
        {
            var dir = GetDirection(PointA, aDirection);
            var start = PointA.position + (dir * aStartDistance);

            var rot = Quaternion.LookRotation(dir);

            spline.SetControlPointWorld(0, start);
            spline.SetControlPointWorld(1, start + (rot * aCurveDistance));
        }

        if (PointB != null)
        {
            var dir = GetDirection(PointB, aDirection);
            var start = PointB.position + (dir * bStartDistance);

            var rot = Quaternion.LookRotation(dir);

            spline.SetControlPointWorld(2, start + (rot * bCurveDistance));
            spline.SetControlPointWorld(3, start);
        }
    }

    public static Vector3 GetDirection(Transform transform, TransformDirection direction)
    {
        switch (direction)
        {
            case TransformDirection.FORWARD:
                return transform.forward;
            case TransformDirection.BACKWARD:
                return -transform.forward;

            case TransformDirection.UP:
                return transform.up;
            case TransformDirection.DOWN:
                return -transform.up;

            case TransformDirection.RIGHT:
                return transform.right;
            case TransformDirection.LEFT:
                return -transform.right;
        }
        return transform.forward;
    }
}