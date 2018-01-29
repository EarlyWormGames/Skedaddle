using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Analytics;
using System;
using System.Collections.Generic;
using RootMotion.FinalIK;

public enum DEATH_TYPE
{
    FALL,
    ELECTRICITY,
    SQUASH,
}

public enum ANIMAL_SIZE
{
    NONE,
    XS,
    S,
    M,
    L,
    XL
}

public enum ANIMAL_NAME
{
    NONE = 0,
    LORIS = 1,
    POODLE = 2,
    ANTEATER = 3,
    ZEBRA = 4,
    //I WANT MY
    ELEPHANT = 5
}

public class Animal : MonoBehaviour
{
    [Serializable]
    public class NamedEvent
    {
        public SOUND_EVENT m_Event;
        public UnityEvent m_eFunction;
    }

    public enum SOUND_EVENT
    {
        FALL_START,
        FALL_END,
        FALL_STOP,
        FALL_DEATH,
        ELECTRO_DEATH,
    }

    //==================================
    //          Public Vars
    //      (Used for Inspector)
    //==================================
    public Transform        m_tCameraPivot;
    public Transform        m_tLeverReference;
    public AnimationExtras  m_eExtras;
    public GameObject       m_goIKSwitch;
    public Transform        m_tJointRoot;
    public Transform        m_tCollider;

    [Header("Speeds")]
    public float            m_fWalkSpeed = 0.001f;
    public float            m_fTopSpeed = 0.05f;
    public float            m_fPushTopMult = 0.6f;
    public float            m_fDecelerateSpeed = 0.001f;
    public float            m_fPullSpeedMult = 0.5f;
    public float            m_fFallSpeedMult = 0.5f;
    public float            m_fRunSpeedMult = 1.5f;
    public float            m_fTurnSpeedMult = 0.95f;

    public float            m_fTurnAxis;

    public float            m_fRotSpeed = 1f;
    public float            m_fSplineRotSpeed = 1f;

    [Header("Climb Jump")]
    public float            m_fJumpWaitTime = 1f;
    public float            m_fJumpLerpSpeed = 2f;
    public float            m_fJumpWaitFrames = 1;

    [Header("Raycast")]
    public float            m_fGroundRaycastDist = 1f;
    public float            m_fRaycastWaitTime = 0f;
    public LayerMask        m_lGroundCheckMask;
    public Transform        m_atGroundCheckPoints;
    public Transform        m_atPushOffCheckPoint;
    public Vector3          m_v3WalkingOffset;
    public Vector3          m_v3WalkingSize;
    public float            m_fWalkingDistance;

    [Header("Eyetracking")]
    [Tooltip("How far left/right eyes need to be looking to trigger movement")]
    public float            m_XBuffer = 1;
    [Tooltip("How far up/down eyes need to be looking to trigger movement")]
    public float            m_YBuffer = 1;

    [Header("Misc")]
    public float            m_fMaxFallDist = 1f;
    public float            m_fHitResist = 1f;
    public float            m_fSitTime = 2f;
    public float            m_fCameraY = 0;
    public Vector3          m_v3DefaultForward;
    public Vector3          m_v3DeathDistance = new Vector3(0, 0.5f, -2.5f);

    public DEATH_TYPE[]     m_NoDeaths;

    [Header("Sounds")]
    public NamedEvent[]     m_aSoundEvents;

    //==================================
    //          Internal Vars
    //    (Public use for scripts)
    //==================================
    internal ANIMAL_SIZE m_eSize;
    internal ANIMAL_NAME m_eName;

    //The object it's currently interactive with (only set while ACTUALLY INTERACTING)
    internal FACING_DIR     m_fFacingDir;

    //Helpers for objects
    internal ActionObject   m_oCurrentObject;
    internal bool           m_bPullingObject;

    internal Animator       m_aAnimalAnimator;

    internal bool           m_bCanWalkLeft = true;
    internal bool           m_bCanWalkRight = true;

    internal bool           m_bFalseUp;
    internal bool           m_bFalseDown;

    internal bool           m_bWallOnRight;
    internal bool           m_bWallOnLeft;

    internal bool           m_bOnGround = false;
    internal bool           m_bCheckGround = true;

    internal Rigidbody      m_rBody;
    internal Collider       m_cCol;

    internal bool           m_bSelected = false;

    internal bool           m_bPullingPlug = false;
    internal bool           m_bPullingLeverOn = false;
    internal bool           m_bPullingLeverOff = false;

    internal bool           m_bDigging = false;

    internal bool           m_bCanRotate = true;
    internal bool           m_bAllowAutoRotate = true;
    internal bool           m_bAutoClimbing = false;

    internal bool           m_bOnTrampoline;
    internal bool           m_bBouncingOnTrampoline;
    internal bool           m_bOnSlope;

    internal bool           m_bCanBeSelected = true;

    internal float          m_fAnimationSpeed;
    internal float          m_fAnimationLength;

    internal float          m_fFallStartY = 0f;
    internal bool           m_bTurning = false;
    internal float          m_fTurnStartRotation = 0f;
    internal bool           m_bFinishTurn = false;
    internal bool           m_bRunning = false;

    internal bool           m_bCheckForFall = true;
    internal Vector3        m_v3ForwardTarg;
    internal Vector3        m_v3UpTarget;
    internal bool           m_bUseIK = true;

    internal bool           m_bForceWalk = false;

    internal bool Alive
    {
        get { return m_bAlive; }
    }

    internal bool           m_bTurned = false;

    internal bool           m_EyeLeft = false;
    internal bool           m_EyeRight = false;
    internal bool           m_EyeUp = false;
    internal bool           m_EyeDown = false;
    internal EWGazeObject   m_GazeObject;
    internal GrounderQuadruped m_gqGrounder;
    internal Vector3        m_v3MoveVelocity;

    //==================================
    //          Protected Vars
    //================================== 
    protected bool          m_bAlive = true;
    
    //Movement
    protected float         m_fCurrentSpeed;
    protected PathFollower  m_pFollower;

    protected bool          m_bFacingLeft = false;
    protected bool          m_bWalkingLeft = false;
    protected bool          m_bWalkingRight = false;
    protected bool          m_bLeanUp = false;
    protected bool          m_bLeanDown = false;
    protected bool          m_bFrontGroundDetected = false;
    protected bool          m_bBackGroundDetected = false;
    protected bool          m_bRestGroundDetected = false;
    protected bool          m_bPushOffDetected = false;
    




    protected float         m_fSelectionTimer = 0;
    protected float         m_fSitTimer = -1f;

    protected bool          m_bEyetrackSelected = false;
    protected bool          m_bGazeRunning = false;
    protected float         m_fGazeTimer = 0f;

    protected Dictionary<Collider, Type> m_dChildColliders;


    //==================================
    //          Private Vars
    //==================================
    private float           m_fRaycastTimer = 0f;
    private float           m_fIKMaxWeight;
    

    private float           m_fTurnAngle;

    private bool            m_bUseXTarget = false;
    private bool            m_bUseYTarget = false;
    private bool            m_bUseZTarget = false;
    private float           m_fXTarget;
    private float           m_fYTarget;
    private float           m_fZTarget;

    void Awake()
    {
        m_aAnimalAnimator = GetComponentInChildren<Animator>();

        OnAwake();
    }
    // Use this for initialization
    void Start()
    {
        m_rBody = GetComponent<Rigidbody>();
        m_cCol = GetComponent<Collider>();
        m_pFollower = GetComponent<PathFollower>();
        m_gqGrounder = m_goIKSwitch.GetComponent<GrounderQuadruped>();
        m_fFallStartY = transform.position.y;

        m_fCurrentSpeed = m_fWalkSpeed;
        m_fIKMaxWeight = m_gqGrounder.weight;
        m_fSitTimer = m_bSelected ? -1 : 0;

        m_GazeObject = GetComponent<EWGazeObject>();

        m_dChildColliders = new Dictionary<Collider, Type>();
        var animalCol = GetComponentInChildren<AnimalCollider>();
        if (animalCol != null)
        {
            Collider[] cols = animalCol.GetComponents<Collider>();
            foreach (var col in cols)
            {
                m_dChildColliders.Add(col, col.GetType());
            }
        }

        OnStart();
    }

    // Update is called once per frame
    void Update()
    {
        if (!m_bAlive)
        {
            m_rBody.useGravity = true;
            m_rBody.isKinematic = false;
            gameObject.layer = LayerMask.NameToLayer("Animal");

        }

        //=================================================
        #region EYETRACKING
        if (EWEyeTracking.active)
        {
            EWGazeObject selectedObj = EWEyeTracking.GetFocusedObject();
            float direction = 0f;
            if (selectedObj != null)
            {
                if (selectedObj.IsActionObject)
                    direction = selectedObj.transform.position.x - transform.position.x;
            }

            if (EWEyeTracking.worldPosition.x > transform.position.x + m_XBuffer || direction > 0)
                m_EyeRight = true;
            else
                m_EyeRight = false;

            if (EWEyeTracking.worldPosition.x < transform.position.x - m_XBuffer || direction < 0)
                m_EyeLeft = true;
            else
                m_EyeLeft = false;

            if (EWEyeTracking.worldPosition.y > transform.position.y + m_YBuffer)
                m_EyeUp = true;
            else
                m_EyeUp = false;

            if (EWEyeTracking.worldPosition.y < transform.position.y - m_YBuffer)
                m_EyeDown = true;
            else
                m_EyeDown = false;
        }

        if (EWEyeTracking.GetFocusedObject() == m_GazeObject && m_GazeObject != null)
        {
            Highlighter.Selected = gameObject;

            if (m_oCurrentObject != null)
            {
                SetTimer(true);

                if (m_fGazeTimer >= EWEyeTracking.holdTime)
                {
                    m_oCurrentObject.Detach();
                    SetTimer(false);
                }
            }
            else
                SetTimer(false);
        }
        else
            SetTimer(false);

        if (m_bGazeRunning)
            m_fGazeTimer += Time.deltaTime;
        #endregion
        //=================================================

        //=================================================
        #region ROTATION
        if (m_bAllowAutoRotate)
        {
            if (m_bTurning)
            {
                //if (m_aAnimalAnimator.GetFloat("Root_Curve_Y") >= 1f)
                //    m_bFinishTurn = true;
                if (!m_bFinishTurn)
                    m_tJointRoot.Rotate(new Vector3(0, m_fAnimationSpeed * m_fTurnAngle * m_fAnimationLength * Time.deltaTime, 0));
            }

            if (m_fFacingDir != FACING_DIR.NONE)
            {
                Quaternion rot = Quaternion.identity;
                switch (m_fFacingDir)
                {
                    case FACING_DIR.FRONT:
                        {
                            rot = Quaternion.Euler(0, 180, 0);
                            break;
                        }
                    case FACING_DIR.RIGHT:
                        {
                            rot = Quaternion.Euler(0, 90, 0);
                            m_bTurned = false;
                            break;
                        }
                    case FACING_DIR.BACK:
                        {
                            rot = Quaternion.Euler(0, 0, 0);
                            break;
                        }
                    case FACING_DIR.LEFT:
                        {
                            rot = Quaternion.Euler(0, 270, 0);
                            m_bTurned = true;
                            break;
                        }
                }

                m_tJointRoot.rotation = Quaternion.Lerp(m_tJointRoot.rotation, rot, Time.deltaTime * m_fRotSpeed);
            }
            else if (!m_bTurning)
            {
                Quaternion rot = Quaternion.Euler(m_tJointRoot.localRotation.eulerAngles.x, m_bTurned ? 180 + m_fTurnAxis : 0 + m_fTurnAxis, m_tJointRoot.localRotation.eulerAngles.z);
                m_tJointRoot.localRotation = Quaternion.Lerp(m_tJointRoot.localRotation, rot, Time.deltaTime * m_fRotSpeed);
            }
        }
        else
        {
            Vector3 rot = Quaternion.LookRotation(m_v3ForwardTarg.normalized).eulerAngles;
            rot.z = m_v3UpTarget.z;
            m_tJointRoot.rotation = Quaternion.Lerp(m_tJointRoot.rotation, Quaternion.Euler(rot), Time.deltaTime * m_fRotSpeed);
        }
        #endregion
        //=================================================

        if (m_tJointRoot != null)
        {
            m_tCollider.rotation = m_tJointRoot.rotation;
        }

        //IK sWitch
        if (m_aAnimalAnimator.GetFloat("IKSwitch") > 0 && m_bUseIK)
        {
            m_goIKSwitch.SetActive(true);
            m_gqGrounder.weight = Mathf.Lerp(m_gqGrounder.weight,m_fIKMaxWeight, Time.deltaTime * 1.5f);
        }
        else if (m_aAnimalAnimator.GetFloat("IKSwitch") == 0 && m_bUseIK)
        {
            m_goIKSwitch.SetActive(true);
            m_gqGrounder.weight = 0.001f;
        }
        else
        {
            m_gqGrounder.weight = 0.001f;
            m_goIKSwitch.SetActive(false);
        }

        if (m_fSitTimer >= 0f)
        {
            m_fSitTimer += Time.deltaTime;
            if (m_fSitTimer >= m_fSitTime)
            {
                m_fSitTimer = -1f;
                m_aAnimalAnimator.SetBool("Sit", true);
            }
        }

        if (m_bWalkingRight && m_rBody.velocity.x < 0.001f && m_rBody.velocity.x > -0.001f)
        {
            m_bWallOnRight = true;
        }
        else
        {
            m_bWallOnRight = false;
        }
        if (m_bWalkingLeft && m_rBody.velocity.x < 0.001f && m_rBody.velocity.x > -0.001f)
        {
            m_bWallOnLeft = true;
        }
        else
        {
            m_bWallOnLeft = false;
        }
        if (m_rBody != null)
        {
            m_rBody.WakeUp();
        }

        if (m_bSelected)
        {
            m_fSelectionTimer += Time.deltaTime;
        }
        else
        {
            m_fSelectionTimer = 0;
        }

        m_fRaycastTimer -= Time.deltaTime;
        if (m_fRaycastTimer <= 0f && m_bCheckGround)
        {
            m_fRaycastTimer = m_fRaycastWaitTime;
            RaycastHit hitInfo;
            if (Physics.Raycast(m_atGroundCheckPoints.position, -transform.up, out hitInfo, m_fGroundRaycastDist, m_lGroundCheckMask))
            {
                m_bRestGroundDetected = true;
            }
            else
            {
                m_bRestGroundDetected = false;
                
            }
            if (m_atPushOffCheckPoint != null)
            {
                if (Physics.Raycast(m_atPushOffCheckPoint.position, -transform.up, out hitInfo, m_fGroundRaycastDist, m_lGroundCheckMask))
                {
                    m_bPushOffDetected = true;
                }
                else
                {
                    m_bPushOffDetected = false;
                }
            }


            bool onGround = false;
            if (m_gqGrounder.forelegSolver.isGrounded)
            {
                if (m_gqGrounder.forelegSolver.currentGroundLayer != LayerMask.NameToLayer("Grounder"))
                {
                    m_bFrontGroundDetected = true;
                }
                else
                {
                    m_bFrontGroundDetected = false;
                }
            }
            else
            {
                m_bFrontGroundDetected = false;
            }
            m_bBackGroundDetected = m_gqGrounder.solver.isGrounded;

            if(m_bFrontGroundDetected || m_bBackGroundDetected)
            {
                onGround = true;
            }
            else
            {
                onGround = false;
            }

            if (onGround)
            {
                if (m_bOnGround == false)
                {
                    //Was falling
                    float dist = m_fFallStartY - transform.position.y;



                    if (dist > m_fMaxFallDist && !m_bOnTrampoline)
                    {
                        Kill(DEATH_TYPE.FALL);
                        PlaySound(SOUND_EVENT.FALL_STOP);
                        PlaySound(SOUND_EVENT.FALL_DEATH);
                    }
                    else
                    {
                        PlaySound(SOUND_EVENT.FALL_END);
                    }
                }

                m_bOnGround = true;
                m_fCurrentSpeed = m_fWalkSpeed;
            }
            else if (m_bCheckForFall)
            {
                if (m_bOnGround == true || m_fFallStartY < transform.position.y)
                {
                    m_fFallStartY = transform.position.y;
                }

                if (m_bOnGround == true)
                {
                    PlaySound(SOUND_EVENT.FALL_START);
                }

                m_bOnGround = false;

            }
        }
        OnUpdate();
        Move();
    }

    void FixedUpdate()
    {
        OnFixedUpdate();
        MoveRig();
    }

    public void ForceTurn(bool a_facingLeft)
    {
        m_bTurned = a_facingLeft;

        m_bFacingLeft = m_bTurned;

        m_tJointRoot.localRotation = Quaternion.Euler(new Vector3(m_tJointRoot.localRotation.eulerAngles.x, m_bTurned ? 180 + m_fTurnAxis : 0 + m_fTurnAxis, m_tJointRoot.localRotation.eulerAngles.z));
        m_fTurnStartRotation = m_tJointRoot.eulerAngles.y;
    }

    protected virtual void Move()
    {
        if (!m_bUseXTarget && !m_bUseYTarget && !m_bUseZTarget)
        {
            if (m_bSelected && m_bAlive)
            {
                m_bWalkingLeft = false;
                m_bWalkingRight = false;

                Vector2 stickAmount = Controller.GetStick(true);

                if (m_bCanWalkLeft)
                {
                    if ((Keybinding.GetKey("MoveLeft") || Controller.GetDpad(ControllerDpad.Left) || stickAmount.x < -0.2f || m_EyeLeft) && !(Keybinding.GetKey("MoveRight") || Controller.GetDpad(ControllerDpad.Right) || stickAmount.x > 0.2f || m_EyeRight))
                    {
                        //m_pFollower.FollowPath(FACING_DIR.LEFT, m_fCurrentSpeed, m_rBody, m_fFacingDir);

                        if (!m_bPullingObject)
                        {
                            if (!m_bTurning)
                            {
                                if (m_bTurned)
                                {
                                    Vector3 force = m_tCollider.transform.forward * m_fCurrentSpeed * (m_bOnGround ? 1 : m_fFallSpeedMult) * (stickAmount.x < -0.2f ? -stickAmount.x : 1);
                                    m_v3MoveVelocity += force;
                                }
                                else
                                {
                                    Vector3 force = Vector3.left * m_fCurrentSpeed * (m_bRunning ? 0 : m_fTurnSpeedMult);
                                    m_v3MoveVelocity += force;
                                    Turn(180);
                                }
                            }
                            else
                            {
                                Vector3 force = Vector3.left * m_fCurrentSpeed * (m_bRunning ? 0 : m_fTurnSpeedMult);
                                m_v3MoveVelocity+= force;
                                if(!m_bTurned)Turn(180);
                            }
                        }
                        else
                        {
                            Vector3 force = new Vector3(-Mathf.Cos(m_fTurnAxis), 0, -Mathf.Sin(m_fTurnAxis)) * m_fCurrentSpeed * m_fPullSpeedMult * (m_bOnGround ? 1 : m_fFallSpeedMult) * (stickAmount.x < -0.2f ? -stickAmount.x : 1);
                            m_v3MoveVelocity += (force) * Time.deltaTime;
                        }
                        m_bWalkingLeft = true;
                    }
                }
                if (m_bCanWalkRight)
                {
                    if ((Keybinding.GetKey("MoveRight") || Controller.GetDpad(ControllerDpad.Right) || stickAmount.x > 0.2f || m_EyeRight) && !(Keybinding.GetKey("MoveLeft") || Controller.GetDpad(ControllerDpad.Left) || stickAmount.x < -0.2f || m_EyeLeft))
                    {
                        //m_pFollower.FollowPath(FACING_DIR.RIGHT, m_fCurrentSpeed, m_rBody, m_fFacingDir);

                        if (!m_bPullingObject)
                        {
                            if (!m_bTurning)
                            {
                                if (!m_bTurned)
                                {
                                    Vector3 force = m_tCollider.transform.forward * m_fCurrentSpeed * (m_bOnGround ? 1 : m_fFallSpeedMult) * (stickAmount.x > 0.2f ? stickAmount.x : 1);
                                    m_v3MoveVelocity += force;
                                }
                                else
                                {
                                    Vector3 force = Vector3.right * m_fCurrentSpeed * (m_bRunning ? 0 : m_fTurnSpeedMult);
                                    m_v3MoveVelocity += force;
                                    Turn(180);
                                }
                            }
                            else
                            {
                                
                                Vector3 force = Vector3.right * m_fCurrentSpeed * (m_bRunning ? 0 : m_fTurnSpeedMult);
                                m_v3MoveVelocity += force;
                                if (m_bTurned)
                                    Turn(180);
                            }
                        }
                        else
                        {
                            Vector3 force = new Vector3(Mathf.Cos(m_fTurnAxis),0, Mathf.Sin(m_fTurnAxis)) * m_fCurrentSpeed * m_fPullSpeedMult * (m_bOnGround ? 1 : m_fFallSpeedMult) * (stickAmount.x > 0.2f ? stickAmount.x : 1);
                            m_v3MoveVelocity += force;
                        }
                        m_bWalkingRight = true;
                    }
                }
                if (m_bFalseUp)
                {
                    if (Keybinding.GetKey("MoveUp") || Controller.GetDpad(ControllerDpad.Up) || stickAmount.y > 0.2f || m_EyeUp)
                    {
                        m_bLeanUp = true;
                    }
                    else
                    {
                        m_bLeanUp = false;
                    }
                }
                else
                {
                    m_bLeanUp = false;
                }
                if (m_bFalseDown)
                {
                    if (Keybinding.GetKey("MoveDown") || Controller.GetDpad(ControllerDpad.Down) || stickAmount.y < 0.2f || m_EyeDown)
                    {
                        m_bLeanDown = true;
                    }
                    else
                    {
                        m_bLeanDown = false;
                    }
                }
                else
                {
                    m_bLeanDown = false;
                }
            }
        }
        else
        {
            if (m_bUseXTarget)
            {
                if (transform.position.x < m_fXTarget - 0.008f)
                {
                    m_rBody.AddForce(-Vector3.left * m_fCurrentSpeed);
                    m_bWalkingRight = true;
                    m_bFacingLeft = false;
                }
                else if (transform.position.x > m_fXTarget + 0.008f)
                {
                    m_rBody.AddForce(Vector3.left * m_fCurrentSpeed);
                    m_bWalkingLeft = true;
                    m_bFacingLeft = true;
                }
                else
                {
                    m_rBody.velocity = new Vector3(0, m_rBody.velocity.y, m_rBody.velocity.z);
                    m_bUseXTarget = false;
                }
            }
            if (m_bUseYTarget)
            {
                if (transform.position.y < m_fYTarget - 0.008f)
                {
                    m_rBody.AddForce(Vector3.up * m_fCurrentSpeed);
                    m_bWalkingRight = true;
                    m_bFacingLeft = false;
                }
                else if (transform.position.y > m_fYTarget + 0.008f)
                {
                    m_rBody.AddForce(Vector3.down * m_fCurrentSpeed);
                    m_bWalkingLeft = true;
                    m_bFacingLeft = true;
                }
                else
                {
                    m_rBody.velocity = new Vector3(m_rBody.velocity.x, 0, m_rBody.velocity.z);
                    m_bUseYTarget = false;
                }
            }
            if (m_bUseZTarget)
            {
                if (transform.position.z < m_fYTarget - 0.008f)
                {
                    m_rBody.AddForce(Vector3.back * m_fCurrentSpeed);
                    m_bWalkingRight = true;
                    m_bFacingLeft = false;
                }
                else if (transform.position.z > m_fYTarget + 0.008f)
                {
                    m_rBody.AddForce(Vector3.forward * m_fCurrentSpeed);
                    m_bWalkingLeft = true;
                    m_bFacingLeft = true;
                }
                else
                {
                    m_rBody.velocity = new Vector3(m_rBody.velocity.x, m_rBody.velocity.y, 0);
                    m_bUseZTarget = false;
                }
            }
        }      
    }

    public void MoveRig()
    {
        float topSpeed = m_fTopSpeed * (m_bOnGround ? (m_bRunning ? m_fRunSpeedMult : 1) : m_fFallSpeedMult);

        m_rBody.AddForce(m_v3MoveVelocity);
        Vector3 velocity = m_rBody.velocity;
        velocity.x = Mathf.Clamp(velocity.x, -topSpeed, topSpeed);
        m_rBody.velocity = velocity;

        m_v3MoveVelocity = Vector3.zero;
    }

    public void MoveInDirection(Vector3 a_direction)
    {
        m_v3MoveVelocity = a_direction * m_fCurrentSpeed;
    }

    public void Turn(float angle)
    {
        if (m_bTurning)
            return;

        m_bTurned = !m_bTurned;
        m_bTurning = true;
        m_fTurnAngle = angle;
        m_aAnimalAnimator.SetTrigger("Turning");
        m_aAnimalAnimator.SetBool("AnyStateFix", true);
        m_fTurnStartRotation = m_tJointRoot.eulerAngles.y;
        //m_bFinishTurn = false;
    }

    public void Turn(FACING_DIR direction)
    {
        if (m_bTurning)
            return;

        
        float angle = 0;
        bool turned = false;
        switch (direction)
        {
            case FACING_DIR.RIGHT:
                angle = 90f;
                turned = false;
                m_bTurned = false;
                break;

            case FACING_DIR.LEFT:
                angle = -90f;
                turned = true;
                m_bTurned = true;
                break;

            case FACING_DIR.FRONT:
                angle = 180f;
                break;
            case FACING_DIR.BACK:
                angle = 0f;
                break;
            default:
                break;
        }
        m_fTurnStartRotation = m_tJointRoot.eulerAngles.y;
        m_fTurnAngle = angle - m_fTurnStartRotation;
        if (Mathf.Abs(m_fTurnAngle) > 0.01f)
        {
            m_bTurning = true;
            m_aAnimalAnimator.SetTrigger("Turning");
            m_aAnimalAnimator.SetBool("AnyStateFix", true);
        }
        else
        {
            m_bTurned = !turned;
        }
    }

    public void FinishTurning()
    {
        m_bTurning = false;
        m_aAnimalAnimator.SetBool("AnyStateFix", false);

        m_bFacingLeft = m_bTurned;

        m_tJointRoot.localRotation = Quaternion.Euler(new Vector3(m_tJointRoot.localRotation.eulerAngles.x, m_bTurned? 180 + m_fTurnAxis : 0 + m_fTurnAxis, m_tJointRoot.localRotation.eulerAngles.z));
        m_fTurnStartRotation = m_tJointRoot.eulerAngles.y;
    }

    public bool MoveTo(float a_xPos)
    {
        m_bUseXTarget = true;
        m_fXTarget = a_xPos;
        if (m_fXTarget < transform.position.x + 0.015f && m_fXTarget > transform.position.x - 0.015f)
        {
            m_rBody.velocity = Vector3.zero;
            m_bUseXTarget = false;
            return true;
        }

        return false;
    }

    public bool MoveTo(Transform a_Target, bool a_xMove, bool a_yMove, bool a_zMove)
    {
        float neededFinishCount = 0;
        float finishedCount = 0;
        if (a_xMove) { m_fXTarget = a_Target.position.x; m_bUseXTarget = true; neededFinishCount++; }
        if (a_yMove) { m_fYTarget = a_Target.position.y; m_bUseYTarget = true; neededFinishCount++; }
        if (a_zMove) { m_fZTarget = a_Target.position.z; m_bUseZTarget = true; neededFinishCount++; }
        if (m_fXTarget < transform.position.x + 0.01f && m_fXTarget > transform.position.x - 0.01f)
        {
            transform.position = new Vector3(m_fXTarget, transform.position.y, transform.position.z);
            m_rBody.velocity = new Vector3(0, m_rBody.velocity.y, m_rBody.velocity.z);
            m_bUseXTarget = false;
            finishedCount++;
        }
        if (m_fYTarget < transform.position.y + 0.01f && m_fYTarget > transform.position.y - 0.01f)
        {
            transform.position = new Vector3(transform.position.x, m_fYTarget, transform.position.z);
            m_rBody.velocity = new Vector3(m_rBody.velocity.x, 0, m_rBody.velocity.z);
            m_bUseYTarget = false;
            finishedCount++;
        }
        if (m_fZTarget < transform.position.z + 0.01f && m_fZTarget > transform.position.z - 0.01f)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, m_fZTarget);
            m_rBody.velocity = new Vector3(m_rBody.velocity.x, m_rBody.velocity.y, 0);
            m_bUseZTarget = false;
            finishedCount++;
        }
        if(finishedCount == neededFinishCount)
        {
            return true;
        }

        return false;
    }

    public void StopMoveTo()
    {
        m_bUseXTarget = false;
    }

    void OnCollisionEnter(Collision a_col)
    {
        
        //if (a_col.impulse.magnitude > m_fHitResist && !a_col.collider.gameObject.CompareTag("Ground"))
        //{
        //    Kill(DEATH_TYPE.SQUASH);
        //}
    }

    void OnCollisionExit(Collision a_col)
    {
        m_bWallOnLeft = false;
        m_bWallOnRight = false;
        //if (a_col.impulse.magnitude > m_fHitResist && !a_col.collider.gameObject.CompareTag("Ground"))
        //{
        //    Kill(DEATH_TYPE.SQUASH);
        //}
    }

    public virtual void Kill(DEATH_TYPE a_death)
    {
        if (m_NoDeaths != null)
        {
            for (int i = 0; i < m_NoDeaths.Length; ++i)
            {
                if (m_NoDeaths[i] == a_death)
                    return;
            }
        }

        m_tCollider.gameObject.layer = LayerMask.NameToLayer("Animal");

        if (m_bAlive)
            CameraController.Instance.ViewThenReload(transform.position + m_v3DeathDistance, 1.5f);

        m_bAlive = false;
        Debug.Log(a_death);
        OnDeath(a_death);

        if (m_rBody != null)
            m_rBody.velocity = Vector3.zero;


        Analytics.CustomEvent(gameObject.name + " death", new Dictionary<string, object>
        {
            { "Type", a_death.ToString() }
        });
    }

    void OnDestroy()
    {
        if (m_fSelectionTimer > 0)
        {
            Analytics.CustomEvent(gameObject.name + " deselected", new Dictionary<string, object>
            {
                { "Time", m_fSelectionTimer }
            });
        }
    }

    //=======================
    // VIRUTAL METHODS
    //=======================
    public virtual void PullLever() { }
    public virtual void OnSelectChange()
    {
        if (m_bSelected)
        {
            m_aAnimalAnimator.SetBool("Sit", false);
            m_fSitTimer = -1f;
        }
        else
            m_fSitTimer = 0f;
    }

    public virtual void OnPushChange() { }

    protected virtual void OnAwake() { }
    protected virtual void OnStart() { }
    protected virtual void OnUpdate() { }
    protected virtual void OnFixedUpdate() { }
    protected virtual void OnDeath(DEATH_TYPE a_type) { }

    public void PlaySound(SOUND_EVENT a_type)
    {
        foreach (NamedEvent ev in m_aSoundEvents)
        {
            if (ev.m_Event == a_type)
            {
                if (ev.m_eFunction != null)
                    ev.m_eFunction.Invoke();
            }
        }
    }

    public void SetTimer(bool a_Run)
    {
        if (m_bGazeRunning == a_Run)
            return;

        m_fGazeTimer = 0f;
        m_bGazeRunning = a_Run;
    }

    void OnDrawGizmosSelected()
    {
        if (m_tJointRoot == null)
            return;

        Gizmos.color = Color.red;
        Matrix4x4 cubeTrans = Matrix4x4.TRS(transform.position + transform.TransformDirection(m_v3WalkingOffset), m_tJointRoot.rotation, m_v3WalkingSize);
        Matrix4x4 old = Gizmos.matrix;
        Gizmos.matrix *= cubeTrans;
        Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
        Gizmos.matrix = old;
    }
}
