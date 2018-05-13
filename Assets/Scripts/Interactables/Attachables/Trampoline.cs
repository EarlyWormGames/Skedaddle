using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputNew;

public class Trampoline : AttachableInteract
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
    private bool wasPositive = true;

    protected override void OnStart()
    {
        base.OnStart();

        if (!LaunchOnTrigger)
            LaunchAxis.Bind(GameManager.Instance.input.handle);

        CanDetach = true;
        BlocksMovement = true;
        BlocksTurn = true;
    }

    protected override bool CheckDetach()
    {
        return true;
    }

    protected override bool CheckInput(ActionSlot input, Animal caller)
    {
        if (!LaunchOnTrigger)
        {
            if (!AllowsAnimal(caller))
                return false;

            if ((LaunchAxis.control.positive.wasJustPressed && !InvertAxis) ||
                (LaunchAxis.control.negative.wasJustPressed && InvertAxis))
            {
                wasPositive = true;
                return true;
            }
            else if ((LaunchAxis.control.negative.wasJustPressed && !InvertAxis) ||
                (LaunchAxis.control.positive.wasJustPressed && InvertAxis))
            {
                wasPositive = false;
                return true;
            }
        }
        return false;
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();

        AnimatorController.SetBool("TooHeavy", WrongIn.Count > 0);
    }

    protected override void DoInteract(Animal caller)
    {
        if (!TryDetachOther(caller))
            return;

        Attach(caller);

        if (wasPositive)
            ExitAnimal();
        else
            LaunchAnimal();
    }

    protected override void OnAnimalEnter(Animal a_animal)
    {
        base.OnAnimalEnter(a_animal);
        if (!LaunchOnTrigger)
            return;

        if (!TryDetachOther(a_animal))
            return;

        Attach(a_animal);
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
            if (item.AnimalName == AttachedAnimal.m_eName)
            {
                spline = item.Spline;
                break;
            }
        }

        RigidbodySettings settings = new RigidbodySettings()
        {
            constraints = AttachedAnimal.m_rBody.constraints
        };
        tempSettings.Add(AttachedAnimal.transform, settings);
        AttachedAnimal.m_rBody.constraints = RigidbodyConstraints.FreezeAll;
        AttachedAnimal.m_rBody.velocity = Vector3.zero;
        AttachedAnimal.m_rBody.angularVelocity = Vector3.zero;
        AttachedAnimal.m_rBody.useGravity = false;

        LaunchItem(AttachedAnimal.transform, spline);
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
        var follower = AttachedAnimal.gameObject.AddComponent<BezierSplineFollower>();
        follower.m_Spline = ExitSpline;
        follower.m_MoveObject = AttachedAnimal.transform;
        follower.m_FollowTime = LaunchSplineSpeed;
        follower.OnPathEnd = new BezierSplineFollower.FollowerEvent();
        follower.OnPathEnd.AddListener(SplineEnd);
        follower.Follow();
    }

    private void SplineEnd(BezierSplineFollower sender, Transform item)
    {
        Destroy(sender);

        if (!tempSettings.ContainsKey(item))
            return;

        Rigidbody rig = item.GetComponent<Rigidbody>();
        Animal animal = item.GetComponent<Animal>();

        if (animal != null)
        {
            animal.currentAttached.Detach(this);
            animal.m_rBody.useGravity = true;
            animal.m_rBody.constraints = tempSettings[item].constraints;
            tempSettings.Remove(item);

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

    protected override void OnDetach(Animal animal)
    {
        base.OnDetach(animal);

        if (!tempSettings.ContainsKey(animal.transform))
            return;

        Destroy(animal.GetComponent<BezierSplineFollower>());

        animal.m_rBody.useGravity = true;
        animal.m_rBody.constraints = tempSettings[animal.transform].constraints;
        tempSettings.Remove(animal.transform);
    }
}