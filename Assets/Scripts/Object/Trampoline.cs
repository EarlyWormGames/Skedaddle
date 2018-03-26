using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputNew;

public class Trampoline : ActionObject
{
    [System.Serializable]
    public class TrampSpline
    {
        public ANIMAL_NAME AnimalName;
        public BezierSpline Spline;
    }

    public UnityEvent OnSplineEnd;

    [Header("Misc Settings")]
    public bool LaunchOnTrigger = false;
    public bool AllowObjects = false;
    public Animator AnimatorController;

    [Header("Spline Settings")]
    public TrampSpline[] AnimalSplines;
    public BezierSpline ObjectSpline;
    public BezierSpline ExitSpline;
    public float LaunchSplineSpeed = 1, ExitSplineSpeed = 0.5f;
    public AnimationCurve LaunchCurve = AnimationCurve.Linear(0, 0, 1, 1);

    [Header("Input")]
    public AxisAction LaunchAxis;
    public bool InvertAxis;

    private Rigidbody lastLaunched;
    private List<Transform> WrongIn = new List<Transform>();
    private Dictionary<Transform, RigidbodySettings> tempSettings = new Dictionary<Transform, RigidbodySettings>();

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

        AnimatorController.SetBool("TooHeavy", WrongIn.Count > 0);
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
        BezierSpline spline = ObjectSpline;
        foreach (var item in AnimalSplines)
        {
            if (item.AnimalName == m_aCurrentAnimal.m_eName)
            {
                spline = item.Spline;
                break;
            }
        }

        m_aCurrentAnimal.m_oCurrentObject = this;

        LaunchItem(m_aCurrentAnimal.transform, spline);
    }

    private void LaunchObject()
    {
        if (tempSettings.ContainsKey(lastLaunched.transform))
            return;

        RigidbodySettings settings = new RigidbodySettings()
        {
            constraints = lastLaunched.constraints
        };
        tempSettings.Add(lastLaunched.transform, settings);

        lastLaunched.useGravity = false;
        lastLaunched.velocity = Vector3.zero;
        lastLaunched.angularVelocity = Vector3.zero;
        lastLaunched.constraints = RigidbodyConstraints.FreezeAll;

        LaunchItem(lastLaunched.transform, ObjectSpline);
    }

    private void LaunchItem(Transform item, BezierSpline spline)
    {
        AnimatorController.SetTrigger("Launch");

        var follower = item.gameObject.AddComponent<BezierSplineFollower>();
        follower.m_Spline = spline;
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

    private void SplineEnd(BezierSplineFollower sender, Transform item)
    {
        Destroy(sender);

        Rigidbody rig = item.GetComponent<Rigidbody>();
        Animal animal = item.GetComponent<Animal>();

        if (animal != null)
        {
            animal.m_oCurrentObject = null;
            OnSplineEnd.Invoke();
            return;
        }

        if (rig != null)
        {
            rig.useGravity = true;
            rig.isKinematic = false;
            rig.constraints = tempSettings[item].constraints;
            tempSettings.Remove(item);
        }
    }
}