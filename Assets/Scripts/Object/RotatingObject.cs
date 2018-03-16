using UnityEngine;
using System.Collections;

public class RotatingObject : MovingObject
{
    public Vector3 StartRotation;
    public Vector3 EndRotation;
    public bool UseLocal;

    protected override bool DoSlide(float time)
    {
        Vector3 start = movingForward ? StartRotation : EndRotation;
        Vector3 end = !movingForward ? StartRotation : EndRotation;

        float t = MovingCurve.Evaluate(time / Speed);

        if (!UseLocal)
            transform.eulerAngles = Vector3.Lerp(start, end, t);
        else
            transform.localEulerAngles = Vector3.Lerp(start, end, t);

        return true;
    }
}
