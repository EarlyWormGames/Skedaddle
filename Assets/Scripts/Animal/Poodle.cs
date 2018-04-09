using UnityEngine;
using UnityEngine.Analytics;
using System.Collections.Generic;

public class Poodle : Animal
{
    //==================================
    //          Public Vars
    //==================================
    [Range(0.01f, 10)]
    public float m_fPelvisCorrectionDampening = 1;
    public float m_fPushForce;
    public float m_IKLedgeBlend;
    public float m_fIKDampen = 1;
    [HideInInspector]
    public bool m_bIKAnimationFix;

    public float m_RunBuffer = 2f;
    public float m_fRunSpeedMult = 1.5f;

    [Header("Loris Carry Settings")]
    [Tooltip("The size of the box check for the Loris")]
    public Vector3 BoxCastSize = Vector3.one;
    [Tooltip("How far from the Poodle the cast will occur")]
    public float BoxCastDistance = 1;
    public float BoxCastY_Offset;
    public Transform LorisHolder;
    public LayerMask LorisCheckLayer;


    //==================================
    //          Internal Vars
    //==================================
    internal bool m_bRunning = false;
    internal bool m_bHoldingLoris = false;

    //==================================
    //          Private Vars
    //================================== 
    private float m_fPlugPullDelay;
    private bool m_bWasFollow = false;
    private float m_fDampen = 1;
    
    private float m_fIKBackLegsDampen;
    private bool m_bInteractPressed = false;


    //Inherited functions
    protected override void OnAwake()
    {
        m_eName = ANIMAL_NAME.POODLE;
        m_eSize = ANIMAL_SIZE.S;
        
    }

    public override void OnSelectChange()
    {
        base.OnSelectChange();

        m_aAnimalAnimator.SetBool("Controlled", m_bSelected);

        if (!m_bSelected)
        {
            Analytics.CustomEvent(m_eName.ToString() + " deselected", new Dictionary<string, object>
            {
                { "Time", m_fSelectionTimer }
            });
        }
    }

    protected override void OnUpdate()
    {
        if (m_bHoldingLoris)
        {
            AnimalController.Instance.GetAnimal(ANIMAL_NAME.LORIS).transform.position = LorisHolder.position;
        }

        m_aAnimalAnimator.SetBool("Controlled", m_bSelected);
        m_aAnimalAnimator.SetBool("Dead", !Alive);

        m_fIKDampen = Mathf.Lerp(m_fIKDampen, m_IKLedgeBlend, Time.deltaTime * 3);
        m_aAnimalAnimator.SetLayerWeight(3, m_fIKDampen);

        m_bRunning = false;
        m_fCurrentSpeed = m_fWalkSpeed;
        if (m_gqGrounder.solver.currentGroundLayer != -1)
        {
            if (m_bSelected && !m_bPullingObject && !m_bTurning && !m_bForceWalk)
            {
                if (EWEyeTracking.active)
                {
                    if (EWEyeTracking.worldPosition.x > transform.position.x + m_RunBuffer ||
                        EWEyeTracking.worldPosition.x < transform.position.x - m_RunBuffer)
                    {
                        m_bRunning = true;
                        m_fCurrentSpeed = m_fWalkSpeed * m_fRunSpeedMult;
                    }
                    else
                        m_bRunning = false;
                }

                if (GameManager.Instance.mainMap.sprint.isHeld)
                {
                    m_fCurrentSpeed = m_fWalkSpeed * m_fRunSpeedMult;
                    m_bRunning = true;
                }
            }
            else if (m_bPullingObject)
            {
                m_bRunning = false;
            }
        }


        //All animations here

        //set horozontal velocity
        m_aAnimalAnimator.SetFloat("Vertical Velocity", m_rBody.velocity.y);
        m_aAnimalAnimator.SetFloat("Horizontal Velocity", !m_bSelected ? 0 : m_aMovement.moveVelocity * m_fWalkAnimMult);

        
        if (!m_bOnSlope)
        {
            if (m_gqGrounder.forelegSolver.rootHit.point.y < m_gqGrounder.solver.rootHit.point.y && m_bOnGround)
            {
                if (m_rBody.velocity.x < 0.1f && m_rBody.velocity.x > -0.1f)
                {
                    if ((m_gqGrounder.solver.rootHit.point.y - m_gqGrounder.forelegSolver.rootHit.point.y < 0.33f) && m_bFrontGroundDetected)
                    {
                        //m_fDampen = Mathf.Clamp(((m_gqGrounder.solver.rootHit.point.y - m_gqGrounder.forelegSolver.rootHit.point.y) / m_fPelvisCorrectionDampening), 0, 1);
                    }
                    else
                    {
                        //distance to the next level down is greater than 0.5
                        //hold for 0.5sec to walk off ledge 




                        //m_aAnimalAnimator.SetBool("EdgeWarning", true);
                    }
                }
            }
        }
        

        m_aAnimalAnimator.SetLayerWeight(2, Mathf.Lerp(m_aAnimalAnimator.GetLayerWeight(2), m_fDampen, Time.deltaTime * 10));

        if (m_aAnimalAnimator.GetBool("PullingOutCord"))
        {
            m_fPlugPullDelay -= Time.deltaTime;
        }
        else
        {
            m_fPlugPullDelay = 0.3f;
        }

        if (m_bAutoClimbing)
        {
            m_aAnimalAnimator.SetBool("AutoClimbing", true);
        }
        else
        {
            m_aAnimalAnimator.SetBool("AutoClimbing", false);
        }

        if (m_bForceWalk)
        {
            m_aAnimalAnimator.SetBool("Controlled", true);
            m_aAnimalAnimator.SetFloat("Horizontal Velocity", 1);
            m_bWasFollow = true;
        }
        else if (m_bWasFollow)
        {
            m_bWasFollow = false;
            m_aAnimalAnimator.SetBool("Controlled", m_bSelected);
        }
        

        //WALKING
        if (m_bOnGround)
        {
            //on ground
            m_aAnimalAnimator.SetBool("OnGround", true);
            m_fDampen = 1;
            m_aAnimalAnimator.SetFloat("Follow-Through Vertical", Mathf.Lerp(m_aAnimalAnimator.GetFloat("Follow-Through Vertical"), 0, Time.deltaTime * 2f));

            m_aAnimalAnimator.SetBool("EdgeWarning", false);
            if (!m_bRestGroundDetected)
            {
                m_aAnimalAnimator.SetBool("EdgeWarning", true);
            }

            if (((m_bWalkingLeft ^ m_bWalkingRight) && m_bSelected) || m_bForceWalk)
            {
                //walking
                m_aAnimalAnimator.SetBool("Walking", true);
                m_aAnimalAnimator.SetBool("FacingLeft", m_bTurned);
            }
            else
            {
                m_aAnimalAnimator.SetBool("Walking", false);
            }
            if (m_bRunning && (m_aMovement.moveVelocity > m_fTopSpeed || m_aMovement.moveVelocity < -m_fTopSpeed))
            {
                m_aAnimalAnimator.SetBool("Sprint", true);
            } else
            {
                m_aAnimalAnimator.SetBool("Sprint", false);
            }

            if (m_bPullingObject)
            {
                if(m_bWalkingRight)
                    m_aAnimalAnimator.SetFloat("Push Direction", 0.1f);
                else if(m_bWalkingLeft)
                    m_aAnimalAnimator.SetFloat("Push Direction", -0.1f);
                else
                    m_aAnimalAnimator.SetFloat("Push Direction", 0);
            }
            else
            {
                m_aAnimalAnimator.SetFloat("Push Direction", 0);
            }

            if (m_bPullingPlug)
            {
                m_aAnimalAnimator.SetBool("PullingOutCord", true);
            }
            if (m_aAnimalAnimator.GetFloat("Stop_Lerp") > 0 && m_fPlugPullDelay < 0)
            {
                m_bPullingPlug = false;
                m_aAnimalAnimator.SetBool("PullingOutCord", false);
            }
            //if(m_bRestGroundDetected)
            //{
            //    m_aAnimalAnimator.SetBool("EdgeWarning", false);
            //}
        }
        else
        {
            //if not on a ledge that is being fixed by the IK system, then fall otherwise IK sys is handling it.
            if (!m_bIKAnimationFix)
            {
                //Falling
                m_aAnimalAnimator.SetBool("Walking", false);
                m_aAnimalAnimator.SetBool("OnGround", false);
                m_aAnimalAnimator.SetBool("EdgeWarning", false);
                m_fDampen = 0;
                m_aAnimalAnimator.SetFloat("Follow-Through Vertical", m_aAnimalAnimator.GetFloat("Vertical Velocity"));
            }

        }
        if (!m_bSelected)
        {
            m_aAnimalAnimator.SetFloat("Push Direction", 0);
        }

        if (m_bSelected && GameManager.Instance.mainMap.interact.wasJustPressed)
            m_bInteractPressed = true;
        else if (m_bSelected && m_bInteractPressed)
        {
            m_bInteractPressed = false;

            if (!m_bHoldingLoris)
            {
                if (m_oCurrentObject == null)
                {
                    Vector3 castPos = transform.position;
                    castPos += m_tJointRoot.forward * BoxCastDistance;
                    castPos.y += BoxCastY_Offset;

                    var cols = Physics.OverlapBox(castPos, BoxCastSize, m_tJointRoot.rotation, LorisCheckLayer);
                    foreach (var item in cols)
                    {
                        Loris loris = item.GetComponentInParent<Loris>();
                        if (loris == null)
                            continue;

                        if (loris.transform.parent != null)
                            break;

                        loris.SetColliderActive(false);
                        loris.m_rBody.useGravity = false;
                        loris.m_bHeldByPoodle = true;

                        loris.transform.SetParent(LorisHolder);
                        loris.transform.localPosition = Vector3.zero;

                        m_bHoldingLoris = true;
                    }
                }
            }
            else
            {
                var loris = AnimalController.Instance.GetAnimal(ANIMAL_NAME.LORIS) as Loris;
                loris.LetGoOfPoodle();
            }
        }
    }

    public override void OnPushChange()
    {
        if (m_bPullingObject == true)
        {
            m_aAnimalAnimator.SetBool("PushingObject", true);
            m_aAnimalAnimator.SetBool("Pushing", true);
        }
        else
        {
            m_aAnimalAnimator.SetBool("PushingObject", false);
            m_aAnimalAnimator.SetFloat("Push Direction", 0);
            m_aAnimalAnimator.SetBool("Pushing", false);
        }
    }

    protected override void OnFixedUpdate()
    {
        if (!m_bOnSlope)
        {
            if (m_gqGrounder.forelegSolver.rootHit.point.y < m_gqGrounder.solver.rootHit.point.y && m_bOnGround)
            {
                if (m_rBody.velocity.x < 0.1f && m_rBody.velocity.x > -0.1f)
                {
                    if (!((m_gqGrounder.solver.rootHit.point.y - m_gqGrounder.forelegSolver.rootHit.point.y < 0.33f)))
                    {
                        if (!m_bPushOffDetected)
                        {
                            if (!m_bTurning)
                            {
                                //m_rBody.AddForce(m_tCollider.transform.forward * m_fPushForce, ForceMode.Impulse);
                            }
                            else
                            {
                               //m_rBody.AddForce(m_bFacingLeft ? Vector3.left * m_fPushForce : Vector3.right * m_fPushForce, ForceMode.Impulse);
                            }
                        }
                    }
                }
            }
        }
    }
    protected override void OnDeath(DEATH_TYPE a_type)
    {
        switch (a_type)
        {
            case DEATH_TYPE.FALL:
                PlaySound(SOUND_EVENT.FALL_DEATH);
                break;
        }
    }

    public override float[] CalculateMoveSpeed()
    {
        float[] speedMinMax = new float[3];
        speedMinMax[0] = m_fCurrentSpeed;

        float mult = (m_bRunning ? m_fRunSpeedMult : 1);
        mult *= m_bPullingObject ? m_fPullSpeedMult : 1;

        if (m_bCanWalkLeft)
            speedMinMax[1] = -m_fTopSpeed * mult;
        if (m_bCanWalkRight)
            speedMinMax[2] = m_fTopSpeed * mult;

        return speedMinMax;
    }

    private void OnDrawGizmos()
    {
        if (m_tJointRoot != null)
        {
            Vector3 castPos = transform.position;
            castPos += m_tJointRoot.forward * BoxCastDistance;
            castPos.y += BoxCastY_Offset;

            Gizmos.matrix = Matrix4x4.TRS(castPos, m_tJointRoot.rotation, BoxCastSize * 2);
            Gizmos.color = Color.blue;

            Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
        }
    }
}
