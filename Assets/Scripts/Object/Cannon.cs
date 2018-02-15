using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Cannon : ActionObject
{
    public float RotateTime = 1;
    public Transform RotateObject, LorisSitPoint;
    public AnimationCurve RotateCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public BezierSplineFollower SplineLeft, SplineRight;

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

        m_CanBeDetached = false;
        m_CanDetach = false;
        m_bBlocksMovement = true;
        m_bBlocksTurn = true;
    }

    private void OnEnable()
    {
        if (SplineLeft != null)
            SplineLeft.OnPathEnd.AddListener(SplineEnd);
        if (SplineRight != null)
            SplineRight.OnPathEnd.AddListener(SplineEnd);
    }

    private void OnDisable()
    {
        if (SplineLeft != null)
            SplineLeft.OnPathEnd.RemoveListener(SplineEnd);
        if (SplineRight != null)
            SplineRight.OnPathEnd.RemoveListener(SplineEnd);
    }

    protected override void OnCanTrigger()
    {
        DoAction();
    }

    public override void DoAction()
    {
        if (!TryDetach())
            return;

        if (m_aCurrentAnimal != null)
            return;

        base.DoAction();
        m_aCurrentAnimal = Animal.CurrentAnimal;
        loris = (Loris)m_aCurrentAnimal;
        loris.m_oCurrentObject = this;
        loris.m_bInCannon = true;
        loris.m_rBody.isKinematic = true;

        loris.m_tCollider.gameObject.layer = LayerMask.NameToLayer("AnimalNoCollide");
        loris.transform.position = LorisSitPoint.position;
        shooting = false;
        firstpress = true;

        m_lAnimalsIn.Remove(loris);
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

        if (isLerping && !shooting)
        {
            loris.transform.position = LorisSitPoint.position;
        }
        else if (input.interact.wasJustPressed && !isLerping)
            Shoot();
        else if ((input.moveX.negative.wasJustPressed && !facingLeft ||
            input.moveX.positive.wasJustPressed && facingLeft && !isLerping) || firstpress)
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

    void SplineEnd()
    {
        loris.m_tCollider.gameObject.layer = LayerMask.NameToLayer("Animal");

        loris.m_bInCannon = false;
        loris.m_rBody.isKinematic = false;
        loris.m_oCurrentObject = null;
        loris = null;
        m_aCurrentAnimal = null;

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