using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputNew;

public class Trampoline : ActionObject
{
    public UnityEvent OnSplineEnd;

    [Header("Misc Settings")]
    public bool LaunchOnTrigger = false;
    public bool AllowObjects = false;
    public Animator AnimatorController;

    [Header("Spline Settings")]
    public BezierSpline LaunchSpline;
    public BezierSpline ExitSpline;
    public float LaunchSplineSpeed = 1, ExitSplineSpeed = 0.5f;
    public AnimationCurve LaunchCurve = AnimationCurve.Linear(0, 0, 1, 1);

    [Header("Input")]
    public AxisAction LaunchAxis;
    public bool InvertAxis;

    private Rigidbody lastLaunched;
    private List<Transform> WrongIn = new List<Transform>();

    protected override void OnStart()
    {
        base.OnStart();

        if (!LaunchOnTrigger)
            LaunchAxis.Bind(GameManager.Instance.input.handle);

        m_CanBeDetached = false;
        m_CanDetach = true;
        m_bBlocksMovement = true;
        m_bBlocksTurn = true;
    }

    protected override void OnCanTrigger()
    {
        if (!LaunchOnTrigger)
        {
            if ((LaunchAxis.control.positive.wasJustPressed && !InvertAxis) ||
                (LaunchAxis.control.negative.wasJustPressed && InvertAxis))
            {
                m_aCurrentAnimal = Animal.CurrentAnimal;
                LaunchAnimal();
            }
            else if ((LaunchAxis.control.negative.wasJustPressed && !InvertAxis) ||
                (LaunchAxis.control.positive.wasJustPressed && InvertAxis))
            {
                m_aCurrentAnimal = Animal.CurrentAnimal;
                ExitAnimal();
            }
        }
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();

        AnimatorController.SetBool("Too Heavy", WrongIn.Count > 0);
    }

    public override void AnimalEnter(Animal a_animal)
    {
        if (!LaunchOnTrigger)
            return;
        m_aCurrentAnimal = a_animal;
        LaunchAnimal();
    }

    protected override void ObjectEnter(Collider a_col)
    {
        if (!LaunchOnTrigger || !AllowObjects)
            WrongIn.Add(a_col.transform);
        else
        {
            lastLaunched = a_col.GetComponent<Rigidbody>();
            if (lastLaunched == null)
                lastLaunched = a_col.GetComponentInParent<Rigidbody>();
            LaunchObject();
        }
    }

    protected override void ObjectExit(Collider a_col)
    {
        if (WrongIn.Contains(a_col.transform))
            WrongIn.Remove(a_col.transform);
    }

    protected override void WrongAnimalEnter(Animal a_animal)
    {
        if(!WrongIn.Contains(a_animal.transform))
            WrongIn.Add(a_animal.transform);
    }

    protected override void WrongAnimalExit(Animal a_animal)
    {
        if (WrongIn.Contains(a_animal.transform))
            WrongIn.Remove(a_animal.transform);
    }

    private void LaunchAnimal()
    {
        m_aCurrentAnimal.m_oCurrentObject = this;

        LaunchItem(m_aCurrentAnimal.transform);
    }

    private void LaunchObject()
    {
        lastLaunched.useGravity = false;
        lastLaunched.velocity = Vector3.zero;
        lastLaunched.angularVelocity = Vector3.zero;
        lastLaunched.isKinematic = true;

        LaunchItem(lastLaunched.transform);
    }

    private void LaunchItem(Transform item)
    {
        AnimatorController.SetTrigger("Launch");

        var follower = item.gameObject.AddComponent<BezierSplineFollower>();
        follower.m_Spline = LaunchSpline;
        follower.m_MoveObject = item.transform;
        follower.m_FollowTime = LaunchSplineSpeed;
        follower.m_Curve = LaunchCurve;

        follower.OnPathEnd = new BezierSplineFollower.FollowerEvent();
        follower.OnPathEnd.AddListener(SplineEnd);

        follower.Follow();
    }

    private void ExitAnimal()
    {
        m_aCurrentAnimal.m_oCurrentObject = this;

        var follower = m_aCurrentAnimal.gameObject.AddComponent<BezierSplineFollower>();
        follower.m_Spline = ExitSpline;
        follower.m_MoveObject = m_aCurrentAnimal.transform;
        follower.m_FollowTime = LaunchSplineSpeed;
        follower.OnPathEnd = new BezierSplineFollower.FollowerEvent();
        follower.OnPathEnd.AddListener(SplineEnd);
        follower.Follow();
    }

    private void SplineEnd(BezierSplineFollower sender)
    {
        Destroy(sender);
        if (m_aCurrentAnimal != null)
        {
            m_aCurrentAnimal.m_oCurrentObject = null;
            OnSplineEnd.Invoke();
        }
        else if (lastLaunched != null)
        {
            lastLaunched.useGravity = true;
            lastLaunched.isKinematic = false;
        }
    }
}