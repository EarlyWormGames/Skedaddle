using UnityEngine;
using UnityEngine.UI;
using System;

public class Area : GUINavigation
{
    public static Area CurrentArea;

    public float m_fMoveToTime = 1f;


    protected bool m_bSelected = false;

    // Use this for initialization
    void Awake()
    {
        enabled = true;
    }

    public virtual void Exit()
    {
        m_bSelected = false;
        Select();
        Camera.main.GetComponent<MenuCam>().m_bPlaying = false;
        Selected = this;

        CameraController.Instance.GoToPoint(CameraController.Instance.m_v3StartPos);
        CameraController.Instance.m_tLookAt = null;

        MenuCam.instance.m_BackButton.SetActive(false);

        CurrentArea = null;
    }

    public virtual void SelectLevel(Level a_level)
    {

    }

    public virtual void PrevLevel() { }
    public virtual void NextLevel() { }
}
