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
    private float moveVelocity;
    private Rigidbody rig;

    private float splineDist;
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
            if (transform.position == splinePos)
            {
                float oldSplineDist = splineDist;
                splinePos = FollowSpline.GetPointAtDist(oldSplineDist + moveVelocity);
                splinePos.y = transform.position.y;

                float move = moveVelocity;
                float mult = 1;
                if (move < 0)
                {
                    mult = -1;
                }
                move *= mult;

                Vector3 dir = splinePos - transform.position;
                dir = Vector3.ClampMagnitude(dir, move);
                splineDist += dir.magnitude * mult;

                if (!TryMove(transform.position + dir))
                {
                    splineDist = oldSplineDist;
                    moveVelocity = 0;
                    splinePos = transform.position;
                }
                else
                {
                    if (splineDist <= MoveMin && FollowSpline.LowExit)
                        FollowSpline = null;
                    else if (FollowSpline.HighExit && splineDist >= FollowSpline.MaxLength - MoveMin)
                        FollowSpline = null;
                }
            }
            else
            {
                splinePos.y = transform.position.y;
                float move = moveVelocity;
                if (move < 0)
                    move *= -1;

                Vector3 dir = splinePos - transform.position;
                dir = Vector3.ClampMagnitude(dir, move);
                Vector3 point = transform.position + dir;

                if (!TryMove(point))
                {
                    splinePos = transform.position;
                    moveVelocity = 0;
                }
            }
        }
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

        int index = FollowSpline.GetClosestPoint(transform.position);
        splineDist = FollowSpline.points[index].totalDistance;
        splinePos = FollowSpline.points[index].current;
        splinePos.y = transform.position.y;
    }

    public void StopSpline()
    {
        FollowSpline = null;
    }
}