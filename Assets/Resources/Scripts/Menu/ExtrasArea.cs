using UnityEngine;
using System.Collections;

public class ExtrasArea : Area
{
    public Transform m_tLookPoint;
    public Transform m_tMovePoint;

    public float m_fMyFocus = 1f;
    public float m_fMyFocalSize = 1f;
    
    private Animator m_aController;

    void Start()
    {
        m_aController = GetComponent<Animator>();
    }

    protected override void OnUpdate()
    {

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

    public override void Click()
    {
        m_bSelected = true;
        Camera.main.GetComponent<MenuCam>().m_bPlaying = true;
        
        CameraController.Instance.m_tLookAt = m_tLookPoint;
        CameraController.Instance.GoToPoint(m_tMovePoint.position, float.PositiveInfinity, m_fMoveToTime);

        m_aController.SetBool("Opened", true);
        m_aController.speed = 0.8f;

        CameraController.onEnd += End;
        m_IsActive = false;
        CurrentArea = this;
    }

    public void End()
    {
        EWApplication.LoadLevel("Extras");
    }
}
