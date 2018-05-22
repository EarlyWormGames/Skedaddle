using System.Collections;
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
    protected override bool HeadTriggerOnly { get; set; }

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

        //Completely override movement by stopping any spline the loris is on
        loris.m_aMovement.StopSpline();

        //The loris should not collide with anything but this
        loris.SetColliderActive(false, this);
        loris.transform.position = LorisSitPoint.position;
        shooting = false;
        firstpress = true;

        //We don't need to worry if the loris is in now
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
        {
            Shoot();
        }
        else if ((GameManager.mainMap.moveX.negative.wasJustPressed && (!facingLeft || InvertKeys) ||
            GameManager.mainMap.moveX.positive.wasJustPressed && (facingLeft || InvertKeys) && !isLerping) || firstpress)
        {
            Switch();
            firstpress = false;
        }
    }

    /// <summary>
    /// Fire ze cannons!
    /// </summary>
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

    /// <summary>
    /// Called once the spline has finished
    /// </summary>
    void SplineEnd(BezierSplineFollower sender, Transform trackedItem)
    {
        loris.SetColliderActive(true);

        loris.m_bInCannon = false;
        loris.m_rBody.isKinematic = false;
        loris = null;

        Detach(this);

        isLerping = true;
        timer = 0;

        //Rotate from current facing to the starting rotation
        Vector3 forward = (facingLeft ? LeftFacing.position : RightFacing.position) - RotateObject.position;
        start = Quaternion.LookRotation(forward.normalized, Vector3.up);
        end = startRotation;
    }

    /// <summary>
    /// Invert the facing direction from left/right
    /// </summary>
    void Switch()
    {
        facingLeft = !facingLeft;
        isLerping = true;
        timer = 0;

        //Grab the forward vector, depending on which direction we're facing
        Vector3 forward = (!facingLeft ? LeftFacing.position : RightFacing.position) - RotateObject.position;

        //If it has been reset, use the initial rotation 
        if (!useStartRotation)
            start = Quaternion.LookRotation(forward.normalized, Vector3.up);
        else
            start = startRotation;

        //Essentially inverts the facing direction
        forward = (facingLeft ? LeftFacing.position : RightFacing.position) - RotateObject.position;

        //The ending rotation should be the forward direction
        end = Quaternion.LookRotation(forward.normalized, Vector3.up);

        //We don't need to use the starting rotation, now that we've switched once
        useStartRotation = false;
    }
}