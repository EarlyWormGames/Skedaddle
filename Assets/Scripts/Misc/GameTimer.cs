using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.IO;

/// <summary>
/// counting the seconds/time the user has been playing from for a:
/// -Speedrun 
///     (the begging of the application.)
/// -Single level
///     (the begging of a level.)
/// </summary>
public class GameTimer : Singleton<GameTimer>
{
    public GameObject m_EndPanel;
    public Text m_TimerText;
    public Text m_EndText;
    public InputField m_Namefield;

    public bool m_DoSpeedRun = true;


    private float m_Timer = 0f;
    private bool m_Running = false;

    // Use this for initialization
    void Start()
    {
        DontDestroy();
        SceneManager.sceneLoaded += SceneLoad;

        if (!m_DoSpeedRun)
            m_TimerText.gameObject.SetActive(false);
    }

    /// <summary>
    /// Unity Action that is called when the scene is loaded.
    /// </summary>
    /// <param name="a_scene"></param>
    /// 
    /// <param name="a_loadMode"></param>
    private void SceneLoad(Scene a_scene, LoadSceneMode a_loadMode)
    {
        if (a_scene.name == "SpeedrunEnd")
        {
            StopTimer();
            m_EndPanel.SetActive(true);
            m_TimerText.gameObject.SetActive(false);

            //Show the cursor
            Cursor.visible = true;

            System.TimeSpan t = System.TimeSpan.FromSeconds(m_Timer);
            m_EndText.text = string.Format("{0:00} minutes {1:00}.{2:00} seconds", t.Minutes, t.Seconds, t.Milliseconds);

            if (!m_DoSpeedRun)
            {
                File.AppendAllText("Time.txt", "Anon\n" + m_Timer.ToString() + "\n");
                SceneManager.LoadScene("1-0");
                ResetTimer();

                if (m_DoSpeedRun)
                    m_TimerText.gameObject.SetActive(true);
                m_EndPanel.SetActive(false);
            }
        }
        else if (a_scene.name == "Menu" || a_scene.name == "1-0")
        {
            StopTimer();
            ResetTimer();
            m_EndPanel.SetActive(false);
            if (m_DoSpeedRun)
                m_TimerText.gameObject.SetActive(true);
        }
        else
        {
            m_EndPanel.SetActive(false);
            if (m_DoSpeedRun)
                m_TimerText.gameObject.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (m_Running)
            m_Timer += Time.deltaTime;

        System.TimeSpan t = System.TimeSpan.FromSeconds(m_Timer);
        m_TimerText.text = string.Format("{0:00}:{1:00}.{2:00}", t.Minutes, t.Seconds, t.Milliseconds);

        if (Input.GetKeyDown(KeyCode.P))
        {
            m_DoSpeedRun = !m_DoSpeedRun;
            m_TimerText.gameObject.SetActive(m_DoSpeedRun);
        }
    }

    /// <summary>
    /// send the score to the save manager
    /// </summary>
    public void SubmitScore()
    {
        if (m_Namefield.text.Length > 0)
        {
            File.AppendAllText("Time.txt", m_Namefield.text + "\n" + m_Timer.ToString() + "\n");
            SceneManager.LoadScene("1-0");
            ResetTimer();

            if (m_DoSpeedRun)
                m_TimerText.gameObject.SetActive(true);
            m_EndPanel.SetActive(false);
        }
    }


    public static void StartTimer()
    {
        Instance.m_Running = true;
    }

    public static void StopTimer()
    {
        Instance.m_Running = false;
    }

    public static void ResetTimer()
    {
        Instance.m_Timer = 0f;
    }
}