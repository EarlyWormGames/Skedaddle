using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalSplineMover : ActionObject
{
    public BezierSplineFollower SplineFollower;
    public bool DoOnTrigger = true;

    protected override void OnStart()
    {
        base.OnStart();

        m_CanDetach = true;
        m_CanBeDetached = true;
        m_bBlocksMovement = true;
        m_bBlocksTurn = true;
    }

    protected override void OnCanTrigger()
    {
        if (!DoOnTrigger && m_aCurrentAnimal == null)
        {
            if (input.interact.wasJustPressed)
            {
                m_aCurrentAnimal = Animal.CurrentAnimal;
                DoAction();
            }
        }
    }

    public override void AnimalEnter(Animal a_animal)
    {
        if (DoOnTrigger && m_aCurrentAnimal == null)
        {
            m_aCurrentAnimal = a_animal;
            DoAction();
        }
    }

    public override void DoAction()
    {
        if (!TryDetach())
            return;

        base.DoAction();

        m_aCurrentAnimal.m_oCurrentObject = this;
        SplineFollower.m_MoveObject = m_aCurrentAnimal.transform;
        SplineFollower.Follow();
    }

    public override void Detach(bool destroy = false)
    {
        base.Detach();

        m_aCurrentAnimal.m_oCurrentObject = null;
        m_aCurrentAnimal = null;
    }

    void SplineEnd(BezierSplineFollower sender)
    {
        Detach();
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