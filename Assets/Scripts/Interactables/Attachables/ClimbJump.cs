using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputNew;

public class ClimbJump : AttachableInteract
{
    public bool DoOnTrigger = false;
    public bool BreakSplineConnection = false;
    public FACING_DIR Direction = FACING_DIR.RIGHT;
    public Transform AnchorPoint;
    public Attachable TransitionTo;
    public ANIMAL_SIZE AutoClimbSize;
    protected override bool HeadTriggerOnly { get { return true; } set { } }

    private Vector3 ClimbCurve;
    private float timer;

    protected override void OnStart()
    {
        base.OnStart();

        CanDetach = true;
        BlocksMovement = true;
        BlocksTurn = true;
    }

    protected override bool CheckDetach()
    {
        return false;
    }

    protected override void OnAnimalEnter(Animal a_animal)
    {
        base.OnAnimalEnter(a_animal);
        if (!enabled)
            return;

        DoAction(a_animal, true);
    }

    void LateUpdate()
    {
        if (AttachedAnimal == null)
            return;

        if (timer > -1f)
        {
            AttachedAnimal.m_rBody.isKinematic = true;
            timer -= Time.deltaTime;

            float climbMult = GetMultiplier(Direction);

            AttachedAnimal.SetDirection(Direction, BreakSplineConnection);

            //Calculate the position of the animal (based on the animation)
            if (Direction == FACING_DIR.RIGHT || Direction == FACING_DIR.LEFT)
            {
                ClimbCurve = new Vector3(climbMult * AttachedAnimal.m_aAnimalAnimator.GetFloat("Root_Curve_Z"), AttachedAnimal.m_aAnimalAnimator.GetFloat("Root_Curve_Y"), 0);
            }
            else
            {
                ClimbCurve = new Vector3(0, AttachedAnimal.m_aAnimalAnimator.GetFloat("Root_Curve_Y"), climbMult * AttachedAnimal.m_aAnimalAnimator.GetFloat("Root_Curve_Z"));
            }

            if (AttachedAnimal.m_aAnimalAnimator.GetFloat("Start_Lerp") < 0.1f)
            {
                //Lerp to the start position
                AttachedAnimal.transform.position = Vector3.Lerp(AttachedAnimal.transform.position, AnchorPoint.position + ClimbCurve, 0.1f);
                AttachedAnimal.m_gqGrounder.weight = 0.01f;
            }
            else
            {
                //Lerp to the end position
                AttachedAnimal.transform.position = Vector3.Lerp(AttachedAnimal.transform.position, AnchorPoint.position + ClimbCurve, 1);
            }

            if (AttachedAnimal.m_aAnimalAnimator.GetFloat("Stop_Lerp") > 0)
            {
                //Stop climbing, reset values
                AttachedAnimal.m_fFacingDir = FACING_DIR.NONE;
                AttachedAnimal.m_rBody.isKinematic = false;
                AttachedAnimal.m_gqGrounder.weight = 1f;

                AttachedAnimal.SetColliderActive(true);

                AttachedAnimal.m_bOnGround = true;
                AttachedAnimal.m_bCheckGround = true;
                AttachedAnimal.m_bAutoClimbing = false;
                AttachedAnimal.m_bAutoClimbLarge = false;

                if (BreakSplineConnection)
                    AttachedAnimal.m_aMovement.StopSpline();


                Animal temp = AttachedAnimal;
                Detach(this);
                //In case the trigger doesn't get called on the TransitionTo object
                if (TransitionTo != null)
                    TransitionTo.AnimalEnter(temp);
                
                timer = -1;
            }
        }
        else
        {
            if (/*m_aCurrentAnimal.m_aAnimalAnimator.GetFloat("Stop_Lerp") > 0 &&*/ AttachedAnimal.m_bAutoClimbing)
            {
                //Stop climbing, reset values
                AttachedAnimal.m_fFacingDir = FACING_DIR.NONE;
                AttachedAnimal.m_rBody.isKinematic = false;

                AttachedAnimal.SetColliderActive(true);

                AttachedAnimal.m_bOnGround = true;
                AttachedAnimal.m_bCheckGround = true;
                AttachedAnimal.m_bAutoClimbing = false;
                AttachedAnimal.m_bAutoClimbLarge = false;

                if (BreakSplineConnection)
                    AttachedAnimal.m_aMovement.StopSpline();

                Animal temp = AttachedAnimal;
                Detach(this);
                //In case the trigger doesn't get called on the TransitionTo object
                if (TransitionTo != null)
                    TransitionTo.AnimalEnter(temp);
                
                timer = -1;
            }
        }
    }

    protected override void DoInteract(Animal caller)
    {
        DoAction(caller, false);
    }

    /// <summary>
    /// Do the climb. Only sometimes want to do on trigger so <paramref name="wasTrigger"/> indicates what occurred
    /// </summary>
    void DoAction(Animal caller, bool wasTrigger)
    {
        if ((DoOnTrigger && !wasTrigger) || (!DoOnTrigger && wasTrigger))
            return;

        if (AttachedAnimal != null)
            return;

        if (!TryClimb(caller))
            return;

        if (!TryDetachOther())
            return;

        Attach(caller);
        
        timer = AttachedAnimal.m_fJumpWaitTime;

        AttachedAnimal.SetColliderActive(false, this);

        AttachedAnimal.m_bCheckGround = false;
        AttachedAnimal.m_bAutoClimbing = true;
        if (AttachedAnimal.m_eSize == AutoClimbSize)
        {
            AttachedAnimal.m_bAutoClimbLarge = true;
        }
        AttachedAnimal.m_rBody.isKinematic = true;

        if (BreakSplineConnection)
            AttachedAnimal.m_aMovement.StopSpline();

        AnimalsIn.RemoveAll(AttachedAnimal);
    }

    /// <summary>
    /// Default always returns true. Meant to allow children to prevent climbing
    /// </summary>
    protected virtual bool TryClimb(Animal animal)
    {
        return true;
    }

    /// <summary>
    /// Get the animation multiplier for <see cref="FACING_DIR"/>
    /// </summary>
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