using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlidingObject : MovingObject
{
    public Transform PointA, PointB;

    protected override void DoSlide()
    {
        base.DoSlide();

        if (PointA == null)
        {
            PointA = new GameObject().transform;
            PointA.position = transform.position;
        }

        Vector3 start = movingForward ? PointA.position : PointB.position;
        Vector3 end = !movingForward ? PointA.position : PointB.position;

        float t = MovingCurve.Evaluate(lerpTimer / Speed);
        transform.position = Vector3.Lerp(start, end, t);
    }
}