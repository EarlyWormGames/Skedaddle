using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputNew;

/*! @brief Allows an Animal to move
* @details Attach this script to any GameObject that has an Animal script attached.  
    This will allow for linear movement, as well as following a SplineMovement.
    <br>It also has many helpful features, such as the @ref MoveWithGround property, which will allow
    an Animal to stick to the current Collider the Animal is sitting on.
*/
[RequireComponent(typeof(Animal))]
[RequireComponent(typeof(Rigidbody))]
public class AnimalMovement : MonoBehaviour
{
    public float DecelerationRate = 1;

    /*  @brief The forward direction for the Animal to follow
        @details The Animal will follow the forward of this Transform when not
        following a SplineMovement object.
    */
    public Transform ForwardDictator;

    /*  @brief The current spline to follow
        @details The AnimalMovement will follow this SplineMovement while it is not null.
    */
    public SplineMovement FollowSpline;

    public float RotateLerpSpeed = 5;
    public float SweepYAdd = 0.1f;
    public AxisAction MoveAxisKey;

    /*  @brief Sets if the Animal will move with the Collider it is sitting on
        @details Enabling this will cause this script to raycast beneath the Animal every FixedUpdate
        and get the current Collider it is sitting on.
        <br>Every FixedUpdate the Collider remains the same,
        the Animal will move the same amount in the same direction that the current Collider moves
    */
    [Header("Ground Movement")]
    [Tooltip("Will the animal move when the ground moves?")]
    public bool MoveWithGround = true;

    /*  @brief Sets if the Animal will rotate with the Collider it is sitting on
        @details This has a similar function to @ref MoveWithGround, except allowing it to rotate
    */
    [Tooltip("Will the animal rotate when the ground rotates?")]
    public bool RotateWithGround = true;

    [EnumFlag] public IgnoreAxis RotateIgnore = IgnoreAxis.X | IgnoreAxis.Z;
    public LayerMask GroundLayers;
    public float RaycastDistance = 0.1f;

    /*  @brief The current movement velocity of the Animal
        @details This value is relative to either:
        -The forward direction, determined by @ref ForwardDictator
        -The direction the Animal is moving along the @ref FollowSpline
    */
    [HideInInspector]
    public float moveVelocity;
    [HideInInspector]
    public float currentSpeed;

    private float lastMove;
    private Animal animal;
    private Rigidbody rig;

    private int currentPoint;
    private Vector3 splinePos;

    private AxisAction currentAxis;
    private float currentInput;

    private Collider groundCollider;
    private Vector3 lastPos;
    private Vector3 lastRot;

    private bool canMoveForward = true, canMoveBackward = true;

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
        if (groundCollider != null)
        {
            if (MoveWithGround)
            {
                Vector3 dir = groundCollider.transform.position - lastPos;
                transform.position += dir;
                lastPos = groundCollider.transform.position;
            }
            if (RotateWithGround)
            {
                Vector3 dir = groundCollider.transform.eulerAngles - lastRot;
                transform.eulerAngles = IgnoreUtils.Calculate(RotateIgnore, transform.eulerAngles, transform.eulerAngles + dir);
                lastRot = groundCollider.transform.eulerAngles;
            }
        }

        if (FollowSpline == null)
            currentAxis = MoveAxisKey;
        else if (FollowSpline.MoveAxisKey.control == null)
            currentAxis = MoveAxisKey;
        else if (FollowSpline.MoveAxisKey.control != null)
            currentAxis = FollowSpline.MoveAxisKey;

        currentInput = currentAxis.control.value;

        animal.m_bForceWalk = false;
        animal.m_gqGrounder.enabled = true;

        if (FollowSpline != null)
        {
            if (FollowSpline.ForceMovement)
            {
                currentInput = 1;
                animal.m_bForceWalk = true;
            }
            if (FollowSpline.DisableGrounder)
                animal.m_gqGrounder.enabled = false;

            currentInput *= FollowSpline.InvertAxis ? -1 : 1;

            if (!canMoveForward && currentInput > 0)
                currentInput = 0;
            else if (!canMoveForward && currentInput < 0)
                canMoveForward = true;

            if (!canMoveBackward && currentInput < 0)
                currentInput = 0;
            else if (!canMoveBackward && currentInput > 0)
                canMoveBackward = true;
        }

        float[] speed = animal.CalculateMoveSpeed();
        float acceleration = currentInput * speed[0];
        moveVelocity += acceleration * Time.deltaTime;
        moveVelocity = Mathf.Clamp(moveVelocity, speed[1], speed[2]);

        if (currentInput == 0)
            moveVelocity = Mathf.MoveTowards(moveVelocity, 0, DecelerationRate * Time.deltaTime);

        currentSpeed = (moveVelocity * Time.deltaTime) + (acceleration * Time.deltaTime * (Time.deltaTime / 2f));

        //Debug.Log("Move Velocity: " + moveVelocity);
        //Debug.Log("Rig Velocity: " + animal.m_rBody.velocity.x);

        Move(speed);
    }

    private void FixedUpdate()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, -transform.up, out hit, RaycastDistance, GroundLayers))
        {
            if (hit.collider != groundCollider)
            {
                groundCollider = hit.collider;
                lastPos = groundCollider.transform.position;
            }
        }
        else
        {
            groundCollider = null;
        }
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
            Vector3 newPoint = transform.position + (ForwardDictator.forward * currentSpeed);
            if (!TryMove(newPoint, SweepYAdd))
                moveVelocity = 0;

            Vector3 rotation = transform.eulerAngles;
            rotation.x = 0;
            rotation.z = 0;
            rotation.y = 90;
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

            splinePos = IgnoreUtils.Calculate(FollowSpline.AxesToIgnore, transform.position, splinePos);

            Vector3 dir = splinePos - transform.position;
            float move = currentSpeed;
            if (move < 0)
                move *= -1;

            Vector3 moveDir = dir.normalized * move;

            TryMove(transform.position + moveDir, SweepYAdd);

            float rotateMult = 1;
            if (currentSpeed < 0)
                rotateMult = -1;

            Vector3 rotation = transform.eulerAngles;
            Vector3 newRotation = Quaternion.LookRotation(dir.normalized * rotateMult, Vector3.up).eulerAngles;           
            newRotation.z = transform.eulerAngles.z;

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

                if (currentPoint < 0)
                {
                    if (FollowSpline.LowExit)
                    {
                        StopSpline();
                        FollowSpline.OnLowExit.Invoke();
                    }
                    else
                        canMoveBackward = false;
                }
                else if (currentPoint > FollowSpline.points.Length - 1)
                {
                    if (FollowSpline.HighExit)
                    {
                        StopSpline();
                        FollowSpline.OnHighExit.Invoke();
                    }
                    else
                        canMoveForward = false;
                }
            }
        }
        lastMove = moveVelocity;
    }

    /// <summary>
    /// Try to move the body to a position, sweepcasting along the way
    /// </summary>
    /// <param name="point">The point to move the Animal to</param>
    /// <param name="addY">The point to move the Animal to</param>
    /// <returns>Returns true when no collision occurs and the Animal has moved</returns>
    public bool TryMove(Vector3 point, float addY = 0.01f)
    {
        Vector3 dir = point - transform.position;
        RaycastHit hit;
        Vector3 v3Temp = transform.position;
        v3Temp.y += addY;
        transform.position = v3Temp;

        if (!rig.SweepTest(dir.normalized, out hit, dir.magnitude, QueryTriggerInteraction.Ignore))
        {
            transform.position = point;
            return true;
        }
        v3Temp.y -= addY;
        transform.position = v3Temp;
        return false;
    }

    /// <summary>
    /// Sets the current SplineMovement for this object to follow
    /// </summary>
    /// <param name="spline">The SplineMovement to follow</param>
    public void SetSpline(SplineMovement spline)
    {
        if (FollowSpline == spline)
            return;

        FollowSpline = spline;
        currentPoint = FollowSpline.GetClosestPoint(transform.position);

        canMoveBackward = true;
        canMoveForward = true;

        if (FollowSpline.DisableGravity)
            animal.m_rBody.useGravity = false;
    }

    /// <summary>
    /// Stop this object from following a spline and reset any values
    /// </summary>
    public void StopSpline()
    {
        FollowSpline = null;
        animal.m_rBody.useGravity = true;
    }
}