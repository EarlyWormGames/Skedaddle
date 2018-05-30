using UnityEngine;
using System.Collections;

/// <summary>
/// Class that holds information on the level and setting to load with
/// </summary>
public class Level : MonoBehaviour
{
    public string m_sLevelName;
    public Transform m_tLookPoint;
    public Level m_lPrevLevel;
    public Level m_lNextLevel;
    public Transform m_tMovePoint;
    public LevelPreview m_LevelScreen;
    
    internal Area m_aParent;

    private float m_fLeftLookTimer = -1f;
    private float m_fRightLookTimer = -1f;
    private float m_fStartTimer = 0;

    private bool m_bLoading;
    private EWGazeObject m_GazeObject;
    private float m_fEyeTimer = 0f;

    // Use this for initialization
    void Start()
    {
        enabled = false;
        m_GazeObject = GetComponent<EWGazeObject>();
    }

    // Update is called once per frame
    void Update()
    {
        if (m_bLoading)
            return;

        if (m_LevelScreen != null)
            m_LevelScreen.m_Forward = true;

        if (EWEyeTracking.GetFocusedObject() == m_GazeObject)
        {
            Highlighter.Selected = gameObject;
            m_fEyeTimer += Time.deltaTime;
        }

        //if (Keybinding.GetKeyDown("Action") || Controller.GetButtonDown(ControllerButtons.A) || /*Input.GetMouseButtonDown(0) ||*/ m_fEyeTimer >= EWEyeTracking.holdTime)
        //{
        //    EWApplication.LoadLevel(m_sLevelName);
        //    m_bLoading = true;
        //}

        Rect left = new Rect(0, 0, Screen.width / 4f, Screen.height);
        Rect right = new Rect(Screen.width - (Screen.width / 4f), 0, Screen.width / 4f, Screen.height);

        if (m_lPrevLevel != null)
        {
            MenuCam.instance.m_leftArrow.SetActive(true);

            //if (Keybinding.GetKeyDown("MoveLeft") || Controller.GetDpadDown(ControllerDpad.Left) || Controller.GetStickPositionDown(true, ControllerDpad.Left))
            //{
            //    m_aParent.SelectLevel(m_lPrevLevel);
            //}
        }
        else
            MenuCam.instance.m_leftArrow.SetActive(false);

        if (m_lNextLevel != null)
        {
            MenuCam.instance.m_rightArrow.SetActive(true);

            //if (Keybinding.GetKeyDown("MoveRight") || Controller.GetDpadDown(ControllerDpad.Right) || Controller.GetStickPositionDown(true, ControllerDpad.Right))
            //{
            //    m_aParent.SelectLevel(m_lNextLevel);
            //}
        }
        else
            MenuCam.instance.m_rightArrow.SetActive(false);
    }
}
