﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Dig : ActionObject
{
    public Transform StartPoint;
    public Transform FinishPoint;
    public BezierSplineFollower Spline;

    public FACING_DIR StartDirection, EndDirection;

    public UnityEvent OnSplineStart;
    public UnityEvent OnDirtExplode;

    public bool Reverse;
    public bool WallDigStart;
    public bool WallDigEnd;

    private Collider trigger;
    private Dig otherDig;

    private void OnEnable()
    {
        Spline.OnPathEnd.AddListener(SplineEnd);
    }

    private void OnDisable()
    {
        Spline.OnPathEnd.RemoveListener(SplineEnd);
    }

    protected override void OnStart()
    {
        base.OnStart();

        m_bBlocksMovement = true;
        m_bBlocksTurn = true;
        m_CanBeDetached = false;
        m_CanDetach = false;

        trigger = GetComponent<Collider>();
    }

    protected override void OnCanTrigger()
    {
        if (m_aCurrentAnimal != null)
            return;
        if (input.interact.wasJustPressed)
        {
            Begin();
        }
    }

    void Begin()
    {
        if (!TryDetach())
            return;

        base.DoAction();

        m_aCurrentAnimal = Animal.CurrentAnimal;
        m_aCurrentAnimal.m_oCurrentObject = this;
        m_aCurrentAnimal.m_aMovement.moveVelocity = 0;
        m_aCurrentAnimal.m_bCheckGround = false;
        m_aCurrentAnimal.m_aMovement.StopSpline();

        Anteater anteater = (Anteater)m_aCurrentAnimal;
        anteater.m_bDigging = true;
        anteater.m_bDigInWall = WallDigStart;
        anteater.m_rBody.useGravity = false;
        anteater.SetDirection(StartDirection, false);

        anteater.transform.position = StartPoint.position;
        m_aCurrentAnimal.SetColliderActive(false, this);

        m_lAnimalsIn.RemoveAll(anteater);

        AnimalController.Instance.CanSwap = false;
    }

    public override void DoAction()
    {
        Anteater anteater = (Anteater)m_aCurrentAnimal;
        Spline.m_MoveObject = m_aCurrentAnimal.transform;
        anteater.SetDirection(EndDirection, false);
        anteater.m_bDigInWall = WallDigEnd;
        Spline.Follow(Reverse);

        OnSplineStart.Invoke();
    }

    public void DirtExplode()
    {
        OnDirtExplode.Invoke();
    }

    void SplineEnd(BezierSplineFollower sender, Transform trackedItem)
    {
        if (m_aCurrentAnimal == null)
            return;

        Anteater anteater = (Anteater)m_aCurrentAnimal;
        anteater.m_bDigging = false;
        
        anteater.transform.position = FinishPoint.position;
    }

    public void Finish()
    {
        m_aCurrentAnimal.SetColliderActive(true);
        m_aCurrentAnimal.m_bCheckGround = true;
        m_aCurrentAnimal.m_rBody.useGravity = true;
        m_aCurrentAnimal.m_oCurrentObject = null;
        m_aCurrentAnimal.SetDirection(FACING_DIR.NONE, false);
        if (EndDirection == FACING_DIR.LEFT)
        {
            m_aCurrentAnimal.m_bTurned = true;
            m_aCurrentAnimal.m_bFacingLeft = true;
        }
        if(EndDirection == FACING_DIR.RIGHT)
        {
            m_aCurrentAnimal.m_bTurned = false;
            m_aCurrentAnimal.m_bFacingLeft = false;
        }
        m_aCurrentAnimal = null;

        AnimalController.Instance.CanSwap = true;
    }
}