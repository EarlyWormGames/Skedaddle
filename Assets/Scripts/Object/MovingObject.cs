using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MovingObject : MonoBehaviour
{
    public Transform PointA, PointB;
    [Tooltip("Time (in s) to transition from point A to B")]
    public float Speed = 1;
    public AnimationCurve MovingCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    protected bool isLerping;
    protected float lerpTimer = 0;
    protected bool movingForward = true;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (isLerping)
            DoSlide();

        OnUpdate();
    }

    protected virtual void OnUpdate() { }

    protected virtual void DoSlide()
    {
        lerpTimer = Mathf.Clamp(lerpTimer + Time.deltaTime, 0, Speed);

        Vector3 start = movingForward ? PointA.position : PointB.position;
        Vector3 end = !movingForward ? PointA.position : PointB.position;

        float t = MovingCurve.Evaluate(lerpTimer / Speed);
        transform.position = Vector3.Lerp(start, end, t);

        if (lerpTimer >= Speed)
            Stop();
    }

    public virtual void Move(bool forward)
    {
        if (forward != movingForward)
        {
            lerpTimer = Speed - lerpTimer;
        }

        isLerping = true;
        movingForward = forward;
    }

    public virtual void Move()
    {
        movingForward = !movingForward;
        isLerping = true;
        lerpTimer = Speed - lerpTimer;
    }

    public virtual void Stop()
    {
        isLerping = false;
    }
}