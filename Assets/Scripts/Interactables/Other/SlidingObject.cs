using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// an object that can slide ?
/// </summary>
public class SlidingObject : MovingObject
{
    public Transform PointA, PointB;

    protected override bool DoSlide(float time)
    {
        if (PointA == null)
        {
            PointA = new GameObject().transform;
            PointA.position = transform.position;
        }

        Vector3 start = movingForward ? PointA.position : PointB.position;
        Vector3 end = !movingForward ? PointA.position : PointB.position;

        float t = MovingCurve.Evaluate(time / Speed);
        return DoMove(Vector3.Lerp(start, end, t));
    }

    protected virtual bool DoMove(Vector3 position)
    {
        transform.position = position;
        return true;
    }
}