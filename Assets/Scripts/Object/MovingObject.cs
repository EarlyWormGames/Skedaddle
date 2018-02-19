﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[AddComponentMenu("")]
public class MovingObject : MonoBehaviour
{
    [Tooltip("Time (in s) to transition from point A to B")]
    public float Speed = 1;
    public AnimationCurve MovingCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    public UnityEvent ForwardEnd;
    public UnityEvent BackwardEnd;

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

        if (lerpTimer >= Speed)
            Stop();

        if (movingForward)
            ForwardEnd.Invoke();
        else
            BackwardEnd.Invoke();
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