using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Area1 : Area
{
    public Level m_lLvl1;
    public float m_LerpSpeed = 1f;
    public BezierSpline m_LookSpline;
    public Transform m_tLookPoint;

    private Level m_SelectedLevel = null;
    private bool m_bMoving = false;
    private float m_fTimer;

    protected override void OnUpdate()
    {
        if (m_bSelected)
        {
            //if (Keybinding.GetKeyDown("Pause") || Controller.GetButtonDown(ControllerButtons.B))
            //{
            //    Exit();
            //    return;
            //}
            Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, m_SelectedLevel.m_tMovePoint.position, Time.deltaTime * m_LerpSpeed);
        }

        if (m_bMoving)
        {
            m_fTimer += Time.deltaTime;
            m_tLookPoint.position = m_LookSpline.GetPoint(m_fTimer / m_fMoveToTime);
            CameraController.Instance.m_tLookAt = m_tLookPoint;
        }
    }

    public override void Select()
    {
        //m_gTarg.FadeIn();


        m_IsActive = true;
        Selected = this;
    }

    public override void Deselect()
    {
        //m_gTarg.FadeOut();
        m_IsActive = false;
    } 

    public override void Click()
    {
        Deselect();
        m_IsActive = false;
        Camera.main.GetComponent<MenuCam>().m_bPlaying = true;

        CameraController.Instance.GoToPoint(m_lLvl1.m_tMovePoint.position, float.PositiveInfinity, m_fMoveToTime);
        CameraController.onEnd += OnEnd;
        m_bMoving = true;
        m_fTimer = 0f;

        CurrentArea = this;
    }

    public override void Exit()
    {
        if (m_SelectedLevel != null)
            m_SelectedLevel.enabled = false;

        m_bSelected = false;
        Select();
        Camera.main.GetComponent<MenuCam>().m_bPlaying = false;
        Selected = this;

        CameraController.Instance.GoToPoint(CameraController.Instance.m_v3StartPos);
        CameraController.Instance.m_tLookAt = null;
        CameraController.onEnd -= OnEnd;

        MenuCam.instance.m_BackButton.SetActive(false);

    }

    void OnEnd()
    {
        SelectLevel(m_lLvl1);
        m_bSelected = true;
        m_bMoving = false;
        MenuCam.instance.m_BackButton.SetActive(true);
    }

    public override void SelectLevel(Level a_level)
    {
        if (m_SelectedLevel != null)
            m_SelectedLevel.enabled = false;

        m_SelectedLevel = a_level;
        m_SelectedLevel.enabled = true;
        m_SelectedLevel.m_aParent = this;

        CameraController.Instance.m_tLookAt = m_SelectedLevel.m_tLookPoint;
    }

    public override void PrevLevel()
    {
        if (m_SelectedLevel != null)
        {
            if (m_SelectedLevel.m_lPrevLevel != null)
                SelectLevel(m_SelectedLevel.m_lPrevLevel);
        }
    }

    public override void NextLevel()
    {
        if (m_SelectedLevel != null)
        {
            if (m_SelectedLevel.m_lNextLevel != null)
                SelectLevel(m_SelectedLevel.m_lNextLevel);
        }
    }
}
