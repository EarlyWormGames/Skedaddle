using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class OptionsArea : Area
{
    public Transform m_tMovePoint;
    public Transform m_tLookPoint;
    public float m_fMyFocus = 1f;
    public float m_fMyFocalSize = 1f;

    public Animator m_aController;

    public TrunkFolder[] m_atFolders;
    public float m_fOpenWait = 1f;
    public float m_fStartDiff = 0.2f;
    public LayerMask m_lOptionsLayer;

    private float m_fOpenTimer = -1f;

    private int m_iSelectedFolder;
    private int m_iStartFolder = -1;


    protected override void OnUpdate()
    {
        if (m_bSelected)
        {/*
            if (Keybinding.GetKeyDown("Pause") || Keybinding.GetButton(GPadButtons.B, XInputDotNetPure.PlayerIndex.One, XInputDotNetPure.GamePadDeadZone.None, 0) == GPButtonState.Down)
            {
                Exit();
            }*/

            //if (Keybinding.GetKeyDown("Pause") || Controller.GetButtonDown(ControllerButtons.B))
            //{
            //    Exit();
            //}

            if (m_iStartFolder >= 0)
            {
                m_fOpenTimer += Time.deltaTime;
                if (m_fOpenTimer >= m_fOpenWait + (m_fStartDiff * m_iStartFolder))
                {
                    m_atFolders[(m_atFolders.Length - 1) - m_iStartFolder].SetDirection(true);
                    if (m_iStartFolder >= m_atFolders.Length - 1)
                        m_iStartFolder = -1;
                    else
                        ++m_iStartFolder;
                }
            }
            else
            {
                if (Input.GetMouseButtonDown(0))
                {
                    Ray ray = CameraController.Instance.camera.ScreenPointToRay(Input.mousePosition);
                    Debug.DrawLine(ray.origin, ray.origin + (ray.direction * 20), Color.red, 2f);
                    RaycastHit hit;
                    if (Physics.Raycast(ray, out hit, 20, m_lOptionsLayer))
                    {
                        for (int i = 0; i < m_atFolders.Length; ++i)
                        {
                            if (m_atFolders[i] == hit.collider.GetComponent<TrunkFolder>())
                            {
                                m_iSelectedFolder = i;
                            }
                        }
                    }
                }

                for (int i = 0; i < m_atFolders.Length; ++i)
                {
                    if (i < m_iSelectedFolder)
                    {
                        m_atFolders[i].SetDirection(false);
                    }
                    else
                    {
                        m_atFolders[i].SetDirection(true);
                    }
                }
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

    public override void Click()
    {
        m_bSelected = true;
        Camera.main.GetComponent<MenuCam>().m_bPlaying = true;
        CameraController.Instance.m_tLookAt = m_tLookPoint;
        CameraController.Instance.GoToPoint(m_tMovePoint.position, float.PositiveInfinity, m_fMoveToTime);

        m_aController.SetBool("Opened", true);
        m_aController.SetFloat("Direction", 1.0f);
        m_fOpenTimer = 0f;
        m_iStartFolder = 0;
        m_iSelectedFolder = 0;
        m_IsActive = false;
        CurrentArea = this;
        MenuCam.instance.m_BackButton.SetActive(true);
    }

    public override void Exit()
    {
        base.Exit();
        CameraController.Instance.GoToPoint(CameraController.Instance.m_v3StartPos);
        CameraController.Instance.m_tLookAt = null;
        m_bSelected = false;

        m_aController.SetBool("Opened", false);
        m_aController.SetFloat("Direction", -1.0f);

        foreach (TrunkFolder move in m_atFolders)
        {
            move.SetDirection(false);
        }
        m_fOpenTimer = -1f;
        m_iStartFolder = -1;
        m_iSelectedFolder = 10;
    }
}
