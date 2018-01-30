using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PPObject : ActionObject
{
    [Header("PP Settings")]
    public Transform m_tParent;
    public Transform m_tMovePoint;
    public ParticleSystem m_psDust;

    public Animal m_aPushingAnimal;
    public bool m_bStopLeft = false;
    public bool m_bStopRight = false;
    public bool m_bChangeDirection = true;
    public bool m_bZDivert = false;
    public bool m_bKeepY = false;

    public string PushEvent, PushStopEvent;

    private bool m_bMoving;
    private bool m_bZMoved;

    private bool m_bPushSound = true;

    private float m_fStartZ;
    private float m_fStartY;

    private Transform m_tMain;

    //Inherited functions
    protected override void OnStart()
    {
        if (m_rBody == null)
        {
            m_rBody = GetComponent<Rigidbody>();
        }

        m_tMain = m_tParent == null ? transform : m_tParent;
        m_fStartY = m_tMain.position.y;
    }

    protected override void OnUpdate()
    {
        if (m_aPushingAnimal != null)
        {
            m_aCurrentAnimal = m_aPushingAnimal;

            if (m_bKeepY)
                m_tMain.position += new Vector3(0, m_fStartY - m_tMain.position.y, 0);

            if (m_aPushingAnimal != m_aCurrentAnimal && m_aCurrentAnimal != null)
            {
                Vector3 dir = m_aCurrentAnimal.transform.position - m_aPushingAnimal.transform.position;
                if (dir.x > 1f)
                {
                    //To the right
                    m_aPushingAnimal.m_bCanWalkRight = false;

                    if (!m_bStopLeft)
                        m_aPushingAnimal.m_bCanWalkLeft = true;
                }
                else if (dir.x < -1f)
                {
                    //To the left
                    m_aPushingAnimal.m_bCanWalkLeft = false;

                    if (!m_bStopRight)
                        m_aPushingAnimal.m_bCanWalkRight = true;
                }
                else
                {
                    if (!m_bStopRight)
                        m_aPushingAnimal.m_bCanWalkRight = true;

                    if (!m_bStopLeft)
                        m_aPushingAnimal.m_bCanWalkLeft = true;
                }
            }
            else
            {
                if (!m_bStopRight)
                    m_aPushingAnimal.m_bCanWalkRight = true;

                if (!m_bStopLeft)
                    m_aPushingAnimal.m_bCanWalkLeft = true;
            }
        }

        if (m_aPushingAnimal != null && m_aPushingAnimal.m_aAnimalAnimator.GetBool("Pushing") && m_aPushingAnimal.m_rBody.velocity.magnitude >= 0.1f)
        {
            if (m_psDust != null)
            {
                if (m_bPushSound)
                {
                    m_psDust.Play();
                }
            }

            if (m_bPushSound)
            {
                m_bPushSound = false;
                NamedEvent.TriggerEvent(PushEvent, m_aSoundEvents);
            }
        }
        else
        {
            if (!m_bPushSound)
                NamedEvent.TriggerEvent(PushStopEvent, m_aSoundEvents);

            if (m_psDust != null)
            {
                if (!m_bPushSound)
                {
                    m_psDust.Stop();
                }
            }
            m_bPushSound = true;
        }

        if (!m_bUseDefaultAction)
            return;

        if (m_aCurrentAnimal == null)
            return;
        else if ((m_aCurrentAnimal.m_oCurrentObject != this && m_aCurrentAnimal.m_oCurrentObject != null) || !m_aCurrentAnimal.m_bSelected)
            return;

        SetTimer(m_aCurrentAnimal.m_oCurrentObject == null && m_bEyetrackSelected); //This is so much easier than doing an if/else

        if ((Keybinding.GetKeyDown("Action") || Controller.GetButtonDown(ControllerButtons.A)) && m_aCurrentAnimal.m_rBody.velocity.y < 0.1 && m_aCurrentAnimal.m_rBody.velocity.y > -0.1 ||
            (m_fGazeTimer >= EWEyeTracking.holdTime))
        {
            DoAction();
            SetTimer(false);
        }
        DoAnimation();

        if (m_bMoving)
        {
            if (m_aPushingAnimal.MoveTo(m_tMovePoint, true, false, m_bZDivert))
            {
                DoAction();
                m_bMoving = false;
            }
        }


        if(m_aPushingAnimal != null && m_aPushingAnimal.m_bAutoClimbing)
        {
            if (m_tParent != null)
                m_tParent.parent = null;
        }
    }

    //protected override void AnimalEnter(Animal a_animal) { }

    public override void DoAction()
    {
        if (m_aPushingAnimal == null)
        {
            m_aPushingAnimal = m_aCurrentAnimal;
        }

        if (!m_aPushingAnimal.m_bOnGround || m_aPushingAnimal.m_bAutoClimbing && !m_aCurrentAnimal.m_bTurning)
        {
            return;
        }
        if (m_tMovePoint != null && !m_bMoving)
        {
            m_bMoving = true;
            if (!m_aPushingAnimal.m_bPullingObject)
            {
                m_fStartZ = m_aPushingAnimal.transform.position.z;
            }
        }
        else
        {
            if (m_aPushingAnimal.m_oCurrentObject == null)
            {
                m_aPushingAnimal.m_bPullingObject = true;
                m_aPushingAnimal.m_oCurrentObject = this;
                m_bQuickExitFix = true;

                if (m_bChangeDirection)
                {
                    if (m_aPushingAnimal.transform.position.x > transform.position.x)
                    {
                        m_aPushingAnimal.m_fFacingDir = FACING_DIR.LEFT;
                        m_aPushingAnimal.m_bTurned = true;
                    }
                    else
                    {
                        m_aPushingAnimal.m_fFacingDir = FACING_DIR.RIGHT;
                        m_aPushingAnimal.m_bTurned = false;

                    }
                }

                if (m_tParent == null)
                {
                    transform.parent = m_aPushingAnimal.transform;
                }
                else
                {
                    m_tParent.parent = m_aPushingAnimal.transform;
                }


                m_aPushingAnimal.OnPushChange();
            }
            else if (m_aPushingAnimal.m_oCurrentObject == this)
            {
                m_aPushingAnimal.m_bPullingObject = false;
                m_aPushingAnimal.m_oCurrentObject = null;
                if (m_tParent == null)
                {
                    transform.parent = null;
                }
                else
                {
                    m_tParent.parent = null;
                }
                m_aPushingAnimal.m_fFacingDir = FACING_DIR.NONE;
                m_aPushingAnimal.m_bCanWalkLeft = true;
                m_aPushingAnimal.m_bCanWalkRight = true;

                Vector3 pos = m_aPushingAnimal.transform.position;
                if (m_bZDivert) pos.z = m_fStartZ;
                m_aPushingAnimal.transform.position = pos;

                m_aPushingAnimal.OnPushChange();
                m_aPushingAnimal = null;
                m_aCurrentAnimal = null;
            }
        }
    }

    public override void Detach()
    {
        if (m_aPushingAnimal == null)
        {
            m_aCurrentAnimal = null;
            return;
        }

        m_aPushingAnimal.m_bPullingObject = false;
        m_aPushingAnimal.m_oCurrentObject = null;
        if (m_tParent == null)
        {
            transform.parent = null;
        }
        else
        {
            m_tParent.parent = null;
        }
        m_aPushingAnimal.m_fFacingDir = FACING_DIR.NONE;
        m_aPushingAnimal.m_bCanWalkLeft = true;
        m_aPushingAnimal.m_bCanWalkRight = true;
        m_aPushingAnimal.OnPushChange();
        m_aPushingAnimal = null;
        m_aCurrentAnimal = null;
    }

    void OnDestroy()
    {
        if (m_aPushingAnimal == null)
            return;

        if (m_aPushingAnimal.m_oCurrentObject == this)
        {
            m_aPushingAnimal.m_bPullingObject = false;
            m_aPushingAnimal.m_oCurrentObject = null;
            if (m_tParent == null)
            {
                transform.parent = null;
            }
            else
            {
                m_tParent.parent = null;
            }
            m_aPushingAnimal.m_fFacingDir = FACING_DIR.NONE;
            m_aPushingAnimal.m_bCanWalkLeft = true;
            m_aPushingAnimal.m_bCanWalkRight = true;
            m_aPushingAnimal.OnPushChange();
            m_aPushingAnimal = null;
        }
    }
}
