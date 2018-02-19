using UnityEngine;
using System.Collections;

public class RotatingObject : MovingObject
{
    public Vector3 StartRotation;
    public Vector3 EndRotation;
    public bool UseLocal;

    protected override void DoSlide()
    {
        base.DoSlide();

        Quaternion start = Quaternion.Euler(movingForward ? StartRotation : EndRotation);
        Quaternion end = Quaternion.Euler(!movingForward ? StartRotation : EndRotation);

        float t = MovingCurve.Evaluate(lerpTimer / Speed);

        if (!UseLocal)
            transform.rotation = Quaternion.Lerp(start, end, t);
        else
            transform.localRotation = Quaternion.Lerp(start, end, t);
    }
}
