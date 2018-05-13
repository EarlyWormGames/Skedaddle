using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputNew;

public class AnimalSplineMover : AttachableInteract
{
    public BezierSplineFollower SplineFollower;
    public bool DoOnTrigger = true;

    protected override void OnStart()
    {
        CanDetach = true;
        BlocksMovement = true;
        BlocksTurn = true;
    }

    protected override void OnAnimalEnter(Animal a_animal)
    {
        //Register keys
        base.OnAnimalEnter(a_animal);

        if (DoOnTrigger && AttachedAnimal == null)
        {
            OnInteract(AttachedAnimal, true);
        }
    }

    protected override void DoInteract(Animal caller)
    {
        OnInteract(caller, false);
    }

    void OnInteract(Animal caller, bool wasTrigger)
    {
        if (DoOnTrigger ^ !wasTrigger)
            return;

        if (!TryDetachOther())
            return;

        Attach(caller);

        SplineFollower.m_MoveObject = AttachedAnimal.transform;
        SplineFollower.Follow();
    }

    void SplineEnd(BezierSplineFollower sender, Transform trackedObject)
    {
        Detach(this);
    }

    private void OnEnable()
    {
        if (SplineFollower)
        {
            if (SplineFollower.OnPathEnd == null)
                SplineFollower.OnPathEnd = new BezierSplineFollower.FollowerEvent();
            SplineFollower.OnPathEnd.AddListener(SplineEnd);
        }
    }

    private void OnDisable()
    {
        if (SplineFollower)
            SplineFollower.OnPathEnd.RemoveListener(SplineEnd);
    }
}