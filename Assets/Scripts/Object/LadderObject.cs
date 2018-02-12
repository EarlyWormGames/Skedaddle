using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LadderObject : ActionObject
{
    public bool LowExit, HighExit;
    public float RotateSpeed = 5;
    public ActionObject TopTransition, BottomTransition;
    public Vector3 IgnoreAxes = new Vector3(1, 0, 1);
    public FACING_DIR Direction;

    [Tooltip("First = Bottom, Last = Top :: THESE MUST BE IN CORRECT ORDER!")]
    public Transform[] Points;

    [HideInInspector]
    public float moveVelocity;


    private int pointIndex;
    private Loris loris;
    private bool moveDown = true, moveUp = true;
    private bool justEnter;

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

        if (IgnoreAxes.x > 0)
            splinePos.x = loris.transform.position.x;
        if (IgnoreAxes.y > 0)
            splinePos.y = loris.transform.position.y;
        if (IgnoreAxes.z > 0)
            splinePos.z = loris.transform.position.z;

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
        if (Animal.CurrentAnimal.m_oCurrentObject != null)
        {
            if (!m_CanDetach || !Animal.CurrentAnimal.m_oCurrentObject.m_CanBeDetached)
                return;
            Animal.CurrentAnimal.m_oCurrentObject.Detach();
        }

        m_aCurrentAnimal = Animal.CurrentAnimal;
        m_aCurrentAnimal.m_oCurrentObject = this;
        m_aCurrentAnimal.transform.SetParent(transform);
        m_aCurrentAnimal.m_rBody.isKinematic = true;

        loris = (Loris)m_aCurrentAnimal;
        loris.m_bClimbing = true;

        pointIndex = FindClosestPoint(transform.position);
        justEnter = true;

        loris.SetDirection(Direction);
    }

    public override void Detach()
    {
        moveVelocity = 0;

        m_aCurrentAnimal.m_oCurrentObject = null;
        m_aCurrentAnimal.transform.parent = null;
        m_aCurrentAnimal.m_rBody.isKinematic = false;

        loris.m_bClimbing = false;
        loris.SetDirection(FACING_DIR.NONE);

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
            if (dist < closeDist || dist < 0)
            {
                index = i;
                closeDist = dist;
            }
        }
        return index;
    }
}