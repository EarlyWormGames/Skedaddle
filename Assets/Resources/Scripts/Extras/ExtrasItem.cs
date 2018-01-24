using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ExtrasItem : MonoBehaviour
{
    public int m_requireNuts;

    internal Extras m_eParent;
    internal GUINavigation m_gNav;

    protected bool m_bSelected;
    protected bool m_bCanQuit = true;

    public virtual void Show()
    {
        m_eParent.Select(this);
        m_bSelected = true;
    }

    public virtual void Hide()
    {
        m_eParent.Select(null);
        m_bSelected = false;
    }

    void Update()
    {
        if (m_bSelected)
        {
            if (m_bCanQuit)
            {
                if (Keybinding.GetKeyDown("Pause") || Controller.GetButtonDown(ControllerButtons.B))
                {
                    m_gNav.Select();
                    Hide();
                }
            }
        }

        OnUpdate();
    }

    protected virtual void OnUpdate() { }
}
