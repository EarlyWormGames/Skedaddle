using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Analytics;
using System;
using System.Collections.Generic;
using RootMotion.FinalIK;
using UnityEngine.InputNew;

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

public enum FACING_DIR
{
    NONE,
    FRONT,
    RIGHT,
    BACK,
    LEFT
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

    public static Animal CurrentAnimal;

    //==================================
    //          Public Vars
    //      (Used for Inspector)
    //==================================
    public Transform        m_tCameraPivot;
    public Transform        m_tLeverReference;
    public AnimationExtras  m_eExtras;
    public GameObject       m_goIKSwitch;
    public Transform        m_tJointRoot;
    public Transform        m_tPelvis;
    public Vector3          m_v3PelvisOffset;
    public Transform        m_tCollider;

    [Header("Speeds")]
    public float            m_fWalkSpeed = 0.001f;
    public float            m_fTopSpeed = 0.05f;
    public float            m_fPullSpeedMult = 0.5f;
    public float            m_fFallSpeedMult = 0.5f;
    

    public float            m_fTurnAxis;

    public float            m_fRotSpeed = 1f;
    public float            m_fSplineRotSpeed = 1f;

    [Header("Animation")]
    public float            m_fWalkAnimMult = 20;
    public float            m_fPushAnimMult = 5;

    [Header("Rotation Settings")]
    public float            m_fForwardRotation = 180;
    public float            m_fBackRotation = 0;
    public float            m_fLeftRotation = 270;
    public float            m_fRightRotation = 90;

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
    public float            m_fCrushForce = 5;
    public DEATH_TYPE[]     m_NoDeaths;

    [Header("Sounds")]
    public NamedEvent[]     m_aSoundEvents;

    public bool             WasTeleport { get; set; }

    //==================================
    //          Internal Vars
    //    (Public use for scripts)
    //==================================
    internal ANIMAL_SIZE    m_eSize;
    internal ANIMAL_NAME    m_eName;
    
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

    internal bool           m_bCanRotate = true;
    internal bool           m_bAllowAutoRotate = true;
    internal bool           m_bAutoClimbing = false;

    internal bool           m_bOnTrampoline;
    internal bool           m_bBouncingOnTrampoline;
    internal bool           m_bOnSlope;

    internal bool           m_bCanBeSelected = true;

    internal float          m_fAnimationSpeed;
    internal float          m_fAnimationLength;

    internal bool           m_ForceTurnImmediate = false;

    internal float          m_fFallStartY = 0f;
    internal bool           m_bTurning = false;
    internal float          m_fTurnStartRotation = 0f;
    internal bool           m_bFinishTurn = false;

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
    internal bool           m_bFacingLeft = false;

    internal bool           m_bCanClimb = true;
    internal bool           m_bWalkingLeft = false;
    internal bool           m_bWalkingRight = false;
    internal AnimalMovement m_aMovement;


    //==================================
    //          Protected Vars
    //================================== 
    protected bool          m_bAlive = true;
    
    //Movement
    protected float         m_fCurrentSpeed;

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

    protected MainMapping   m_input;

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
        m_aMovement = GetComponent<AnimalMovement>();

        OnAwake();
    }
    // Use this for initialization
    void Start()
    {
        m_rBody = GetComponent<Rigidbody>();
        m_cCol = GetComponent<Collider>();
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

        m_input = GameManager.Instance.input.GetActions<MainMapping>();

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
                            rot = Quaternion.Euler(0, m_fForwardRotation, 0);
                            break;
                        }
                    case FACING_DIR.RIGHT:
                        {
                            rot = Quaternion.Euler(0, m_fRightRotation, 0);
                            m_bTurned = false;
                            break;
                        }
                    case FACING_DIR.BACK:
                        {
                            rot = Quaternion.Euler(0, m_fBackRotation, 0);
                            break;
                        }
                    case FACING_DIR.LEFT:
                        {
                            rot = Quaternion.Euler(0, m_fLeftRotation, 0);
                            m_bTurned = true;
                            break;
                        }
                }

                m_tJointRoot.rotation = Quaternion.Lerp(m_tJointRoot.rotation, rot, m_ForceTurnImmediate ? 1 : Time.deltaTime * m_fRotSpeed);
            }
            else if (!m_bTurning)
            {
                Quaternion rot = Quaternion.Euler(m_tJointRoot.localRotation.eulerAngles.x, m_bTurned ? 180 + m_fTurnAxis : 0 + m_fTurnAxis, m_tJointRoot.localRotation.eulerAngles.z);
                m_tJointRoot.localRotation = Quaternion.Lerp(m_tJointRoot.localRotation, rot, m_ForceTurnImmediate ? 1 : Time.deltaTime * m_fRotSpeed);
            }
        }
        else
        {
            Vector3 rot = Quaternion.LookRotation(m_v3ForwardTarg.normalized).eulerAngles;
            rot.z = m_v3UpTarget.z;
            m_tJointRoot.rotation = Quaternion.Lerp(m_tJointRoot.rotation, Quaternion.Euler(rot), m_ForceTurnImmediate ? 1 : Time.deltaTime * m_fRotSpeed);
        }
        #endregion
        //=================================================

        if (m_tJointRoot != null)
        {
            m_tCollider.eulerAngles = new Vector3(m_gqGrounder.PelvisRotation.eulerAngles.x + m_v3PelvisOffset.x, m_tJointRoot.rotation.eulerAngles.y + m_v3PelvisOffset.y, m_v3PelvisOffset.z);
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
        if (m_bWalkingLeft && m_rBody.velocity.x < 0.0001f && m_rBody.velocity.x > -0.0001f)
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
            if (m_gqGrounder.forelegSolver.currentGroundLayer != -1)
            {
                if (m_gqGrounder.forelegSolver.currentGroundLayer != LayerMask.NameToLayer("Grounder"))
                {
                    m_bFrontGroundDetected = true;
                }
                else
                {
                    m_bFrontGroundDetected = false;
                    
                }
                m_gqGrounder.solver.GroundTheshold = m_gqGrounder.solver.maxStep;
            }
            else
            {
                m_bFrontGroundDetected = false;
                m_gqGrounder.solver.GroundTheshold = 0.01f;
            }
            m_bBackGroundDetected = m_gqGrounder.solver.currentGroundLayer != -1;

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

                    if (dist > m_fMaxFallDist && (!m_bOnTrampoline || !WasTeleport))
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

        WasTeleport = false;
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
        m_v3MoveVelocity = Vector3.zero;  
    }

    public void MoveRig()
    {
        if (m_v3MoveVelocity != Vector3.zero)
        {
            float topSpeed = m_fTopSpeed * (m_bOnGround ? 1 : m_fFallSpeedMult);

            m_rBody.AddForce(m_v3MoveVelocity);
            Vector3 velocity = m_rBody.velocity;
            velocity.x = Mathf.Clamp(velocity.x, -topSpeed, topSpeed);
            velocity.z = Mathf.Clamp(velocity.z, -topSpeed, topSpeed);
            m_rBody.velocity = velocity;
        }
    }

    public void MoveInDirection(Vector3 a_direction)
    {
        m_v3MoveVelocity = a_direction * m_fCurrentSpeed;
        MoveRig();
        m_v3MoveVelocity = Vector3.zero;
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
                angle = 0;
                turned = false;
                m_bTurned = false;
                break;

            case FACING_DIR.LEFT:
                angle = -180;
                turned = true;
                m_bTurned = true;
                break;

            case FACING_DIR.FRONT:
                angle = 90;
                break;
            case FACING_DIR.BACK:
                angle = -90;
                break;
            default:
                break;
        }
        m_fTurnStartRotation = m_tJointRoot.localEulerAngles.y;
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

    void OnCollisionEnter(Collision collision)
    {
        Vector3 collisionDir = collision.contacts[0].point - transform.position;
        float dot = Vector3.Dot(Vector3.up, collisionDir.normalized);

        if (dot > 0 && collision.contacts[0].otherCollider.attachedRigidbody != null)
        {
            float impactVelocity = collision.contacts[0].otherCollider.attachedRigidbody.velocity.magnitude * collision.contacts[0].otherCollider.attachedRigidbody.mass;
            Debug.Log("Crush force: " + impactVelocity.ToString());

            //It's above the animal, check for crush death
            if (impactVelocity >= m_fCrushForce)
            {
                Kill(DEATH_TYPE.SQUASH);
            }
        }
    }

    void OnCollisionExit(Collision collision)
    {
        m_bWallOnLeft = false;
        m_bWallOnRight = false;
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

    public virtual float[] CalculateMoveSpeed()
    {
        float[] speedMinMax = new float[3];
        speedMinMax[0] = m_fCurrentSpeed;

        if (m_bCanWalkLeft)
            speedMinMax[1] = -m_fTopSpeed * (m_bPullingObject ? m_fPullSpeedMult : 1);
        if (m_bCanWalkRight)
            speedMinMax[2] = m_fTopSpeed * (m_bPullingObject ? m_fPullSpeedMult : 1);

        return speedMinMax;
    }

    public bool CanTurn()
    {
        bool objOkay = true;
        if (m_oCurrentObject != null)
            objOkay = !m_oCurrentObject.m_bBlocksTurn;

        if (!Alive)
            return false;

        return !m_bTurning && !m_bPullingObject && objOkay;
    }

    public virtual bool CanMove()
    {
        bool objOkay = true;
        if (m_oCurrentObject != null)
            objOkay = !m_oCurrentObject.m_bBlocksMovement;

        if (!Alive)
            return false;

        return (m_bCanWalkLeft || m_bCanWalkRight) && objOkay;
    }

    public void SetDirection(FACING_DIR direction, bool detach)
    {
        if (m_aMovement.FollowSpline != null)
        {
            if (detach)
                m_aMovement.FollowSpline = null;
            else
                return;
        }
        m_fFacingDir = direction;
    }

    public void AllowSelection(bool allow)
    {
        m_bCanBeSelected = allow;

        if (!allow && m_bSelected)
        {
            AnimalController.Instance.ChangeAnimal();
        }
    }
}
