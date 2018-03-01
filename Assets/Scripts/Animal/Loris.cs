using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class Loris : Animal
{
    //==================================
    //          Public Vars
    //==================================
    public float m_fClimbSpeed = 1f;
    public float m_fClimbTopSpeed = 1.5f;
    public float m_fClimbStopSpeed = 0.2f;

    public Light[] m_lVisionLight = new Light[0];
    public float m_fOnIntensity = 1.68f;
    public float m_fLightSpeed = 1f;
    public Vector2 m_v2IdleChangeTime;

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

    //==================================
    //          Private Vars
    //==================================
    private bool m_bStartedWalking = false;
    private bool m_bIgnoreCheck = false;

    private bool m_bUseLight = false;
    private bool m_bUseNightVision = false;
    private LorisNightVision NV;

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

        NV = GetComponent<LorisNightVision>();
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

        if (m_bClimbing)
        {
            m_fFallStartY = transform.position.y;
        }
        //All animations here

        //if (m_pFollower.m_bFollow)
        //{
        //    m_aAnimalAnimator.SetBool("Controlled", true);
        //    m_bWasFollow = true;
        //}
        //else if (m_bWasFollow)
        //{
        //    m_bWasFollow = false;
        //    m_aAnimalAnimator.SetBool("Controlled", m_bSelected);
        //}

        //set horozontal velocity
        m_aAnimalAnimator.SetFloat("Velocity", m_aMovement.moveVelocity * m_fWalkAnimMult);
        m_aAnimalAnimator.SetBool("Climbing", m_bClimbing);

        float verticalVelocity = 0;

        if (!m_bClimbing)
            m_aAnimalAnimator.SetFloat("Vertical Velocity", m_rBody.velocity.y);
        else
        {
            verticalVelocity = ((LadderObject)m_oCurrentObject).moveVelocity;
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
            m_aAnimalAnimator.SetFloat("Horizontal Velocity", (rot < 180 ? m_aMovement.moveVelocity : -m_aMovement.moveVelocity) * m_fPushAnimMult);
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

            if (m_oCurrentObject != null)
            {
                //if (m_aAnimalAnimator.GetFloat("OnRope") == 1 && m_oCurrentObject.GetComponent<LadderObject>().m_bCanShimmy)
                //{
                //    
                //        if (Keybinding.GetKey("MoveLeft") || Controller.GetDpad(ControllerDpad.Left) || Controller.GetStick(true).x < -0.2f)
                //        {
                //            m_aAnimalAnimator.SetBool("Shimmy Left", true);
                //            m_aAnimalAnimator.SetBool("Shimmy Right", false);
                //        }
                //        else if (Keybinding.GetKey("MoveRight") || Controller.GetDpad(ControllerDpad.Right) || Controller.GetStick(true).x > 0.2f)
                //        {
                //            m_aAnimalAnimator.SetBool("Shimmy Right", true);
                //            m_aAnimalAnimator.SetBool("Shimmy Left", false);
                //        }
                //        else
                //        {
                //            m_aAnimalAnimator.SetBool("Shimmy Left", false);
                //            m_aAnimalAnimator.SetBool("Shimmy Right", false);
                //        }
                //    
                //}
                //else
                //{
                //    m_aAnimalAnimator.SetBool("Shimmy Left", false);
                //    m_aAnimalAnimator.SetBool("Shimmy Right", false);
                //}
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

        if (m_lVisionLight.Length > 0)
        {
            if (m_bUseLight && !m_bSelected)
            {
                // LIGHT ON
                for(int i = 0; i < m_lVisionLight.Length; ++i)
                    m_lVisionLight[i].intensity = Mathf.Lerp(m_lVisionLight[i].intensity, m_fOnIntensity, m_fLightSpeed * Time.deltaTime);
            }
            else
            {
                // LIGHT OFF
                for(int i = 0; i < m_lVisionLight.Length; ++i)
                    m_lVisionLight[i].intensity = Mathf.Lerp(m_lVisionLight[i].intensity, -1f, m_fLightSpeed * Time.deltaTime);
            }
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

        if (m_fSelectionTimer > 0)
        {
            Analytics.CustomEvent(gameObject.name + " deselected", new Dictionary<string, object>
            {
                { "Time", m_fSelectionTimer }
            });
        }
    }

    protected override void OnFixedUpdate()
    {
        
    }

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

    public void SetNightVision(bool a_On)
    {
        CameraController.Instance.m_bUseNightVision = a_On;
        m_bUseLight = a_On;
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

    public override bool CanMove()
    {
        bool canmove = base.CanMove();
        return canmove && !m_bClimbing && !m_bInCannon;
    }
}