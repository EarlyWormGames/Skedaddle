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
    public float MoveMin = 0.01f;
    public Transform ForwardDictator;
    public BezierSpline FollowSpline;

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
                float oldSplinePos = splineDist;

                splineDist = Mathf.Clamp(splineDist + moveVelocity, 0, FollowSpline.MaxSplineLength);
                splinePos = FollowSpline.GetPoint(splineDist / FollowSpline.MaxSplineLength);
                splinePos.y = transform.position.y;
                
                if (!TryMove(splinePos))
                {
                    splineDist = oldSplinePos;
                    moveVelocity = 0;
                    splinePos = transform.position;
                }
            }
            else
            {
                splinePos.y = transform.position.y;

                Vector3 dir = splinePos - transform.position;
                dir = Vector3.ClampMagnitude(dir, a_speeds[2]);
                Vector3 point = transform.position + dir;

                TryMove(point);
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

    public void SetSpline(BezierSpline spline)
    {
        FollowSpline = spline;
        splineDist = FollowSpline.GetArcLength(transform.position, true);
        splinePos = FollowSpline.GetPoint(splineDist / FollowSpline.MaxSplineLength);
        splinePos.y = transform.position.y;
    }
}