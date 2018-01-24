using UnityEngine;
using System.Collections;

public class LadderStopper : ActionObject
{
    public bool m_bIsTop = false;
    public bool m_isFrayedRope = false;
    public HingeJoint m_rFrayedRope;
    public ClimbJump m_cEndClimb;

    //Inherited functions

    protected override void OnStart()
    {
        m_aRequiredAnimal = ANIMAL_NAME.LORIS;
        m_bUseDefaultAction = false;
    }

    protected override void OnUpdate()
    {
        if (m_aCurrentAnimal == null || m_cEndClimb == null)
            return;

        Loris lor = m_aCurrentAnimal.GetComponent<Loris>();
        if (lor == null)
        {
            return;
        }

        if (lor.m_oCurrentObject == null)
            return;

        if (lor.m_oCurrentObject.GetType() != typeof(LadderObject))
            return;

        if ((Keybinding.GetKey("MoveUp") || Controller.GetDpad(ControllerDpad.Up) || Controller.GetStick(true).y > 0 || lor.m_EyeUp) && m_aCurrentAnimal.m_bSelected)
        {
            ((LadderObject)lor.m_oCurrentObject).StopClimbing(true);
            lor.m_rBody.velocity = Vector3.zero;
            m_cEndClimb.MakeClimb(lor);
        }
    }

    public override void AnimalEnter(Animal a_animal)
    {
        Loris lor = a_animal.GetComponent<Loris>();
        if (lor == null)
        {
            return;
        }

        if (lor.m_oCurrentObject == null)
            return;

        if (lor.m_oCurrentObject.GetType() != typeof(LadderObject))
            return;

        LadderObject ladder = (LadderObject)lor.m_oCurrentObject;

        if (m_bIsTop)
        {
            lor.m_bCanClimbUp = false;
        }
        else
        {
            lor.m_bCanClimbDown = false;
            if (m_isFrayedRope)
            {
                ladder.StopClimbing();
                m_rFrayedRope.connectedBody = null;
                Destroy(m_rFrayedRope);
            }
        }

        //if (m_cEndClimb != null)
        //{
        //    ((LadderObject)lor.m_oCurrentObject).StopClimbing();
        //    lor.m_rBody.velocity = Vector3.zero;
        //    m_cEndClimb.MakeClimb(lor);
        //}
    }

    protected override void AnimalExit(Animal a_animal)
    {
        Loris lor = a_animal.GetComponent<Loris>();
        if (lor == null)
        {
            return;
        }

        if (m_bIsTop)
        {
            lor.m_bCanClimbUp = true;
        }
        else
        {
            lor.m_bCanClimbDown = true;
        }
    }

    //protected override void DoAction() { }
}
