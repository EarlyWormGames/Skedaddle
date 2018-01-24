using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public enum FACING_DIR
{
    NONE,
    FRONT,
    BACK,
    LEFT,
    RIGHT,
}

public class LadderObject : ActionObject
{
    public Transform m_tCentre;
    public LadderObject m_LeftRope;
    public LadderObject m_RightRope;
    public Transform[] m_tSteps;
    public FACING_DIR m_lFacing;
    public bool m_bIsRope = false;
    public bool m_bIsTop = false;
    public bool m_bMoveBack = false;
    public bool m_bCanShimmy;

    public LadderStopper m_StopperBottom;
    public LadderStopper m_StopperTop;

    //==================================
    //          Private Vars
    //================================== 
    private Loris m_lAnimal;
    private float m_fTopDelay = 0.5f;
    private float m_fShimmyTimer = 0;
    private bool m_bMoveTo = false;
    private bool m_bTopEnter = false;
    private float m_fStartZ;

    //Inherited functions

    protected override void OnStart()
    {
        if (m_LeftRope != null || m_RightRope != null)
        {
            m_bCanShimmy = true;
        }
        else
        {
            m_bCanShimmy = false;
        }
    }
    protected override void OnUpdate()
    {
        if (m_aCurrentAnimal != null)
        {
            if (m_aCurrentAnimal.m_oCurrentObject == null || m_aCurrentAnimal.m_oCurrentObject == this)
            {
                m_lAnimal = m_aCurrentAnimal.GetComponent<Loris>();
            }
        }

        if (m_lAnimal != null)
        {
            SetTimer(m_lAnimal.m_EyeUp || m_bEyetrackSelected);

            if ((Keybinding.GetKeyDown("MoveUp") || Keybinding.GetKeyDown("MoveDown") ||
                Controller.GetDpadDown(ControllerDpad.Up) || Controller.GetStickPositionDown(true, ControllerDpad.Up) ||
                Controller.GetDpadDown(ControllerDpad.Down) || Controller.GetStickPositionDown(true, ControllerDpad.Down) ||
                m_fGazeTimer >= EWEyeTracking.shortHoldTime) &&
                m_lAnimal.m_bSelected && !m_lAnimal.m_bClimbing && !m_bMoveTo)
            {
                SetTimer(false);
                m_bMoveTo = true;
                if (!m_lAnimal.m_bOnGround)
                {
                    m_lAnimal.m_rBody.velocity = Vector3.zero;
                    m_lAnimal.transform.Translate((m_tCentre.position - m_lAnimal.transform.position).x, 0, 0);
                    StartClimbing(m_lAnimal);
                }
            }
            if (m_lAnimal.m_bClimbing)
            {
                if (m_bEyetrackSelected)
                    Highlighter.Selected = null;

                SetTimer(false);

                if (m_lAnimal.m_bSelected)
                {
                    if ((Keybinding.GetKey("MoveRight") || Controller.GetDpad(ControllerDpad.Right) || Controller.GetStick(true).x > 0.2f) && m_RightRope != null)
                    {
                        if (Keybinding.GetKeyDown("Action") || Controller.GetButtonDown(ControllerButtons.A))
                        {
                            m_fShimmyTimer = 0;
                            m_RightRope.m_lAnimal = m_lAnimal;
                            m_RightRope.StartClimbing(m_lAnimal);
                            m_RightRope.m_fStartZ = m_fStartZ;
                            m_lAnimal.m_aAnimalAnimator.SetTrigger("Shimmy Jump");
                            m_aCurrentAnimal = null;
                            m_lAnimal = null;
                            return;
                        }
                    }
                    else if ((Keybinding.GetKey("MoveLeft") || Controller.GetDpad(ControllerDpad.Left) || Controller.GetStick(true).x < -0.2f) && m_LeftRope != null)
                    {
                        if (Keybinding.GetKeyDown("Action") || Controller.GetButtonDown(ControllerButtons.A))
                        {
                            m_fShimmyTimer = 0;
                            m_LeftRope.m_lAnimal = m_lAnimal;
                            m_LeftRope.StartClimbing(m_lAnimal);
                            m_LeftRope.m_fStartZ = m_fStartZ;
                            m_lAnimal.m_aAnimalAnimator.SetTrigger("Shimmy Jump");
                            m_aCurrentAnimal = null;
                            m_lAnimal = null;
                            return;
                        }
                    }
                    else if ((Keybinding.GetKeyDown("Action") || Controller.GetButtonDown(ControllerButtons.A)))
                    {
                        StopClimbing(true);
                        return;
                    }
                }
                else
                {
                    if ((Keybinding.GetKeyDown("Action") || Controller.GetButtonDown(ControllerButtons.A) || m_fGazeTimer >= EWEyeTracking.shortHoldTime))
                    {
                        SetTimer(false);
                        StartClimbing(m_lAnimal);
                        return;
                    }
                }
            }

            if (m_bMoveTo)
            {
                m_lAnimal.m_rBody.velocity -= new Vector3(0, m_lAnimal.m_rBody.velocity.y, 0);
                if (m_bIsTop)
                {
                    m_bTopEnter = true;
                }
                m_bQuickExitFix = true;
                if (m_lAnimal.MoveTo(m_tCentre.position.x))
                {
                    StartClimbing(m_lAnimal);
                }
            }
            else if (m_lAnimal.m_bClimbing)
            {
                if (m_fShimmyTimer < 1)
                {
                    m_fShimmyTimer += Time.deltaTime * 2;
                }

                if (m_bTopEnter)
                {
                    m_fTopDelay -= Time.deltaTime;
                }
                m_bQuickExitFix = false;
                m_lAnimal.transform.position = new Vector3(Mathf.Lerp(m_lAnimal.transform.position.x, m_tCentre.position.x, m_fShimmyTimer), m_bTopEnter ? Mathf.Lerp(m_lAnimal.transform.position.y, m_tCentre.position.y, Time.deltaTime * 2) : m_lAnimal.transform.position.y, m_tCentre.position.z);

                if (m_fTopDelay < 0)
                {
                    m_fTopDelay = 0.5f;
                    m_bTopEnter = false;

                    m_lAnimal = null;
                    m_aCurrentAnimal = null;
                    return;
                }
            }
            else if (m_bMoveTo && !m_lAnimal.m_bOnGround)
            {
                m_lAnimal.m_rBody.useGravity = false;
            }
        }
    }

    public void StartClimbing(Loris l_animal)
    {
        m_bMoveTo = false;
        l_animal.m_oCurrentObject = this;

        l_animal.m_bClimbing = true;
        l_animal.m_bCanWalkLeft = false;
        l_animal.m_bCanWalkRight = false;

        l_animal.m_rBody.useGravity = false;
        //m_lAnimal.m_rBody.isKinematic = true;
        //m_lAnimal.transform.parent = transform;
        l_animal.m_tCollider.gameObject.layer = LayerMask.NameToLayer("AnimalNoCollide");

        l_animal.ClimbChange(m_lFacing, m_bIsRope);

        m_fStartZ = l_animal.transform.position.z;

        if (m_StopperBottom != null)
        {
            BoxCollider boxBottom = m_StopperBottom.GetComponent<BoxCollider>();
            if (boxBottom.bounds.Intersects(l_animal.m_tCollider.GetComponent<Collider>().bounds))
            {
                l_animal.m_bCanClimbDown = false;
            }
        }
        if (m_StopperTop != null)
        {
            BoxCollider boxTop = m_StopperTop.GetComponent<BoxCollider>();
            if (boxTop.bounds.Intersects(l_animal.m_tCollider.GetComponent<Collider>().bounds))
            {
                l_animal.m_bCanClimbUp = false;
            }
        }
    }

    public void StopClimbing(bool a_DestroyAnim = false)
    {
        m_lAnimal.m_oCurrentObject = null;
        m_lAnimal.m_bClimbing = false;
        m_lAnimal.m_bCanWalkLeft = true;
        m_lAnimal.m_bCanWalkRight = true;


        m_lAnimal.m_rBody.useGravity = true;
        m_lAnimal.m_rBody.isKinematic = false;
        m_lAnimal.transform.parent = null;
        m_lAnimal.m_tCollider.gameObject.layer = LayerMask.NameToLayer("Animal");
        m_lAnimal.ClimbChange(FACING_DIR.NONE, false);

        if (a_DestroyAnim)
        {
            if (m_bMoveTo)
                m_lAnimal.StopMoveTo();
            m_bMoveTo = false;
        }

        Vector3 pos = m_lAnimal.transform.position;
        if (m_bMoveBack) pos.z = m_fStartZ;
        m_lAnimal.transform.position = pos;

        m_lAnimal = null;
    }

    public override void AnimalEnter(Animal a_animal)
    {
        if (a_animal.GetComponent<Loris>() == null)
            return;

        Loris loris = (Loris)a_animal;


        if (loris.m_bClimbing)
        {
            loris.m_oCurrentObject = this;
            if (!m_bIsRope)
            {
                loris.m_aAnimalAnimator.SetFloat("OnRope", 0);
            }
            else
            {
                loris.m_aAnimalAnimator.SetFloat("OnRope", 1);
            }
        }
    }

    protected override void AnimalExit(Animal a_animal)
    {
        if (m_lAnimal == null)
            return;

        if (m_lAnimal.m_tCollider.gameObject.layer != LayerMask.NameToLayer("AnimalNoCollide") || m_lAnimal.m_oCurrentObject != this)
        {
            if (m_bMoveTo)
                m_lAnimal.StopMoveTo();

            m_lAnimal = null;
            m_bQuickExitFix = false;
            m_bMoveTo = false;
        }
    }
}
