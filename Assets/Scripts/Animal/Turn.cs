using UnityEngine;
using System.Collections;

public class Turn : StateMachineBehaviour {

    public float clipLength;

    internal Animal m_aAnimal;

    private float m_stateSpeed;
    private bool m_bTurnAgain;
    private float m_fAnimationLength;
    private float m_fAnimationSpeed;
    private bool m_startTranstion;
    private bool m_endTransition;

    /// <summary>
    /// begin the transition of the animal turning around 
    /// </summary>
    /// <param name="animator"></param>
    /// <param name="stateInfo"></param>
    /// <param name="layerIndex"></param>
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        m_endTransition = false;
        m_startTranstion = true;
        m_aAnimal = animator.GetComponentInParent<Animal>();
        m_aAnimal.m_bFinishTurn = false;
        if (!m_bTurnAgain)
        {
            m_stateSpeed = stateInfo.speedMultiplier;
        }
        m_bTurnAgain = false;

        m_aAnimal.m_fAnimationSpeed = stateInfo.speed * stateInfo.speedMultiplier;
        m_aAnimal.m_fAnimationLength = 1 / clipLength;

    }

    /// <summary>
    /// the actual turn
    /// </summary>
    /// <param name="animator"></param>
    /// <param name="stateInfo"></param>
    /// <param name="layerIndex"></param>
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (m_aAnimal.m_bSelected)
        {
            //if (Keybinding.GetKeyDown("MoveRight") || Controller.GetDpadDown(ControllerDpad.Right) || Controller.GetStickPositionDown(true, ControllerDpad.Right))
            //{
            //    animator.SetFloat("Turn_Speed", 3);
            //    if (m_aAnimal.m_bTurned)
            //    {
            //        m_bTurnAgain = true;
            //    }
            //}
            //if (Keybinding.GetKeyDown("MoveLeft") || Controller.GetDpadDown(ControllerDpad.Left) || Controller.GetStickPositionDown(true, ControllerDpad.Left))
            //{
            //    animator.SetFloat("Turn_Speed", 3);
            //    if (!m_aAnimal.m_bTurned)
            //    {
            //        m_bTurnAgain = true;
            //    }
            //}
        }

        m_aAnimal.m_fAnimationSpeed = stateInfo.speed * stateInfo.speedMultiplier;
        if (animator.IsInTransition(0))
        {
            if (!m_startTranstion && !m_endTransition)
            {
                m_aAnimal.m_bFinishTurn = true;
                m_aAnimal.FinishTurning();
                m_endTransition = true;
                if (m_bTurnAgain)
                {
          
                    m_aAnimal.Turn(180);
                    return;
                }

                animator.SetFloat("Turn_Speed", m_stateSpeed);

            }
        } else
        {
            m_startTranstion = false;
        }
    }

    /// <summary>
    /// set the turn speed in the animator.
    /// </summary>
    /// <param name="animator"></param>
    /// <param name="stateInfo"></param>
    /// <param name="layerIndex"></param>
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetFloat("Turn_Speed", m_stateSpeed);
    }
}
