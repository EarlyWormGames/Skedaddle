using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputNew;

[RequireComponent(typeof(Animal))]
[RequireComponent(typeof(Rigidbody))]
public class AnimalMovement : MonoBehaviour
{
    public float DecelerationRate = 1;
    public float MoveMin = 0.00001f;
    public Transform ForwardDictator;
    public SplineMovement FollowSpline;

    public float RotateLerpSpeed = 60;

    public AxisAction MoveAxisKey;

    [HideInInspector]
    public float moveVelocity;

    private float lastMove;
    private Animal animal;
    private Rigidbody rig;

    private int currentPoint;
    private Vector3 splinePos;

    private AxisAction currentAxis;
    private float currentInput;

    // Use this for initialization
    void Start()
    {
        MoveAxisKey.Bind(GameManager.Instance.input.handle);
        rig = GetComponent<Rigidbody>();
        animal = GetComponent<Animal>();
    }

    // Update is called once per frame
    void Update()
    {
        if (FollowSpline == null)
            currentAxis = MoveAxisKey;
        else if (FollowSpline.MoveAxisKey.control == null)
            currentAxis = MoveAxisKey;
        else if (FollowSpline.MoveAxisKey.control != null)
            currentAxis = FollowSpline.MoveAxisKey;

        currentInput = currentAxis.control.value;
        if (FollowSpline != null)
        {
            if (FollowSpline.ForceMovement)
                currentInput = 1;
            currentInput *= FollowSpline.InvertAxis ? -1 : 1;
        }

        float[] speed = animal.CalculateMoveSpeed();
        moveVelocity += currentInput * Time.deltaTime * speed[0];
        moveVelocity = Mathf.Clamp(moveVelocity, speed[1], speed[2]);

        if (currentInput == 0)
            moveVelocity = Mathf.MoveTowards(moveVelocity, 0, DecelerationRate * Time.deltaTime);

        //Debug.Log("Move Velocity: " + moveVelocity);
        //Debug.Log("Rig Velocity: " + animal.m_rBody.velocity.x);

        Move(speed);
    }

    void Move(float[] a_speeds)
    {
        if (!animal.m_bSelected && FollowSpline == null)
            return;
        if (FollowSpline != null)
        {
            if (!FollowSpline.ForceMovement && !animal.m_bSelected)
                return;
        }

        if (currentInput == 0)
        {
            animal.m_bWalkingLeft = false;
            animal.m_bWalkingRight = false;
        }
        else if (currentInput > 0)
        {
            animal.m_bWalkingRight = true;
            animal.m_bWalkingLeft = false;
        }
        else if (currentInput < 0)
        {
            animal.m_bWalkingLeft = true;
            animal.m_bWalkingRight = false;
        }

        if (moveVelocity == 0)
            return;

        if ((currentInput < 0 && !animal.m_bFacingLeft) && animal.CanTurn())
        {
            animal.Turn(FACING_DIR.LEFT);
        }
        else if ((currentInput > 0 && animal.m_bFacingLeft) && animal.CanTurn())
        {
            animal.Turn(FACING_DIR.RIGHT);
        }

        if (!animal.CanMove())
        {
            moveVelocity = 0;
            return;
        }

        if (FollowSpline == null)
        {
            Vector3 newPoint = transform.position + (ForwardDictator.forward * moveVelocity);
            if (!TryMove(newPoint))
                moveVelocity = 0;
            Vector3 rotation = transform.eulerAngles;
            rotation.y = 90;
            rotation.z = 0;
            rotation.x = 0;
            transform.eulerAngles = rotation;
        }
        else
        {
            if (lastMove < 0 && moveVelocity > 0)
                ++currentPoint;
            else if (lastMove > 0 && moveVelocity < 0)
                --currentPoint;

            currentPoint = Mathf.Clamp(currentPoint, 0, FollowSpline.points.Length - 1);
            splinePos = FollowSpline.GetPosition(currentPoint);

            if (FollowSpline.IgnoreAxis.x > 0)
                splinePos.x = transform.position.x;
            if (FollowSpline.IgnoreAxis.y > 0)
                splinePos.y = transform.position.y;
            if (FollowSpline.IgnoreAxis.z > 0)
                splinePos.z = transform.position.z;

            Vector3 dir = splinePos - transform.position;
            float move = moveVelocity;
            if (move < 0)
                move *= -1;

            Vector3 moveDir = dir.normalized * move;

            TryMove(transform.position + moveDir);

            float rotateMult = 1;
            if (moveVelocity < 0)
                rotateMult = -1;

            Vector3 rotation = transform.eulerAngles;
            Vector3 newRotation = transform.eulerAngles;
            transform.forward = dir.normalized * rotateMult;
            newRotation.x = transform.eulerAngles.x;
            newRotation.y = transform.eulerAngles.y;

            transform.rotation = Quaternion.Lerp(Quaternion.Euler(rotation), Quaternion.Euler(newRotation), Time.deltaTime * RotateLerpSpeed);

            Vector3 forward = animal.transform.forward;

            if (animal.m_tJointRoot.localRotation.y > 180)
                forward *= -1;

            float dot = Vector3.Dot(dir, forward);

            if (dir.magnitude < move || ((dot < 0 && moveVelocity > 0) || (dot > 0 && moveVelocity < 0)))
            {
                if (moveVelocity < 0)
                    --currentPoint;
                else
                    ++currentPoint;

                if (currentPoint < 0 && FollowSpline.LowExit)
                    StopSpline();
                else if (currentPoint > FollowSpline.points.Length - 1 && FollowSpline.HighExit)
                    StopSpline();
            }
        }
        lastMove = moveVelocity;
    }

    /// <summary>
    /// Try to move the body to a position, sweepcasting along the way
    /// </summary>
    /// <param name="point"></param>
    /// <returns></returns>
    public bool TryMove(Vector3 point)
    {
        Vector3 dir = point - transform.position;
        RaycastHit hit;
        if (!rig.SweepTest(dir.normalized, out hit, dir.magnitude, QueryTriggerInteraction.Ignore))
        {
            transform.position = point;
            return true;
        }
        return false;
    }

    public void SetSpline(SplineMovement spline)
    {
        if (FollowSpline == spline)
            return;

        FollowSpline = spline;
        currentPoint = FollowSpline.GetClosestPoint(transform.position);

        if (FollowSpline.DisableGravity)
            animal.m_rBody.useGravity = false;
    }

    public void StopSpline()
    {
        FollowSpline = null;
        animal.m_rBody.useGravity = true;
    }
}