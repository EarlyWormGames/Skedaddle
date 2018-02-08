using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.EventSystems;

[System.Serializable]
public enum TabName
{
    MAIN,
    SOUND,
    ACCESS,
    GRAPHICS,
    LEVELS
}

public class InGameMenu : MonoBehaviour
{
    

    [System.Serializable]
    public class Menutab
    {
        public CanvasGroup m_Group;
        public TabName m_Name;

        internal bool m_bSelected;
        internal float m_fGazeTimer;
    }

    public Animator m_Animator;
    public float m_TabFadeSpeed;
    public CanvasGroup m_Background;
    public CanvasGroup m_PauseButton;
    public CustomSlider m_ContrastSlider;
    public Menutab[] m_Tabs;

    private bool m_bPaused;
    private float m_fAnimationTime = 0f;

    private Menutab m_SelectedTab;
    private bool m_bShow = true;

    // Use this for initialization
    void Start()
    {
#if !UNITY_EDITOR
        Cursor.visible = false;
#endif

        m_Background.alpha = 0f;

        foreach (Menutab tab in m_Tabs)
        {
            tab.m_Group.alpha = 0f;
        }

        m_PauseButton.alpha = 1f;

        m_ContrastSlider.value = FadeOutEffect.Amount;
        m_ContrastSlider.onPointerDown += HideBG;
        m_ContrastSlider.onPointerUp += ShowBG;
    }

    // Update is called once per frame
    void Update()
    {
        m_PauseButton.gameObject.SetActive(EWEyeTracking.active);

        if (m_bPaused)
        {
            if (m_bShow)
                m_Background.alpha = Mathf.Min(m_Background.alpha + Time.unscaledDeltaTime * m_TabFadeSpeed, 1f);
            else
                m_Background.alpha = Mathf.Max(m_Background.alpha - Time.unscaledDeltaTime * m_TabFadeSpeed, 0f);

            m_PauseButton.alpha = Mathf.Max(m_PauseButton.alpha - Time.unscaledDeltaTime * m_TabFadeSpeed, 0f);
        }
        else
        {
            m_Background.alpha = Mathf.Max(m_Background.alpha - Time.unscaledDeltaTime * m_TabFadeSpeed, 0f);
            m_PauseButton.alpha = Mathf.Min(m_PauseButton.alpha + Time.unscaledDeltaTime * m_TabFadeSpeed, 1f);
        }

        foreach (Menutab tab in m_Tabs)
        {
            if (tab.m_bSelected)
            {
                if (tab.m_Group.alpha < 1f)
                    tab.m_Group.alpha = Mathf.Min(tab.m_Group.alpha + Time.unscaledDeltaTime * m_TabFadeSpeed, 1f);
                tab.m_Group.blocksRaycasts = true;
            }
            if (!tab.m_bSelected)
            {
                if (tab.m_Group.alpha > 0f)
                    tab.m_Group.alpha = Mathf.Max(tab.m_Group.alpha - Time.unscaledDeltaTime * m_TabFadeSpeed, 0f);
                tab.m_Group.blocksRaycasts = false;
            }
        }

        //if ((Keybinding.GetKeyDown("Pause") || Controller.GetButtonDown(ControllerButtons.Start)))
        //{
        //    if (!m_bPaused)
        //    {
        //        Pause();
        //    }
        //    else if (m_SelectedTab.m_Name == TabName.MAIN)
        //    {
        //        Return();
        //    }
        //    else
        //    {
        //        SwitchTab(TabName.MAIN);
        //    }
        //}
    }

    public void Menu()
    {
        EWApplication.LoadLevel("1-0");
        Time.timeScale = 1f;
        if (m_Animator != null)
            m_Animator.SetTrigger("Close");
#if !UNITY_EDITOR
        //Cursor.visible = true;
#endif
    }

    public void Reload()
    {
        EWApplication.ReloadLevel();
        Time.timeScale = 1f;
        if (m_Animator != null)
            m_Animator.SetTrigger("Close");
#if !UNITY_EDITOR
        Cursor.visible = false;
#endif
    }

    public void Pause()
    {
#if !UNITY_EDITOR
                Cursor.visible = true;
#endif
        m_bPaused = true;
        if (m_Animator != null)
            m_Animator.SetTrigger("Open");
        m_SelectedTab = GetTab(TabName.MAIN);
        m_SelectedTab.m_bSelected = true;
        Time.timeScale = 0f;
    }

    public void Return()
    {
#if !UNITY_EDITOR
                Cursor.visible = false;
#endif

        m_bPaused = false;
        if (m_Animator != null)
            m_Animator.SetTrigger("Close");

        m_SelectedTab.m_bSelected = false;
        m_SelectedTab = null;
        Time.timeScale = 1f;
    }

    public void SwitchTab(TabName a_tab)
    {
        m_SelectedTab.m_bSelected = false;
        m_SelectedTab = GetTab(a_tab);
        m_SelectedTab.m_bSelected = true;
    }

    public void SwitchTab(CanvasGroup a_tab)
    {
        m_SelectedTab.m_bSelected = false;
        m_SelectedTab = GetTab(a_tab);
        m_SelectedTab.m_bSelected = true;
    }

    public Menutab GetTab(CanvasGroup a_tab)
    {
        foreach (Menutab tab in m_Tabs)
        {
            if (tab.m_Group == a_tab)
            {
                return tab;
            }
        }
        return null;
    }

    public Menutab GetTab(TabName a_tab)
    {
        foreach (Menutab tab in m_Tabs)
        {
            if (tab.m_Name == a_tab)
            {
                return tab;
            }
        }
        return null;
    }

    public float FadeOut
    {
        get
        {
            return FadeOutEffect.Amount;
        }
        set
        {
            FadeOutEffect.Amount = value;
        }
    }

    public void AddFadeOut(float a_amount)
    {
        FadeOutEffect.Amount = Mathf.Clamp01(FadeOutEffect.Amount + a_amount);
        m_ContrastSlider.value = FadeOutEffect.Amount;
    }

    public void LoadLevel(string a_name)
    {
        EWApplication.LoadLevel(a_name);
    }

    public void HideBG(PointerEventData a_data)
    {
        m_bShow = false;
    }

    public void ShowBG(PointerEventData a_data)
    {
        m_bShow = true;
    }
}
