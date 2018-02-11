﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputNew;

public class ClimbJump : ActionObject
{
    public bool DoOnTrigger = false;
    public bool AllowDetach = true;
    public FACING_DIR Direction = FACING_DIR.RIGHT;
    public FACING_DIR FinishDirection = FACING_DIR.RIGHT;
    public Transform AnchorPoint;
    public ActionObject TransitionTo;
    
    private Vector3 ClimbCurve;
    private MainMapping input;
    private float timer;

    protected override void OnStart()
    {
        base.OnStart();
        input = GameManager.Instance.GetComponent<PlayerInput>().GetActions<MainMapping>();
    }

    protected override void OnCanTrigger()
    {
        if (!DoOnTrigger)
        {
            if (input.moveY.rawValue > 0)
            {
                DoAction();
            }
        }
        else
        {
            DoAction();
        }
    }

    void LateUpdate()
    {
        if (m_aCurrentAnimal == null)
            return;

        if (timer > -1f)
        {
            m_aCurrentAnimal.m_rBody.isKinematic = true;
            timer -= Time.deltaTime;

            float climbMult = GetMultiplier(Direction);
            m_aCurrentAnimal.m_fFacingDir = Direction;

            //Calculate the position of the animal (based on the animation)
            if (Direction == FACING_DIR.RIGHT || Direction == FACING_DIR.LEFT)
            {
                ClimbCurve = new Vector3(climbMult * m_aCurrentAnimal.m_aAnimalAnimator.GetFloat("Root_Curve_Z"), m_aCurrentAnimal.m_aAnimalAnimator.GetFloat("Root_Curve_Y"), 0);
            }
            else
            {
                ClimbCurve = new Vector3(0, m_aCurrentAnimal.m_aAnimalAnimator.GetFloat("Root_Curve_Y"), climbMult * m_aCurrentAnimal.m_aAnimalAnimator.GetFloat("Root_Curve_Z"));
            }

            if (m_aCurrentAnimal.m_aAnimalAnimator.GetFloat("Start_Lerp") < 0.1f)
            {
                //Lerp to the start position
                m_aCurrentAnimal.transform.position = Vector3.Lerp(m_aCurrentAnimal.transform.position, AnchorPoint.position + ClimbCurve, 0.1f);
            }
            else
            {
                //Lerp to the end position
                m_aCurrentAnimal.transform.position = Vector3.Lerp(m_aCurrentAnimal.transform.position, AnchorPoint.position + ClimbCurve, 2);
            }

            if (m_aCurrentAnimal.m_aAnimalAnimator.GetFloat("Stop_Lerp") > 0)
            {
                //Stop climbing, reset values
                m_aCurrentAnimal.m_fFacingDir = FACING_DIR.NONE;
                m_aCurrentAnimal.m_rBody.isKinematic = false;
                m_aCurrentAnimal.m_oCurrentObject = null;

                m_aCurrentAnimal.m_tCollider.gameObject.layer = LayerMask.NameToLayer("Animal");

                m_aCurrentAnimal.m_bOnGround = true;
                m_aCurrentAnimal.m_bCheckGround = true;
                m_aCurrentAnimal.m_bAutoClimbing = false;

                //Turn the animal
                if ((m_aCurrentAnimal.m_bTurned && FinishDirection == FACING_DIR.RIGHT) || (!m_aCurrentAnimal.m_bTurned && FinishDirection == FACING_DIR.LEFT))
                    m_aCurrentAnimal.Turn(FinishDirection);

                //In case the trigger doesn't get called on the TransitionTo object
                if (TransitionTo != null)
                    TransitionTo.AnimalEnter(m_aCurrentAnimal);

                m_aCurrentAnimal = null;
                timer = -1;
            }
        }
        else
        {
            if (m_aCurrentAnimal.m_aAnimalAnimator.GetFloat("Stop_Lerp") > 0 && m_aCurrentAnimal.m_bAutoClimbing)
            {
                //Stop climbing, reset values
                m_aCurrentAnimal.m_fFacingDir = FACING_DIR.NONE;
                m_aCurrentAnimal.m_rBody.isKinematic = false;
                m_aCurrentAnimal.m_oCurrentObject = null;

                m_aCurrentAnimal.m_tCollider.gameObject.layer = LayerMask.NameToLayer("Animal");

                m_aCurrentAnimal.m_bOnGround = true;
                m_aCurrentAnimal.m_bCheckGround = true;
                m_aCurrentAnimal.m_bAutoClimbing = false;

                //Turn the animal
                if ((m_aCurrentAnimal.m_bTurned && FinishDirection == FACING_DIR.RIGHT) || (!m_aCurrentAnimal.m_bTurned && FinishDirection == FACING_DIR.LEFT))
                    m_aCurrentAnimal.Turn(FinishDirection);

                //In case the trigger doesn't get called on the TransitionTo object
                if (TransitionTo != null)
                    TransitionTo.AnimalEnter(m_aCurrentAnimal);

                m_aCurrentAnimal = null;
                timer = -1;
            }
        }
    }

    public override void DoAction()
    {
        if (m_aCurrentAnimal != null)
            return;

        if (Animal.CurrentAnimal.m_oCurrentObject != null && Animal.CurrentAnimal.m_oCurrentObject != this)
        {
            if (!AllowDetach)
                return;
            Animal.CurrentAnimal.m_oCurrentObject.Detach();
        }

        m_aCurrentAnimal = Animal.CurrentAnimal;
        timer = m_aCurrentAnimal.m_fJumpWaitTime;

        m_aCurrentAnimal.m_oCurrentObject = this;
        m_aCurrentAnimal.m_tCollider.gameObject.layer = LayerMask.NameToLayer("AnimalNoCollide");
        m_aCurrentAnimal.m_bCheckGround = false;
        m_aCurrentAnimal.m_bAutoClimbing = true;
        m_aCurrentAnimal.m_rBody.isKinematic = true;
    }

    public static float GetMultiplier(FACING_DIR direction)
    {
        switch (direction)
        {
            case FACING_DIR.RIGHT:
                return 1;
            case FACING_DIR.LEFT:
                return -1;
            case FACING_DIR.FRONT:
                return -1;
            case FACING_DIR.BACK:
                return 1;
        }
        return 1;
    }
}