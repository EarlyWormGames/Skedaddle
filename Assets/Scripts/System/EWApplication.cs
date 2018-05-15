using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;

public class EWApplication : Singleton<EWApplication>
{
    public delegate void OnLevelCallback(string a_levelName);

    //============================
    //       Public Vars
    //============================
    public string       m_sLoadLevelName;
    public float        m_fFadeSpeed = 1f;

    public OnLevelCallback m_olCallbacks;
    public bool         m_bRefresh;

    //============================
    //       Private Vars
    //============================
    private int         m_iWaitFrames;
    private bool        m_bQuit = false;
    private bool        m_bLoadCalled = false;

    private float       m_fTimer = 0f;

    // Use this for initialization
    void Start ()
    {
        DontDestroy();
        SceneManager.sceneLoaded += SceneLoaded;

        m_sLoadLevelName = SceneManager.GetActiveScene().name;
        TransitionEffect.instance.value = 1;
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (SceneManager.GetActiveScene().name != m_sLoadLevelName || m_bRefresh || m_bQuit)
        {
            if (TransitionEffect.instance.value >= 1f)
            {
                if (m_bQuit)
                {
                    Application.Quit();
                    return;
                }
                else if (!m_bLoadCalled)
                {
                    m_bLoadCalled = true;
                    //Load the level when it is fully visible
                    SceneManager.LoadSceneAsync(m_sLoadLevelName, LoadSceneMode.Single);
                }
            }
            else
            {
                m_fTimer += Time.deltaTime;
                //Make the game less visible
                TransitionEffect.instance.value = (m_fTimer / m_fFadeSpeed);
            }
        }
        else
        {
            if (TransitionEffect.instance.value <= 0f)
            {
                //Disable this script since we don't need to do anymore work
                enabled = false;
            }
            else if (m_iWaitFrames <= 0)
            {
                m_fTimer += Time.deltaTime;
                //Make the game more visible
                TransitionEffect.instance.value = 1 - (m_fTimer / m_fFadeSpeed);
            }
            else
            {
                --m_iWaitFrames;
            }
        }
	}

    void SceneLoaded(Scene a_scene, LoadSceneMode a_mode)
    {
        m_bLoadCalled = false;
        m_bRefresh = false;
        m_fTimer = 0f;

        TransitionEffect.instance.value = 1;
    }

    public static void LoadLevel(string a_levelName)
    {
        Instance.m_sLoadLevelName = a_levelName;
        Instance.enabled = true;
        Instance.m_iWaitFrames = 10;
        Instance.m_fTimer = 0f;

        Time.timeScale = 1;

        if (Instance.m_olCallbacks != null)
            Instance.m_olCallbacks(a_levelName);
    }

    public static void ReloadLevel()
    {
        Instance.m_sLoadLevelName = SceneManager.GetActiveScene().name;
        Instance.enabled = true;
        Instance.m_iWaitFrames = 10;
        Instance.m_bRefresh = true;
        Instance.m_fTimer = 0f;

        if (Instance.m_olCallbacks != null)
            Instance.m_olCallbacks(SceneManager.GetActiveScene().name);
    }

    public static void Quit()
    {
        Instance.m_bQuit = true;

        Instance.m_sLoadLevelName = SceneManager.GetActiveScene().name;
        Instance.enabled = true;
        Instance.m_iWaitFrames = 10;
        Instance.m_bRefresh = true;

        Instance.m_fTimer = 0f;
        Instance.m_olCallbacks("");
    }
}
