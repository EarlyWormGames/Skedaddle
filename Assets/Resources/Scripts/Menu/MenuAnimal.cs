using UnityEngine;
using System.Collections;

public class MenuAnimal : Area
{
    public ANIMAL_NAME m_aName;
    public MeshRenderer m_mCard;
    public Vector2 m_v2IdleChangeTime;

    [Header("Move In Settings")]
    public Transform m_tMovePoint;
    public Transform m_tLookPoint;
    public float m_fMyFocus = 1f;
    public float m_fMyFocalSize = 1f;

    [Header("Fading Settings")]
    public AnimationCurve m_aFadeCurve;
    public float m_fFadeInSpeed = 1f;
    public float m_fFadeOutSpeed = 0.5f;


    private float m_fFadeTimer = -1f;
    private float m_fIdleTimer = 10f;
    private bool m_bFadeIn = true;
    private Animator m_aAnimalAnimator;

    // Use this for initialization
    void Start()
    {
        gameObject.SetActive(SaveManager.CheckAnimal(m_aName));
        m_aAnimalAnimator = GetComponent<Animator>();

        if (m_mCard == null)
            return;

        Color col = m_mCard.material.color;
        col.a = 0;
        m_mCard.material.color = col;
    }

    protected override void OnUpdate()
    {
        m_fIdleTimer -= Time.deltaTime;
        if (m_fIdleTimer < 0)
        {
            m_aAnimalAnimator.SetInteger("IdleNo", Mathf.RoundToInt(Random.Range(0.51f, 2.49f)));
            m_fIdleTimer = Random.Range(m_v2IdleChangeTime.x, m_v2IdleChangeTime.y);
        }

        if (m_bSelected)
        {
            if (Keybinding.GetKeyDown("Pause") || Controller.GetButtonDown(ControllerButtons.B))
            {
                Exit();
            }
        }

        if (m_fFadeTimer >= 0f)
        {
            m_fFadeTimer += Time.deltaTime;

            m_fFadeTimer = Mathf.Min(m_fFadeTimer, (m_bFadeIn ? m_fFadeInSpeed : m_fFadeOutSpeed));

            float t = m_fFadeTimer / (m_bFadeIn ? m_fFadeInSpeed : m_fFadeOutSpeed);
            t = m_aFadeCurve.Evaluate(t);
            if (m_mCard != null)
            {
                Color col = m_mCard.material.color;
                col.a = m_bFadeIn ? t : 1 - t;
                m_mCard.material.color = col;
            }
        }
    }

    public override void Select()
    {
        m_IsActive = true;
        Selected = this;
    }

    public override void Deselect()
    {
        m_IsActive = false;
    }

    public void FadeReady()
    {
        if (m_fFadeTimer < 0f)
            m_fFadeTimer = 0f;
        MenuCam.instance.m_BackButton.SetActive(true);
    }

    public override void Click()
    {
        m_bFadeIn = true;
        
        m_bSelected = true;
        Camera.main.GetComponent<MenuCam>().m_bPlaying = true;

        CameraController.Instance.m_tLookAt = m_tLookPoint;
        CameraController.Instance.GoToPoint(m_tMovePoint.position, float.PositiveInfinity, m_fMoveToTime);
        CameraController.onEnd += FadeReady;
        m_IsActive = false;

        CurrentArea = this;
    }

    public override void Exit()
    {
        base.Exit();
        CameraController.onEnd -= FadeReady;

        m_bFadeIn = false;

        //Inverse the fade timer to be fading out
        float t = 1 - (m_fFadeTimer / m_fFadeInSpeed);
        m_fFadeTimer = t * m_fFadeOutSpeed;
        
        CameraController.Instance.GoToPoint(CameraController.Instance.m_v3StartPos, float.PositiveInfinity, m_fMoveToTime);
        CameraController.Instance.m_tLookAt = null;
        //GetComponent<GUINavigation>().Select();
        MenuCam.instance.m_BackButton.SetActive(false);
    }
}
