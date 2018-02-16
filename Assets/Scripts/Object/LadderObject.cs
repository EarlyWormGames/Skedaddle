using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LadderObject : ActionObject
{
    public bool LowExit = true, HighExit = true;
    public float RotateSpeed = 5;
    public ActionObject TopTransition, BottomTransition;
    [EnumFlag] public IgnoreAxis AxesToIgnore = IgnoreAxis.X | IgnoreAxis.Z;
    public FACING_DIR Direction;
    public bool IsRope = false;

    public bool ForceMoveToClosest = true;

    [Tooltip("First = Bottom, Last = Top :: THESE MUST BE IN CORRECT ORDER!")]
    public Transform[] Points;

    [HideInInspector]
    public float moveVelocity;

    private int pointIndex;
    private Loris loris;
    private bool moveDown = true, moveUp = true;
    private bool justEnter;

    protected override void OnStart()
    {
        base.OnStart();

        m_CanBeDetached = false;
        m_CanDetach = false;
        m_bBlocksMovement = true;
        m_bBlocksTurn = true;
    }

    protected override void OnCanTrigger()
    {
        if ((input.moveY.positive.wasJustPressed || input.moveY.negative.wasJustPressed || input.interact.wasJustPressed) && m_aCurrentAnimal == null)
        {
            DoAction();
        }
    }

    protected override void OnUpdate()
    {
        if (loris == null)
            return;

        if (input.interact.wasJustPressed && !justEnter)
        {
            Detach();
            return;
        }
        justEnter = false;

        float[] speed = loris.CalculateMoveSpeed();
        moveVelocity += input.moveY.value * Time.deltaTime * speed[0];
        moveVelocity = Mathf.Clamp(moveVelocity, speed[1], speed[2]);

        if (input.moveY.value == 0)
        {
            moveVelocity = Mathf.MoveTowards(moveVelocity, 0, loris.m_fClimbStopSpeed * Time.deltaTime);
        }

        Move();
    }

    void Move()
    {
        if (moveVelocity == 0)
            return;

        if (!loris.m_bSelected)
            return;

        if (moveVelocity < 0 && !moveDown)
            return;
        else if (moveVelocity < 0 && !moveUp)
            moveUp = true;
        else if (moveVelocity > 0 && !moveUp)
            return;
        else if (moveVelocity > 0 && !moveDown)
            moveDown = true;

        Vector3 splinePos = Vector3.zero;
        pointIndex = Mathf.Clamp(pointIndex, 0, Points.Length - 1);
        splinePos = Points[pointIndex].position;

        splinePos = IgnoreUtils.Calculate(AxesToIgnore, loris.transform.position, splinePos);

        Vector3 dir = splinePos - loris.transform.position;
        float move = moveVelocity;
        if (move < 0)
            move *= -1;

        Vector3 moveDir = dir.normalized * move;

        Debug.Log(loris.m_aMovement.TryMove(loris.transform.position + moveDir));

        float rotateMult = 1;
        if (moveVelocity < 0)
            rotateMult = -1;

        //Vector3 rotation = transform.eulerAngles;
        //Vector3 newRotation = transform.eulerAngles;
        //transform.forward = dir.normalized * rotateMult;
        //newRotation.x = transform.eulerAngles.x;
        //newRotation.y = transform.eulerAngles.y;
        //
        //transform.rotation = Quaternion.Lerp(Quaternion.Euler(rotation), Quaternion.Euler(newRotation), Time.deltaTime * RotateSpeed);

        float dot = Vector3.Dot(dir, loris.transform.up);

        if (dir.magnitude < move || ((dot < 0 && moveVelocity > 0) || (dot > 0 && moveVelocity < 0)))
        {
            if (moveVelocity < 0)
                --pointIndex;
            else
                ++pointIndex;

            if (pointIndex < 0 && !LowExit)
                moveDown = false;
            else if (pointIndex < 0 && LowExit)
                Detach();
            else if (pointIndex > Points.Length - 1 && !HighExit)
                moveUp = false;
            else if (pointIndex > Points.Length - 1 && HighExit)
                Detach();
        }
    }

    public override void DoAction()
    {
        if (m_aCurrentAnimal != null)
            return;

        if (!TryDetach())
            return;

        base.DoAction();
        m_aCurrentAnimal = Animal.CurrentAnimal;
        m_aCurrentAnimal.m_oCurrentObject = this;
        m_aCurrentAnimal.transform.SetParent(transform);
        m_aCurrentAnimal.m_rBody.isKinematic = true;

        loris = (Loris)m_aCurrentAnimal;
        loris.m_bClimbing = true;
        if (IsRope)
            loris.m_bHorizontalRope = true;

        pointIndex = FindClosestPoint(transform.position);

        Vector3 dir = Points[pointIndex].position - loris.transform.position;

        if ((pointIndex == 0 && Vector3.Dot(dir.normalized, Vector3.up) >= 0) ||
            (pointIndex == Points.Length - 1 && Vector3.Dot(dir.normalized, Vector3.up) <= 0))
        {
            loris.transform.position = IgnoreUtils.Calculate(AxesToIgnore, loris.transform.position, Points[pointIndex].position);
        }

        justEnter = true;

        loris.SetDirection(Direction);
    }

    public override void Detach()
    {
        base.Detach();
        moveVelocity = 0;

        m_aCurrentAnimal.m_oCurrentObject = null;
        m_aCurrentAnimal.transform.parent = null;
        m_aCurrentAnimal.m_rBody.isKinematic = false;

        loris.m_bClimbing = false;
        loris.SetDirection(FACING_DIR.NONE);

        if (IsRope)
            loris.m_bHorizontalRope = false;

        if (TopTransition != null && pointIndex > Points.Length - 1)
            TopTransition.AnimalEnter(m_aCurrentAnimal);
        else if (BottomTransition != null && pointIndex < 0)
            BottomTransition.AnimalEnter(m_aCurrentAnimal);

        loris = null;
        m_aCurrentAnimal = null;
    }

    public int FindClosestPoint(Vector3 position)
    {
        float closeDist = -1;
        int index = 0;
        for (int i = 0; i < Points.Length; ++i)
        {
            float dist = Vector3.Distance(Points[i].position, position);
            if (dist < closeDist || closeDist < 0)
            {
                index = i;
                closeDist = dist;
            }
        }
        return index;
    }
}