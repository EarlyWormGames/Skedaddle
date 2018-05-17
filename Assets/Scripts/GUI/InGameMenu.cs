using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
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

    public UnityEvent OnPause;

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

        if (GameManager.mainMap.pause.wasJustPressed)
        {
            if (!m_bPaused)
            {
                Pause();
            }
            else if (m_SelectedTab.m_Name == TabName.MAIN)
            {
                Return();
            }
            else
            {
                SwitchTab(TabName.MAIN);
            }
        }
    }

    public void Menu()
    {
        EWApplication.LoadLevel("2-1");
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

    /// <summary>
    /// Pause the game.
    /// </summary>
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

    /// <summary>
    /// UnPause the game
    /// </summary>
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

    /// <summary>
    /// select the current tab in the pause menu
    /// </summary>
    /// <param name="a_tab"></param>
    public void SwitchTab(TabName a_tab)
    {
        m_SelectedTab.m_bSelected = false;
        m_SelectedTab = GetTab(a_tab);
        m_SelectedTab.m_bSelected = true;
    }
    /// <summary>
    /// select the current tab in the pause menu
    /// </summary>
    /// <param name="a_tab"></param>
    public void SwitchTab(CanvasGroup a_tab)
    {
        m_SelectedTab.m_bSelected = false;
        m_SelectedTab = GetTab(a_tab);
        m_SelectedTab.m_bSelected = true;
    }

    /// <summary>
    /// grab a tab from the menu.
    /// </summary>
    /// <param name="a_tab"></param>
    /// <returns></returns>
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

    /// <summary>
    /// grab a tab from the menu.
    /// </summary>
    /// <param name="a_tab"></param>
    /// <returns></returns>
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

    /// <summary>
    /// load a scene
    /// </summary>
    /// <param name="a_name"></param>
    public void LoadLevel(string a_name)
    {
        EWApplication.LoadLevel(a_name);
    }

    /// <summary>
    /// hide the back ground
    /// </summary>
    /// <param name="a_data"></param>
    public void HideBG(PointerEventData a_data)
    {
        m_bShow = false;
    }

    /// <summary>
    /// show the background
    /// </summary>
    /// <param name="a_data"></param>
    public void ShowBG(PointerEventData a_data)
    {
        m_bShow = true;
    }
}
