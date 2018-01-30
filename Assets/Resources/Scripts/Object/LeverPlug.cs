using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class LeverPlug : ActionObject
{
    public UnityEvent OnLeverForward, OnLeverReverse;

    [Header("Quick Camera Pan")]
    public Transform m_tCameraMove;
    public Transform m_tCameraFocus;
    public float m_fWaitTimer = 1f;
    public float m_fLookTime = 1f;

    [Header("Animation")]
    [Tooltip("Where the animal will position itself to for animating")]
    public Transform m_tMovePoint;
    public bool m_bIsPlug;
    public float m_fHoldTime = 0f;
    public float m_fSoundTime = 0.2f;
    [Tooltip("Which direction should the animal face while animating?")]
    public FACING_DIR m_fDirection = FACING_DIR.BACK;
    public FACING_DIR m_FinishDirection;
    
    [Tooltip("Can the lever be used multiple times?")]
    public bool m_bUseOnce = false;

    protected bool m_bOn = false;

    protected bool m_bIsAnimating = false;
    protected float m_fSoundTimer = -1f;

    private bool m_bFirst = true;

    private bool m_bMoveTo = false;
    private bool m_bUsed = false;

    //Inherited functions

    protected override void OnStart()
    {
        //m_fTimer = m_fHoldTime;
    }

    protected override void OnUpdate()
    {
        if (!m_bUseDefaultAction)
            return;

        if (m_aCurrentAnimal == null)
            return;
        else if ((m_aCurrentAnimal.m_oCurrentObject != this && m_aCurrentAnimal.m_oCurrentObject != null) || !m_aCurrentAnimal.m_bSelected)
            return;

        if ((Keybinding.GetKeyDown("Action") || Controller.GetButtonDown(ControllerButtons.A) || EWEyeTracking.GetFocusedObject() == m_aCurrentAnimal.m_GazeObject) && (m_bUseOnce? !m_bUsed : true))
        {
            m_bUsed = true;
            m_bMoveTo = true;
        }

        if (m_bIsAnimating)
        {
            DoAnimation();
        }

        if (m_bMoveTo)
        {
            if (m_aCurrentAnimal.MoveTo(m_tMovePoint.position.x))
            {
                m_fSoundTimer = 0f;

                m_bMoveTo = false;
                m_aCurrentAnimal.m_bCanWalkLeft = false;
                m_aCurrentAnimal.m_bCanWalkRight = false;
                m_aCurrentAnimal.m_fFacingDir = m_fDirection;
                StartAnimation();
            }
        }
    }
    public override void AnimalEnter(Animal a_animal)
    {
        m_aCurrentAnimal.m_eExtras.m_lpLever = this;
    }
    //protected override void AnimalExit(Animal a_animal){}
    public override void DoAction()
    {
        m_bOn = !m_bOn;

        if (m_bOn)
            OnLeverForward.Invoke();
        else
            OnLeverReverse.Invoke();

        if (m_bFirst)
        {
            if (m_tCameraMove != null)
            {
                CameraController.Instance.ViewObject(m_tCameraMove.gameObject, m_fWaitTimer, m_fLookTime, m_tCameraFocus);

            }
            m_bFirst = false;
        }
    }

    public override void DoActionOn()
    {
        enabled = false;
    }

    public override void DoActionOff()
    {
        enabled = true;
    }

    public void TurnOff()
    {
        m_bOn = false;
        OnLeverReverse.Invoke();

        DoAnimation();
    }

    public void TurnOn()
    {
        m_bOn = true;
        OnLeverForward.Invoke();

        DoAnimation();
    }

    public void StartAnimation()
    {
        if(m_bIsPlug && (Poodle)m_aCurrentAnimal)
        {
            m_aCurrentAnimal.m_bPullingPlug = true;
        }

        if (!m_bIsPlug)
        {
            if (!m_bOn)
            {
                m_aCurrentAnimal.m_bPullingLeverOn = true;
            }
            else
            {
                m_aCurrentAnimal.m_bPullingLeverOff = true;
            }
        }
    }

    public void StopAnimation()
    {
        m_bIsAnimating = false;
        m_aCurrentAnimal.m_bPullingPlug = false;
        m_aCurrentAnimal.m_bPullingLeverOn = false;
        m_aCurrentAnimal.m_bPullingLeverOff = false;
        m_aCurrentAnimal.m_bCanWalkLeft = true;
        m_aCurrentAnimal.m_bCanWalkRight = true;
        m_aCurrentAnimal.m_fFacingDir = FACING_DIR.NONE;

        if ((m_aCurrentAnimal.m_bTurned && m_FinishDirection == FACING_DIR.RIGHT) || (!m_aCurrentAnimal.m_bTurned && m_FinishDirection == FACING_DIR.LEFT))
            m_aCurrentAnimal.Turn(m_FinishDirection);
    }
}
