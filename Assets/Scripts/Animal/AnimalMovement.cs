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
    private float splinePos;

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

        Move();
    }

    private void FixedUpdate()
    {
        
    }

    void Move()
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
            float oldSplinePos = splinePos;
            
            splinePos = Mathf.Clamp(splinePos + moveVelocity, 0, FollowSpline.MaxSplineLength);            
            var point = FollowSpline.GetPoint(splinePos / FollowSpline.MaxSplineLength);
            point.y = transform.position.y;

            Vector3 dir = point - transform.position;
            if (!TryMove(point))
            {
                splinePos = oldSplinePos;
                moveVelocity = 0;
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
}