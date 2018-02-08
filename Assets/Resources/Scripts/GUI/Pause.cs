using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class Pause : MonoBehaviour
{

    public string[] m_sDontShowPause;

    public Canvas m_cPauseCanvas;
    public GameObject m_gPopUp;
    public Button m_gPauseButton;
    //public GUINavigation m_gFirstSelection;

    private bool m_bCanPause = true;
    private bool m_bIsPaused = false;
    // Use this for initialization
    void Start()
    {
        m_gPopUp.SetActive(false);
        SceneManager.sceneLoaded += SceneLoaded;
        SceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
    }

    void SceneLoaded(Scene a_scene, LoadSceneMode a_mode)
    {
        m_bCanPause = true;
        for (int i = 0; i < m_sDontShowPause.Length; i++)
        {
            if (m_sDontShowPause[i] == SceneManager.GetActiveScene().name)
            {
                m_cPauseCanvas.enabled = false;
                m_gPopUp.SetActive(false);
                m_bCanPause = false;
                break;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        //if ((Keybinding.GetKeyDown("Pause") || Controller.GetButtonDown(ControllerButtons.Start)) && m_bCanPause)
        //{
        //    if (!m_bIsPaused)
        //    {
        //        OpenPause();
        //    }
        //    else
        //    {
        //        ClosePause();
        //    }
        //}
    }

    public void OpenPause()
    {
        Time.timeScale = 0;
        m_gPauseButton.interactable = false;
        m_gPopUp.SetActive(true);
        m_bIsPaused = true;

        //m_gFirstSelection.enabled = true;
        //m_gFirstSelection.Select();
    }

    public void ClosePause()
    {
        Time.timeScale = 1;
        m_gPopUp.SetActive(false);
        m_bIsPaused = false;
        m_gPauseButton.interactable = true;
    }

    public void ReturnToMenu()
    {
        EWApplication.LoadLevel("Menu");
    }

    public void Quit()
    {
        EWApplication.Quit();
    }
}
