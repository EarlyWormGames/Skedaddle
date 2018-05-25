using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class Loris : Animal
{
    //==================================
    //          Public Vars
    //==================================
    [Header("Climb Settings")]
    public float m_fClimbSpeed = 1f;
    public float m_fClimbTopSpeed = 1.5f;
    public float m_fClimbStopSpeed = 0.2f;
    public Vector3 m_fShimmyBoxSize = new Vector3(0.1f, 0.1f, 0.1f);
    public float m_fShimmyDistance = 0.6f;

    [Header("Loris Misc")]
    public Light[] m_lVisionLight = new Light[0];
    public float m_fOnIntensity = 1.68f;
    public float m_fLightSpeed = 1f;
    public Vector2 m_v2IdleChangeTime;
    public bool m_bStartWithNVOn = false;

    public float m_fDropTime = 0.3f;

    [Tooltip("How far up/down eyes need to be looking to move while climbing")]
    public float m_fClimbBuffer = 0.5f;

    public float m_fClimbAnimMult = 100;
    public float m_fClimbAnimMinimum = 0.005f;

    //==================================
    //          Internal Vars
    //==================================
    internal bool m_bClimbing = false;
    internal bool m_bCanClimbUp = true;
    internal bool m_bCanClimbDown = true;
    internal bool m_bInCannon = false;

    internal bool m_bHorizontalRope;

    internal bool m_bHeldByPoodle = false;

    //==================================
    //          Private Vars
    //==================================
    private bool m_bStartedWalking = false;
    private bool m_bIgnoreCheck = false;

    private bool m_bUseLight = false;
    public bool GetLightStatus() { return m_bUseLight; }
    private bool m_bUseNightVision = false;
    private NightVision NV;

    private float m_fDropTimer = 0.3f;
    private float m_fElectroTimer = 0.3f;
    private float m_fIdleTimer = 10f;
    private bool m_bWasFollow = false;

    private bool m_bEyeClimbUp;
    private bool m_bEyeClimbDown;

    //Inherited functions
    protected override void OnAwake()
    {
        m_eName = ANIMAL_NAME.LORIS;
        m_eSize = ANIMAL_SIZE.XS;

        NV = GetComponent<NightVision>();
        if (m_bStartWithNVOn) SetNightVision(true);
    }

    protected override void OnUpdate()
    {
        if (EWEyeTracking.active)
        {
            if (EWEyeTracking.worldPosition.y > transform.position.y + m_fClimbBuffer)
                m_bEyeClimbUp = true;
            else
                m_bEyeClimbUp = false;

            if (EWEyeTracking.worldPosition.y < transform.position.y - m_fClimbBuffer)
                m_bEyeClimbDown = true;
            else
                m_bEyeClimbDown = false;
        }
        m_fCurrentSpeed = m_fWalkSpeed;

        if (m_bClimbing)
        {
            m_fFallStartY = transform.position.y;
        }
        //All animations here

        bool forceMove = false;
        if (m_aMovement.FollowSpline != null)
        {
            if (m_aMovement.FollowSpline.ForceMovement)
            {
                m_aAnimalAnimator.SetBool("Controlled", true);
                m_bWasFollow = true;
                forceMove = true;
            }
        }

        if (m_bWasFollow && !forceMove)
        {
            m_bWasFollow = false;
            m_aAnimalAnimator.SetBool("Controlled", m_bSelected);
        }

        //set horizontal velocity
        m_aAnimalAnimator.SetFloat("Velocity", (!m_bSelected && !m_bForceWalk) ? 0 : m_aMovement.moveVelocity * m_fWalkAnimMult);
        m_aAnimalAnimator.SetBool("Climbing", m_bClimbing);

        float verticalVelocity = 0;

        if (!m_bClimbing)
            m_aAnimalAnimator.SetFloat("Vertical Velocity", m_rBody.velocity.y);
        else
        {
            verticalVelocity = !m_bSelected ? 0 : ((LadderObject)currentAttached).moveVelocity;
            m_aAnimalAnimator.SetFloat("Vertical Velocity", verticalVelocity * m_fClimbAnimMult);
        }

        m_fIdleTimer -= Time.deltaTime;

        if (m_fIdleTimer < 0)
        {
            if(m_aAnimalAnimator.GetInteger("IdleNo") == 0)
            {
                m_aAnimalAnimator.SetInteger("IdleNo", 1);
                m_fIdleTimer = 3;
            }
            else
            {
                m_aAnimalAnimator.SetInteger("IdleNo", 0);
                m_fIdleTimer = Random.Range(m_v2IdleChangeTime.x, m_v2IdleChangeTime.y);
            }
        }

        //if (m_bLeanUp)
        //{
        //    m_aAnimalAnimator.SetFloat("LeanDirection", 1);
        //}
        //else if (m_bLeanDown)
        //{
        //    m_aAnimalAnimator.SetFloat("LeanDirection", -1);
        //}
        //else
        //{
        //    m_aAnimalAnimator.SetFloat("LeanDirection", 0);
        //}

        if (m_aAnimalAnimator.GetBool("Electrocuted"))
        {
            m_fElectroTimer -= Time.deltaTime;
            if (m_fElectroTimer <= 0)
            {
                m_aAnimalAnimator.SetBool("Electrocuted", false);
                m_fElectroTimer = 0.3f;
            }
        }

        if (!m_bOnGround)
        {
            m_aAnimalAnimator.SetFloat("Follow-Through Vertical", m_aAnimalAnimator.GetFloat("Vertical Velocity") * 2);
        }
        else
        {
            m_aAnimalAnimator.SetFloat("Follow-Through Vertical", Mathf.Lerp(m_aAnimalAnimator.GetFloat("Follow-Through Vertical"), 0.1f, Time.deltaTime * 1.5f));
        }

        //if(m_bOnGround && !m_bRestGroundDetected)
        //{
        //    m_aAnimalAnimator.SetBool("EdgeWarning", true);
        //}
        //else
        //{
        //    m_aAnimalAnimator.SetBool("EdgeWarning", false);
        //}

        m_aAnimalAnimator.SetBool("Dead", !m_bAlive);

        if (m_bWallOnLeft || m_bWallOnRight)
        {
            m_aAnimalAnimator.SetBool("OnWall", true);
        }
        else
        {
            m_aAnimalAnimator.SetBool("OnWall", false);
        }

        //AutoClimbing
        if (m_bAutoClimbing)
        {
            m_aAnimalAnimator.SetBool("AutoClimbing", true);
        }
        else
        {
            m_aAnimalAnimator.SetBool("AutoClimbing", false);
            m_aAnimalAnimator.SetBool("Walking", false);
        }

        if (m_bInCannon)
        {
            m_aAnimalAnimator.SetBool("Cannon", true);
        }
        else
        {
            m_aAnimalAnimator.SetBool("Cannon", false);
        }

        if (m_bPullingObject)
        {
            m_aAnimalAnimator.SetBool("Pushing", true);
            float rot = m_tJointRoot.localEulerAngles.y;
            m_aAnimalAnimator.SetFloat("Horizontal Velocity", !m_bSelected ? 0 : (rot < 180 ? m_aMovement.moveVelocity : -m_aMovement.moveVelocity) * m_fPushAnimMult);
        }
        else
        {
            m_aAnimalAnimator.SetBool("Pushing", false);
            m_aAnimalAnimator.SetFloat("Horizontal Velocity", 0);
            
        }

        if (m_bPullingLeverOn)
        {
            m_aAnimalAnimator.SetBool("PullingLeverOn", true);
        }
        else
        {
            m_aAnimalAnimator.SetBool("PullingLeverOn", false);
        }

        if (m_bPullingLeverOff)
        {
            m_aAnimalAnimator.SetBool("PullingLeverOff", true);
        }
        else
        {
            m_aAnimalAnimator.SetBool("PullingLeverOff", false);
        }

        //WALKING
        if (m_bOnGround && !m_bClimbing && !m_bAutoClimbing)
        {
            //on ground
            m_aAnimalAnimator.SetBool("OnGround", true);
            if ((m_bWalkingLeft ^ m_bWalkingRight) || m_bForceWalk)
            {
                m_aAnimalAnimator.SetBool("Walking", true);
                m_bStartedWalking = true;
            }
            else
            {
                m_aAnimalAnimator.SetBool("Walking", false);
            }
        }
        else if (!m_bClimbing && !m_bAutoClimbing)
        {
            //Falling
            m_aAnimalAnimator.SetBool("Walking", false);
            m_aAnimalAnimator.SetBool("OnGround", false);

        }
        else if(!m_bAutoClimbing && m_bClimbing)
        {
            //Climbing
            m_aAnimalAnimator.SetFloat("ClimbDirection", verticalVelocity);

            if (verticalVelocity > m_fClimbAnimMinimum)
            {
                //Climb up
                m_aAnimalAnimator.SetBool("ClimbingUp", true);
                m_aAnimalAnimator.SetBool("ClimbingDown", false);
                m_aAnimalAnimator.SetBool("ClimbMoving", true);
                m_aAnimalAnimator.SetFloat("ClimbDirection", 1);
            }
            else if (verticalVelocity < -m_fClimbAnimMinimum)
            {
                //climb Down
                m_aAnimalAnimator.SetBool("ClimbingUp", false);
                m_aAnimalAnimator.SetBool("ClimbingDown", true);
                m_aAnimalAnimator.SetBool("ClimbMoving", true);
                m_aAnimalAnimator.SetFloat("ClimbDirection", -1);
            }
            else
            {
                //climb idle
                m_aAnimalAnimator.SetBool("ClimbingUp", false);
                m_aAnimalAnimator.SetBool("ClimbingDown", false);
                m_aAnimalAnimator.SetBool("ClimbMoving", false);
            }

            
            m_aAnimalAnimator.SetBool("OnGround", false);
            m_aAnimalAnimator.SetBool("Walking", false);

            LadderObject ladder = (LadderObject)currentAttached;
            if (ladder != null)
            {
                if (ladder.TryShimmyLeft)
                {
                    m_aAnimalAnimator.SetBool("Shimmy Left", true);
                    m_aAnimalAnimator.SetBool("Shimmy Right", false);
                }
                else if (ladder.TryShimmyRight)
                {
                    m_aAnimalAnimator.SetBool("Shimmy Right", true);
                    m_aAnimalAnimator.SetBool("Shimmy Left", false);
                }
                else
                {
                    m_aAnimalAnimator.SetBool("Shimmy Left", false);
                    m_aAnimalAnimator.SetBool("Shimmy Right", false);
                }
            }
            else
            {
                m_aAnimalAnimator.SetBool("Shimmy Left", false);
                m_aAnimalAnimator.SetBool("Shimmy Right", false);
            }
        }

        if (m_bOnTrampoline)
        {
            if (!m_bBouncingOnTrampoline)
            {
                m_aAnimalAnimator.SetBool("OnGround", true);
                m_aAnimalAnimator.SetBool("Walking", true);
            }
        }
        if (!m_bIgnoreCheck)
        {
            if (m_bHorizontalRope)
            {
                m_aAnimalAnimator.SetFloat("OnRope", 1.0f);
            }
            else
            {
                m_aAnimalAnimator.SetFloat("OnRope", 0.0f);
            }
        }

        //=====================================
        // NIGHT VISION
        //=====================================
        if (m_bUseLight && m_bSelected)
        {
            NV.NightVisionOn = true;
        }
        else
        {
            NV.NightVisionOn = false;
        }

        //=====================================
        bool climbDown = false;

        if (!climbDown)
            m_fDropTimer = m_fDropTime;
    }

    public override void OnSelectChange()
    {
        base.OnSelectChange();

        m_aAnimalAnimator.SetBool("Controlled", m_bSelected);

        if (m_bSelected && m_bHeldByPoodle)
        {
            var poodle = AnimalController.Instance.GetAnimal(ANIMAL_NAME.POODLE) as Poodle;
            poodle.DropLoris();
        }
    }

    /// <summary>
    /// Call when you want the loris to be dropped by the poodle
    /// </summary>
    public void LetGoOfPoodle()
    {
        if (m_bHeldByPoodle)
        {
            transform.parent = null;
            var poodle = AnimalController.Instance.GetAnimal(ANIMAL_NAME.POODLE) as Poodle;
            m_bHeldByPoodle = false;

            transform.position = poodle.transform.position;

            var dir = poodle.LorisHolder.position - transform.position;

            RaycastHit hitInfo;
            if (m_rBody.SweepTest(dir.normalized, out hitInfo, dir.magnitude))
            {
                dir.y = 0;
                var inverseLength = dir.magnitude - hitInfo.distance;
                transform.position = hitInfo.point + (dir.normalized * -inverseLength);
            }
            else
                transform.position = poodle.LorisHolder.position;

            m_rBody.useGravity = true;
            SetColliderActive(true);
        }
    }

    protected override void OnFixedUpdate()
    {
        
    }
    
    /// <summary>
    /// set the facing direction of the loris when climbing
    /// </summary>
    /// <param name="a_facingType"></param>
    /// <param name="a_isRope"></param>
    public void ClimbChange(FACING_DIR a_facingType, bool a_isRope)
    {
        m_bIgnoreCheck = true;
        if (a_isRope)
        {
            m_aAnimalAnimator.SetFloat("OnRope", 1);
        }
        else
        {
            m_aAnimalAnimator.SetFloat("OnRope", 0);
        }

        if (!m_bClimbing)
        {
            m_fFacingDir = a_facingType;
            return;
        }

        m_fFacingDir = a_facingType;     
    }

    protected override void OnDeath(DEATH_TYPE a_type)
    {
        switch (a_type)
        {
            case DEATH_TYPE.ELECTRICITY:
                {
                    m_aAnimalAnimator.SetBool("Electrocuted", true);
                    m_aAnimalAnimator.SetBool("Dead", true);
                    PlaySound(SOUND_EVENT.ELECTRO_DEATH);
                    break;
                }
            case DEATH_TYPE.FALL:
                PlaySound(SOUND_EVENT.FALL_DEATH);
                break;
        }
    }
    
    /// <summary>
    /// turn night vision on/off
    /// </summary>
    /// <param name="a_On"></param>
    public void SetNightVision(bool a_On)
    {
        CameraController.Instance.m_bUseNightVision = a_On;
        m_bUseLight = a_On;
        NV.NightVisionOn = a_On;
        RenderSettings.ambientMode = a_On ? UnityEngine.Rendering.AmbientMode.Skybox : UnityEngine.Rendering.AmbientMode.Flat;
    }

    public override float[] CalculateMoveSpeed()
    {
        float[] speedMinMax = new float[3];
        speedMinMax[0] = m_fCurrentSpeed;
        if (m_bClimbing)
            speedMinMax[0] = m_fClimbSpeed;

        if (!m_bClimbing)
        {
            if (m_bCanWalkLeft)
                speedMinMax[1] = -m_fTopSpeed * (m_bPullingObject ? m_fPullSpeedMult : 1);
            if (m_bCanWalkRight)
                speedMinMax[2] = m_fTopSpeed * (m_bPullingObject ? m_fPullSpeedMult : 1);
        }
        else
        {
            speedMinMax[1] = -m_fClimbTopSpeed;
            speedMinMax[2] = m_fClimbTopSpeed;
        }

        return speedMinMax;
    }

    /// <summary>
    /// Can the loris move?
    /// </summary>
    /// <returns></returns>
    public override bool CanMove()
    {
        bool canmove = base.CanMove();
        return canmove && !m_bClimbing && !m_bInCannon;
    }

    public override ANIMAL_NAME GetName()
    {
        return ANIMAL_NAME.LORIS;
    }
}