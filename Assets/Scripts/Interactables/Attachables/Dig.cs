using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Dig : AttachableInteract
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

        BlocksMovement = true;
        BlocksTurn = true;
        CanDetach = false;

        trigger = GetComponent<Collider>();
    }

    protected override bool CheckDetach()
    {
        return false;
    }

    protected override void DoInteract(Animal caller)
    {
        if (!TryDetachOther())
            return;

        Attach(caller);

        AttachedAnimal.m_aMovement.moveVelocity = 0;
        AttachedAnimal.m_bCheckGround = false;
        AttachedAnimal.m_aMovement.StopSpline();

        Anteater anteater = (Anteater)AttachedAnimal;
        anteater.m_bDigging = true;
        anteater.m_bDigInWall = WallDigStart;
        anteater.m_rBody.useGravity = false;
        anteater.SetDirection(StartDirection, false);

        anteater.transform.position = StartPoint.position;
        AttachedAnimal.SetColliderActive(false, this);

        AnimalsIn.RemoveAll(anteater);

        AnimalController.Instance.CanSwap = false;
    }

    public void StartSpline()
    {
        Anteater anteater = (Anteater)AttachedAnimal;
        Spline.m_MoveObject = AttachedAnimal.transform;
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
        if (AttachedAnimal == null)
            return;

        Anteater anteater = (Anteater)AttachedAnimal;
        anteater.m_bDigging = false;
        
        anteater.transform.position = FinishPoint.position;
    }

    public void Finish()
    {
        AttachedAnimal.SetColliderActive(true);
        AttachedAnimal.m_bCheckGround = true;
        AttachedAnimal.m_rBody.useGravity = true;
        AttachedAnimal.SetDirection(FACING_DIR.NONE, false);
        if (EndDirection == FACING_DIR.LEFT)
        {
            AttachedAnimal.m_bTurned = true;
            AttachedAnimal.m_bFacingLeft = true;
        }
        if(EndDirection == FACING_DIR.RIGHT)
        {
            AttachedAnimal.m_bTurned = false;
            AttachedAnimal.m_bFacingLeft = false;
        }

        Detach(this);

        AnimalController.Instance.CanSwap = true;
    }
}