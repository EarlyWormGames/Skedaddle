using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Cannon : ActionObject
{
    public float RotateTime = 1;
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

    public override void DoAction()
    {
        if (!TryDetach())
            return;

        m_aCurrentAnimal = Animal.CurrentAnimal;
        loris = (Loris)m_aCurrentAnimal;
        loris.m_oCurrentObject = this;
        loris.m_bInCannon = true;
        loris.m_rBody.isKinematic = true;

        loris.m_tCollider.gameObject.layer = LayerMask.NameToLayer("AnimalNoCollider");
    }

    protected override void OnUpdate()
    {
        if (isLerping)
        {
            timer = Mathf.Clamp(timer + Time.deltaTime, 0, RotateTime);

            float t = RotateCurve.Evaluate(timer / RotateTime);
            transform.rotation = Quaternion.Lerp(start, end, t);

            if (timer >= RotateTime)
                isLerping = false;
        }

        if (loris == null)
            return;
        
        if (input.interact.wasJustPressed && !isLerping)
            Shoot();
        else if (input.moveX.negative.wasJustPressed && !facingLeft ||
            input.moveX.positive.wasJustPressed && facingLeft && !isLerping)
            Switch();
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

        useStartRotation = true;

        isLerping = true;
        timer = 0;
        end = startRotation;
        start = Quaternion.LookRotation(facingLeft ? LeftFacing.forward : RightFacing.forward, transform.up);
    }

    void SplineEnd()
    {
        loris.m_tCollider.gameObject.layer = LayerMask.NameToLayer("Animal");

        loris.m_bInCannon = false;
        loris.m_rBody.isKinematic = false;
        loris.m_oCurrentObject = null;
        loris = null;
        m_aCurrentAnimal = null;
    }

    void Switch()
    {
        facingLeft = !facingLeft;
        isLerping = true;
        timer = 0;

        if (!useStartRotation)
            start = Quaternion.LookRotation(facingLeft ? LeftFacing.forward : RightFacing.forward, transform.up);
        else
            start = startRotation;

        end = Quaternion.LookRotation(!facingLeft ? LeftFacing.forward : RightFacing.forward, transform.up);
    }
}