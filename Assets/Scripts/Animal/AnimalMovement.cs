using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputNew;

[RequireComponent(typeof(Animal))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PlayerInput))]
public class AnimalMovement : MonoBehaviour
{
    public float DecelerationRate = 1;
    public float MoveMin = 0.00001f;
    public Transform ForwardDictator;
    public SplineMovement FollowSpline;

    private Animal animal;
    private MainMapping input;
    private float moveVelocity, lastMove;
    private Rigidbody rig;

    private int currentPoint;
    private Vector3 splinePos;

    // Use this for initialization
    void Start()
    {
        input = GetComponent<PlayerInput>().GetActions<MainMapping>();
        rig = GetComponent<Rigidbody>();
        animal = GetComponent<Animal>();
    }

    // Update is called once per frame
    void Update()
    {
        float[] speed = animal.CalculateMoveSpeed();
        moveVelocity += input.moveX.value * Time.deltaTime * speed[0];
        moveVelocity = Mathf.Clamp(moveVelocity, speed[1], speed[2]);
        moveVelocity = Mathf.MoveTowards(moveVelocity, 0, DecelerationRate * Time.deltaTime);

        Move(speed);
    }

    private void FixedUpdate()
    {
        
    }

    void Move(float[] a_speeds)
    {
        if (moveVelocity == 0)
            return;

        if (FollowSpline == null)
        {
            Vector3 newPoint = transform.position + (ForwardDictator.forward * moveVelocity);
            if (!TryMove(newPoint))
                moveVelocity = 0;
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

            float move = moveVelocity;
            if (move < 0)
                move *= -1;

            Vector3 dir = splinePos - transform.position;
            Vector3 moveDir = dir.normalized * move;

            TryMove(transform.position + moveDir);

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
        if (!rig.SweepTest(dir.normalized, out hit, dir.magnitude))
        {
            transform.position = point;
            return true;
        }
        return false;
    }

    public void SetSpline(SplineMovement spline)
    {
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