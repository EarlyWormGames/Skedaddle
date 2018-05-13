﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Cannon : Attachable
{
    public float RotateTime = 1;
    public Transform RotateObject, LorisSitPoint;
    public AnimationCurve RotateCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public BezierSplineFollower SplineLeft, SplineRight;
    public bool InvertKeys;

    [Tooltip("Uses the forward of this transform for this direction")]
    public Transform LeftFacing, RightFacing;

    public UnityEvent OnShoot;

    private Loris loris;
    private bool facingLeft = false;
    private bool isLerping;
    private float timer;
    private Quaternion startRotation;
    private Quaternion start, end;
    private bool useStartRotation = true;
    private bool shooting = false;
    private bool firstpress = true;

    protected override void OnStart()
    {
        base.OnStart();
        startRotation = RotateObject.rotation;

        CanDetach = false;
        BlocksMovement = true;
        BlocksTurn = true;
    }

    private void OnEnable()
    {
        if (SplineLeft != null)
        {
            if (SplineLeft.OnPathEnd == null)
                SplineLeft.OnPathEnd = new BezierSplineFollower.FollowerEvent();

            SplineLeft.OnPathEnd.AddListener(SplineEnd);
        }
        if (SplineRight != null)
        {
            if (SplineRight.OnPathEnd == null)
                SplineRight.OnPathEnd = new BezierSplineFollower.FollowerEvent();

            SplineRight.OnPathEnd.AddListener(SplineEnd);
        }
    }

    private void OnDisable()
    {
        if (SplineLeft != null)
            SplineLeft.OnPathEnd.RemoveListener(SplineEnd);
        if (SplineRight != null)
            SplineRight.OnPathEnd.RemoveListener(SplineEnd);
    }
    
    protected override void OnAnimalEnter(Animal animal)
    {
        if (!TryDetachOther())
            return;

        Attach(animal);

        loris = (Loris)AttachedAnimal;
        loris.m_bInCannon = true;
        loris.m_rBody.isKinematic = true;

        loris.m_aMovement.StopSpline();

        loris.SetColliderActive(false, this);
        loris.transform.position = LorisSitPoint.position;
        shooting = false;
        firstpress = true;

        AnimalsIn.RemoveAll(loris);
    }

    protected override void OnUpdate()
    {
        if (isLerping)
        {
            timer = Mathf.Clamp(timer + Time.deltaTime, 0, RotateTime);

            float t = RotateCurve.Evaluate(timer / RotateTime);
            RotateObject.rotation = Quaternion.Lerp(start, end, t);

            if (timer >= RotateTime)
                isLerping = false;
        }

        if (loris == null)
            return;

        if (!loris.m_bSelected)
            return;

        if (isLerping && !shooting)
        {
            loris.transform.position = LorisSitPoint.position;
        }
        else if (GameManager.mainMap.interact.wasJustPressed && !isLerping)
            Shoot();
        else if ((GameManager.mainMap.moveX.negative.wasJustPressed && (!facingLeft || InvertKeys) ||
            GameManager.mainMap.moveX.positive.wasJustPressed && (facingLeft || InvertKeys) && !isLerping) || firstpress)
        {
            Switch();
            firstpress = false;
        }
    }

    public void Shoot()
    {
        if (facingLeft)
        {
            SplineLeft.m_MoveObject = loris.transform;
            SplineLeft.Follow();
        }
        else if (!facingLeft)
        {
            SplineRight.m_MoveObject = loris.transform;
            SplineRight.Follow();
        }

        shooting = true;
        useStartRotation = true;

        OnShoot.Invoke();
    }

    void SplineEnd(BezierSplineFollower sender, Transform trackedItem)
    {
        loris.SetColliderActive(true);

        loris.m_bInCannon = false;
        loris.m_rBody.isKinematic = false;
        loris = null;

        Detach(this);


        isLerping = true;
        timer = 0;

        Vector3 forward = (facingLeft ? LeftFacing.position : RightFacing.position) - RotateObject.position;
        start = Quaternion.LookRotation(forward.normalized, Vector3.up);
        end = startRotation;
    }

    void Switch()
    {
        facingLeft = !facingLeft;
        isLerping = true;
        timer = 0;

        Vector3 forward = (!facingLeft ? LeftFacing.position : RightFacing.position) - RotateObject.position;
        if (!useStartRotation)
            start = Quaternion.LookRotation(forward.normalized, Vector3.up);
        else
            start = startRotation;

        forward = (facingLeft ? LeftFacing.position : RightFacing.position) - RotateObject.position;
        end = Quaternion.LookRotation(forward.normalized, Vector3.up);

        useStartRotation = false;
    }
}