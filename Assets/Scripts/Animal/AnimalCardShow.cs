using UnityEngine;
using System.Collections;

public class AnimalCardShow : MonoBehaviour
{
    public ANIMAL_NAME m_Animal;
    public GameObject m_CardMesh;
    
    public Transform m_tMovePoint;
    public Transform m_tLookPoint;
    public float m_fTransitionTime;
    public bool m_MoveCamera = false;
    public Animator m_Animator;

    private bool m_bShowing = false;
    private bool m_CameraMoved = false;
    private bool m_bDone = false;

    public void Show()
    {
        if (m_Animator == null)
        {
            m_CardMesh.SetActive(true);
        }
        else
            m_Animator.SetTrigger("Show");

        m_bShowing = true;
        AnimalController.Instance.ChangeAnimal(m_Animal);
        AnimalController.Instance.Deselect();

        if (m_MoveCamera)
        {
            m_CameraMoved = true;
            CameraController.Instance.GoToPoint(m_tMovePoint.position, float.PositiveInfinity, m_fTransitionTime);
            CameraController.Instance.m_tLookAt = m_tLookPoint;
        }
    }

    public void Hide()
    {
        if (m_Animator == null)
        {
            m_CardMesh.SetActive(false);
        }
        else
            m_Animator.SetTrigger("Hide");

        m_bShowing = false;
        AnimalController.Instance.Reselect();

        if (m_CameraMoved)
        {
            CameraController.Instance.FollowAnimal();
            CameraController.Instance.m_tLookAt = null;
        }
    }

    void Start()
    {
        if (m_Animator == null)
        {
            m_CardMesh.SetActive(false);
        }
    }

    void Update()
    {
        if (m_bShowing)
        {
            //if (Keybinding.GetKeyDown("Action") || Controller.GetButtonDown(ControllerButtons.A))
            //{
            //    Hide();
            //}
        }
    }
}