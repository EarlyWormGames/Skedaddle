using UnityEngine;
using System.Collections;

public class GUINavigation : MonoBehaviour
{
    public GUINavigation[] m_gUpDir;
    public GUINavigation[] m_gDownDir;
    public GUINavigation[] m_gLeftDir;
    public GUINavigation[] m_gRightDir;

    public static GUINavigation Selected;

    protected bool m_IsActive = false;

    // Use this for initialization
    void Awake()
    {
        enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_IsActive)
        {
            //if (Keybinding.GetKeyDown("MoveUp") || Controller.GetDpadDown(ControllerDpad.Up) || Controller.GetStickPositionDown(true, ControllerDpad.Up))
            //{
            //    for (int i = 0; i < m_gUpDir.Length; ++i)
            //    {
            //        if (m_gUpDir[i].gameObject.activeInHierarchy)
            //        {
            //            Deselect();
            //            m_gUpDir[i].Select();
            //            return;
            //        }
            //    }
            //}
            //if (Keybinding.GetKeyDown("MoveDown") || Controller.GetDpadDown(ControllerDpad.Down) || Controller.GetStickPositionDown(true, ControllerDpad.Down))
            //{
            //    for (int i = 0; i < m_gDownDir.Length; ++i)
            //    {
            //        if (m_gDownDir[i].gameObject.activeInHierarchy)
            //        {
            //            Deselect();
            //            m_gDownDir[i].Select();
            //            return;
            //        }
            //    }
            //}
            //if (Keybinding.GetKeyDown("MoveLeft") || Controller.GetDpadDown(ControllerDpad.Left) || Controller.GetStickPositionDown(true, ControllerDpad.Left))
            //{
            //    for (int i = 0; i < m_gLeftDir.Length; ++i)
            //    {
            //        if (m_gLeftDir[i].gameObject.activeInHierarchy)
            //        {
            //            Deselect();
            //            m_gLeftDir[i].Select();
            //            return;
            //        }
            //    }
            //}
            //if (Keybinding.GetKeyDown("MoveRight") || Controller.GetDpadDown(ControllerDpad.Right) || Controller.GetStickPositionDown(true, ControllerDpad.Right))
            //{
            //    for (int i = 0; i < m_gRightDir.Length; ++i)
            //    {
            //        if (m_gRightDir[i].gameObject.activeInHierarchy)
            //        {
            //            Deselect();
            //            m_gRightDir[i].Select();
            //            return;
            //        }
            //    }
            //}
            //
            //if (Keybinding.GetKeyDown("Action") || Controller.GetButtonDown(ControllerButtons.A))
            //{
            //    Click();
            //}
        }

        OnUpdate();
    }

    protected virtual void OnUpdate() { }

    public virtual void Deselect()
    {
        enabled = false;
        m_IsActive = false;
    }

    public virtual void Select()
    {
        enabled = true;
        m_IsActive = true;
        Selected = this;
    }

    public virtual void Click()
    {
        enabled = false;
        m_IsActive = false;
    }
}
