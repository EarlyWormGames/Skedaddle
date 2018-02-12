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

    [HideInInspector]
    public float moveVelocity;

    private float lastMove;
    private Animal animal;
    private MainMapping input;
    private Rigidbody rig;

    private int currentPoint;
    private Vector3 splinePos;

    // Use this for initialization
    void Start()
    {
        input = GameManager.Instance.GetComponent<PlayerInput>().GetActions<MainMapping>();
        rig = GetComponent<Rigidbody>();
        animal = GetComponent<Animal>();
    }

    // Update is called once per frame
    void Update()
    {
        float[] speed = animal.CalculateMoveSpeed();
        moveVelocity += input.moveX.value * Time.deltaTime * speed[0];
        moveVelocity = Mathf.Clamp(moveVelocity, speed[1], speed[2]);

        if (input.moveX.value == 0)
        {
            moveVelocity = Mathf.MoveTowards(moveVelocity, 0, DecelerationRate * Time.deltaTime);
            animal.m_bWalkingLeft = false;
            animal.m_bWalkingRight = false;
        }
        else if (input.moveX.value > 0)
        {
            animal.m_bWalkingRight = true;
            animal.m_bWalkingLeft = false;
        }
        else if (input.moveX.value < 0)
        {
            animal.m_bWalkingLeft = true;
            animal.m_bWalkingRight = false;
        }

        if ((moveVelocity < 0 && !animal.m_bFacingLeft) && animal.CanTurn())
        {
            animal.Turn(FACING_DIR.LEFT);
        }
        else if ((moveVelocity > 0 && animal.m_bFacingLeft) && animal.CanTurn())
        {
            animal.Turn(FACING_DIR.RIGHT);
        }

        Move(speed);
    }

    void Move(float[] a_speeds)
    {
        if (moveVelocity == 0)
            return;

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
            splinePos = FollowSpline.points[currentPoint].current;
            splinePos.y = transform.position.y;

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

            if (dir.magnitude < move)
            {
                if (moveVelocity < 0)
                    --currentPoint;
                else
                    ++currentPoint;

                if (currentPoint < 0 && FollowSpline.LowExit)
                    FollowSpline = null;
                else if (currentPoint > FollowSpline.points.Length - 1 && FollowSpline.HighExit)
                    FollowSpline = null;
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
        splinePos = FollowSpline.points[currentPoint].current;
        splinePos.y = transform.position.y;
    }

    public void StopSpline()
    {
        FollowSpline = null;
    }
}