using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputNew;

public class LadderObject : AttachableInteract
{
    [Header("Exit Settings")]
    public bool LowExit = true;
    public bool HighExit = true;
    public Attachable TopTransition, BottomTransition;

    [Header("Movement")]
    public float RotateSpeed = 5;
    [EnumFlag] public IgnoreAxis AxesToIgnore = IgnoreAxis.X | IgnoreAxis.Z;
    public FACING_DIR Direction;
    public bool IsRope = false;

    [Tooltip("First = Bottom, Last = Top :: THESE MUST BE IN CORRECT ORDER!")]
    public Transform[] Points;
    public UnityEvent OnLowExit, OnHighExit;

    [Header("Misc")]
    public bool ForceMoveToClosest = true;
    public bool DisableCollision;
    public bool UseExitZ = true;
    public LayerMask ShimmyLayer = 1 << 11;

    [HideInInspector]
    public float moveVelocity;
    [HideInInspector]
    public float currentSpeed;

    [HideInInspector]
    public bool TryShimmyLeft, TryShimmyRight;

    private int pointIndex;
    private Loris loris;
    private bool moveDown = true, moveUp = true;
    private bool justEnter;
    private bool justExit;
    private float entryZ;

    protected override void OnStart()
    {
        base.OnStart();
        
        BlocksMovement = true;
        BlocksTurn = true;
    }

    protected override bool CheckInput(ActionSlot input, Animal caller)
    {
        if (justExit)
            return false;

        return KeyCheck(caller.transform);
    }

    protected override void OnUpdate()
    {
        justExit = false;
        if (loris == null)
            return;

        LadderObject shimmyLeft = GetShimmyObject(loris.transform.position, Vector3.left, loris.m_fShimmyDistance, loris.m_fShimmyBoxSize, Color.red);
        LadderObject shimmyRight = GetShimmyObject(loris.transform.position, Vector3.right, loris.m_fShimmyDistance, loris.m_fShimmyBoxSize, Color.blue);

        TryShimmyLeft = false;
        TryShimmyRight = false;

        if (shimmyLeft != null)
        {
            if (shimmyLeft.IsPointInRange(loris.transform.position.y) && GameManager.mainMap.leftButton.isHeld)
                TryShimmyLeft = true;
        }
        if (shimmyRight != null)
        {
            if (shimmyRight.IsPointInRange(loris.transform.position.y) && GameManager.mainMap.rightButton.isHeld)
                TryShimmyRight = true;
        }

        if (GameManager.mainMap.interact.wasJustPressed && !justEnter && AttachedAnimal.m_bSelected)
        {
            if (TryShimmyLeft)
            {
                Shimmy(shimmyLeft);
                return;
            }
            else if (TryShimmyRight)
            {
                Shimmy(shimmyRight);
                return;
            }

            Detach(this);
            return;
        }
        justEnter = false;

        float[] speed = loris.CalculateMoveSpeed();
        float acceleration = GameManager.mainMap.moveY.value * speed[0];
        moveVelocity += acceleration * Time.deltaTime;
        moveVelocity = Mathf.Clamp(moveVelocity, speed[1], speed[2]);

        if (GameManager.mainMap.moveY.value == 0)
        {
            moveVelocity = Mathf.MoveTowards(moveVelocity, 0, loris.m_fClimbStopSpeed * Time.deltaTime);
        }

        currentSpeed = (moveVelocity * Time.deltaTime) + (acceleration * Time.deltaTime * (Time.deltaTime / 2f));

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
        float move = currentSpeed;
        if (move < 0)
            move *= -1;

        Vector3 moveDir = dir.normalized * move;

        if (!DisableCollision)
            Debug.Log(loris.m_aMovement.TryMove(loris.transform.position + moveDir));
        else
            loris.transform.position += moveDir;

        float rotateMult = 1;
        if (currentSpeed < 0)
            rotateMult = -1;

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
            {
                Detach(this);
                OnLowExit.Invoke();
            }
            else if (pointIndex > Points.Length - 1 && !HighExit)
                moveUp = false;
            else if (pointIndex > Points.Length - 1 && HighExit)
            {
                Detach(this);
                OnHighExit.Invoke();
            }
        }
    }

    protected override void DoInteract(Animal caller)
    {
        if (!TryDetachOther())
            return;

        Attach(caller);

        AttachedAnimal.transform.SetParent(transform);
        AttachedAnimal.m_rBody.isKinematic = true;

        AttachedAnimal.m_aMovement.StopSpline();

        entryZ = AttachedAnimal.transform.position.z;

        if (DisableCollision)
            AttachedAnimal.SetColliderActive(false, this);

        loris = (Loris)AttachedAnimal;
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
        else
        {
            Vector3 pos = IgnoreUtils.Calculate(AxesToIgnore, loris.transform.position, Points[pointIndex].position);
            pos.y = loris.transform.position.y;
            loris.transform.position = pos;
        }

        justEnter = true;

        loris.SetDirection(Direction, true);
    }

    protected override void OnDetach(Animal animal)
    {
        //Base re-registers keys
        base.OnDetach(animal);

        justExit = true;

        moveVelocity = 0;

        AnimalsIn.RemoveAll(AttachedAnimal);
        
        AttachedAnimal.transform.parent = null;
        AttachedAnimal.m_rBody.isKinematic = false;

        if (UseExitZ)
        {
            Vector3 pos = AttachedAnimal.transform.position;
            pos.z = entryZ;
            AttachedAnimal.transform.position = pos;
        }

        if (DisableCollision)
            AttachedAnimal.SetColliderActive(true);

        loris.m_bClimbing = false;
        loris.SetDirection(FACING_DIR.NONE, false);

        if (IsRope)
            loris.m_bHorizontalRope = false;

        var temp = AttachedAnimal;
        AttachedAnimal = null;

        if (TopTransition != null && pointIndex > Points.Length - 1)
            TopTransition.AnimalEnter(temp);
        else if (BottomTransition != null && pointIndex < 0)
            BottomTransition.AnimalEnter(temp);

        loris = null;
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

    bool KeyCheck(Transform animal)
    {
        if (GameManager.mainMap.interact.isHeld)
            return true;

        if (animal.position.y <= Points[0].position.y)
        {
            if (GameManager.mainMap.moveY.positive.isHeld)
                return true;
        }
        else if (animal.position.y >= Points[Points.Length - 1].position.y)
        {
            if (GameManager.mainMap.moveY.negative.isHeld)
                return true;
        }
        else if (GameManager.mainMap.moveY.negative.isHeld || GameManager.mainMap.moveY.positive.isHeld)
            return true;

        return false;
    }

    public LadderObject GetShimmyObject(Vector3 position, Vector3 direction, float distance, Vector3 boxSize, Color debugColor)
    {
        if (loris == null)
            return null;

        position = position + (direction * distance);

        var colliders = Physics.OverlapBox(position, boxSize, Quaternion.LookRotation(direction), ShimmyLayer, QueryTriggerInteraction.Collide);
        ExtDebug.DrawBox(position, boxSize, Quaternion.LookRotation(direction), debugColor);

        float dist = -1;
        LadderObject shimmyObject = null;
        foreach (var collider in colliders)
        {
            var ladder = collider.GetComponent<LadderObject>();
            if (ladder == null)
            {
                if (collider.attachedRigidbody == null)
                    continue;
                ladder = collider.attachedRigidbody.GetComponent<LadderObject>();
            }
            if (ladder == null || ladder == this)
                continue;

            float d = Vector3.Distance(position, collider.bounds.center);
            if (d < dist || dist < 0)
            {
                dist = d;
                shimmyObject = ladder;
            }
        }

        return shimmyObject;
    }

    /// <summary>
    /// Check if a y position is within this ladder's range
    /// </summary>
    /// <param name="yPoint"></param>
    /// <returns></returns>
    public bool IsPointInRange(float yPoint)
    {
        if (yPoint < Points[0].position.y - 0.1f)
            return false;
        if (yPoint > Points[Points.Length - 1].position.y + 0.1f)
            return false;

        return true;
    }

    public void Shimmy(LadderObject shimmyObject)
    {
        if (shimmyObject == null)
            return;

        var temp = AttachedAnimal;
        Detach(this);
        shimmyObject.Interact(temp);
    }
}