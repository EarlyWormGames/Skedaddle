using UnityEngine;
using UnityEngine.SceneManagement;
using RootMotion.FinalIK;
using System.Collections;

public class ClimbJump : ActionObject
{
    public Transform m_tAnchorPoint;
    public FACING_DIR m_fDirection;
    public FACING_DIR m_FinishDirection;
    public Animal m_aClimbingAnimal;
    public bool m_bDoOnTrigger;
    public bool m_bDoOnKeyPress = true;

    public ActionObject m_TransitionTo;

    private float m_fTimer = -1f;
    private float m_fClimbSideDirection = 1;
    private Vector3 m_v3ClimbCurve;

    //Inherited functions

    //protected override void OnStart() { }

    protected override void OnUpdate()
    {
        if (m_aCurrentAnimal != null && m_bDoOnKeyPress)
        {
            if (m_aCurrentAnimal.m_bSelected && !m_aCurrentAnimal.m_bAutoClimbing && m_aCurrentAnimal.m_aAnimalAnimator.GetFloat("Stop_Lerp") <= 0 && !m_aCurrentAnimal.m_bTurning)
            {
                SetTimer(m_aCurrentAnimal.m_EyeUp);

                if (Keybinding.GetKeyDown("MoveUp") || Controller.GetDpadDown(ControllerDpad.Up) || Controller.GetStickPositionDown(true, ControllerDpad.Up) ||
                    m_fGazeTimer >= EWEyeTracking.shortHoldTime)
                {
                    SetTimer(false);
                    DoAction();
                }
            }
        }
    }

    protected override void OnFixedUpdate()
    {
        if (m_fTimer > -1f)
        {
            m_aClimbingAnimal.m_rBody.isKinematic = true;
            m_aClimbingAnimal.m_aAnimalAnimator.updateMode = AnimatorUpdateMode.AnimatePhysics;
            if (m_fDirection == FACING_DIR.RIGHT || m_fDirection == FACING_DIR.LEFT)
            {
                m_v3ClimbCurve = new Vector3(m_fClimbSideDirection * m_aClimbingAnimal.m_aAnimalAnimator.GetFloat("Root_Curve_Z"), m_aClimbingAnimal.m_aAnimalAnimator.GetFloat("Root_Curve_Y"), 0);
            }
            else
            {
                m_v3ClimbCurve = new Vector3(0, m_aClimbingAnimal.m_aAnimalAnimator.GetFloat("Root_Curve_Y"), m_fClimbSideDirection * m_aClimbingAnimal.m_aAnimalAnimator.GetFloat("Root_Curve_Z"));
            }

            m_fTimer -= Time.deltaTime;

            if (m_aClimbingAnimal.m_aAnimalAnimator.GetFloat("Start_Lerp") < 0.1f)
            {
                m_aClimbingAnimal.transform.position = Vector3.Lerp(m_aClimbingAnimal.transform.position, m_tAnchorPoint.position + m_v3ClimbCurve, 0.1f);
            }
            else
            {
                m_aClimbingAnimal.transform.position = Vector3.Lerp(m_aClimbingAnimal.transform.position, m_tAnchorPoint.position + m_v3ClimbCurve, 1f);
            }
            m_aClimbingAnimal.m_fFacingDir = m_fDirection;

            switch (m_fDirection)
            {
                case FACING_DIR.RIGHT:
                    {
                        m_fClimbSideDirection = 1;
                        break;
                    }
                case FACING_DIR.LEFT:
                    {
                        m_fClimbSideDirection = -1;
                        break;
                    }
                case FACING_DIR.FRONT:
                    {
                        m_fClimbSideDirection = -1;
                        break;
                    }
                case FACING_DIR.BACK:
                    {
                        m_fClimbSideDirection = 1;
                        break;
                    }
            }

            //calibrates switching states with animation curves
            if (m_aClimbingAnimal.m_aAnimalAnimator.GetFloat("Stop_Lerp") > 0)
            {
                m_aClimbingAnimal.m_fFacingDir = FACING_DIR.NONE;
                m_aClimbingAnimal.m_bCanWalkLeft = true;
                m_aClimbingAnimal.m_bCanWalkRight = true;
                m_aClimbingAnimal.m_bTurning = false;
                m_aClimbingAnimal.m_rBody.isKinematic = false;
                m_aClimbingAnimal.m_oCurrentObject = null;
                m_aClimbingAnimal.m_aAnimalAnimator.updateMode = AnimatorUpdateMode.Normal;

                m_aClimbingAnimal.m_tCollider.gameObject.layer = LayerMask.NameToLayer("Animal");


                //if (m_fFrameTimer <= 0f)
                //{
                m_aClimbingAnimal.m_bOnGround = true;
                m_aClimbingAnimal.m_bCheckGround = true;
                m_aClimbingAnimal.m_bAutoClimbing = false;


                if ((m_aClimbingAnimal.m_bTurned && m_FinishDirection == FACING_DIR.RIGHT) || (!m_aClimbingAnimal.m_bTurned && m_FinishDirection == FACING_DIR.LEFT))
                    m_aClimbingAnimal.Turn(m_FinishDirection);

                if (m_TransitionTo != null)
                {
                    m_TransitionTo.AnimalEnter(m_aClimbingAnimal);
                }

                m_aCurrentAnimal = null;
                m_aClimbingAnimal = null;

                //}

                m_fTimer = -1f;
            }
        }
        else
        {
            if (m_aClimbingAnimal != null)
            {
                if (m_aClimbingAnimal.m_aAnimalAnimator.GetFloat("Stop_Lerp") > 0 && m_aClimbingAnimal.m_bAutoClimbing)
                {
                    m_aClimbingAnimal.m_fFacingDir = FACING_DIR.NONE;
                    m_aClimbingAnimal.m_bCanWalkLeft = true;
                    m_aClimbingAnimal.m_bCanWalkRight = true;
                    m_aClimbingAnimal.m_bTurning = false;
                    m_aClimbingAnimal.m_rBody.isKinematic = false;
                    m_aClimbingAnimal.m_oCurrentObject = null;
                    m_aClimbingAnimal.m_aAnimalAnimator.updateMode = AnimatorUpdateMode.Normal;

                    m_aClimbingAnimal.m_tCollider.gameObject.layer = LayerMask.NameToLayer("Animal");


                    //if (m_fFrameTimer <= 0f)
                    //{
                    m_aClimbingAnimal.m_bOnGround = true;
                    m_aClimbingAnimal.m_bCheckGround = true;
                    m_aClimbingAnimal.m_bAutoClimbing = false;


                    if (m_TransitionTo != null)
                    {
                        m_TransitionTo.AnimalEnter(m_aClimbingAnimal);
                    }

                    m_aCurrentAnimal = null;
                    m_aClimbingAnimal = null;
                }
            }

            //if (m_fFrameTimer > 0)
            //{
            //    m_fFrameTimer -= Time.deltaTime;

            //    if (m_fFrameTimer <= 0f)
            //    {
            //        m_aAnimal.m_bOnGround = true;               
            //        m_fGroundTimer = 0.1f;
            //        m_aAnimal.m_bAllowAutoRotate = true;
            //    }
            //}

            //if (m_fGroundTimer > 0)
            //{
            //    m_fGroundTimer -= Time.deltaTime;
            //    m_aAnimal.m_bOnGround = true;
            //    if (m_fGroundTimer <= 0f)
            //    {
            //        m_aAnimal.m_fGroundRaycastDist = m_fTempCheckDist;
            //        m_aAnimal.m_bOnGround = true;
            //        m_aAnimal.m_bAutoClimbing = false;
            //        m_aCurrentAnimal = null;
            //        m_aAnimal = null;
            //    }
            //}
        }
    }

    public void MakeClimb(Animal a_anim)
    {
        m_aCurrentAnimal = a_anim;
        DoAction();
    }

    public override void AnimalEnter(Animal a_animal)
    {
        if (m_bDoOnTrigger)
        {
            if (m_aCurrentAnimal != null)
            {
                if (m_aCurrentAnimal.m_bSelected && !m_aCurrentAnimal.m_bAutoClimbing)
                {
                    DoAction();
                }
            }
        }
    }
    //protected override void AnimalExit(Animal a_animal) { }
    public override void DoAction()
    {
        m_fTimer = m_aCurrentAnimal.m_fJumpWaitTime;
        m_aClimbingAnimal = m_aCurrentAnimal;

        if (m_aClimbingAnimal.m_oCurrentObject != null)
        {
            m_aClimbingAnimal.m_oCurrentObject.Detach();
        }
        m_aClimbingAnimal.m_aAnimalAnimator.CrossFade("AutoClimb", 0.1f);

        m_aClimbingAnimal.m_bAutoClimbing = true;
        m_aClimbingAnimal.m_bCanWalkLeft = false;
        m_aClimbingAnimal.m_bCanWalkRight = false;
        m_aClimbingAnimal.m_rBody.isKinematic = true;
        m_aClimbingAnimal.m_oCurrentObject = this;

        m_aClimbingAnimal.m_tCollider.gameObject.layer = LayerMask.NameToLayer("AnimalNoCollide");

        m_aClimbingAnimal.m_bCheckGround = false;
    }

    // fixed point a to fixed point b


}
