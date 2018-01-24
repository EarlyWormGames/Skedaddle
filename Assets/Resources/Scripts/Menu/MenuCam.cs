using UnityEngine;
using System.Collections;

public class MenuCam : MonoBehaviour
{
    public static MenuCam instance;

    public LayerMask m_lHitLayers;
    public GameObject m_Slider;
    public GameObject m_BackButton;
    public GameObject m_leftArrow;
    public GameObject m_rightArrow;

    internal bool m_bPlaying;

    new private Camera camera;
    private float m_fTimer;

    void Start()
    {
        instance = this;

        camera = GetComponent<Camera>();
        Cursor.visible = true;
        m_BackButton.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.BackQuote))
        {
            if (m_Slider != null)
                m_Slider.SetActive(!m_Slider.activeInHierarchy);
        }

        if (!m_bPlaying)
        {
            m_leftArrow.SetActive(false);
            m_rightArrow.SetActive(false);
        }

        if (m_bPlaying || !Splash.GameLoaded || Controller.Connected)
            return;

        if (!EWEyeTracking.active)
        {
            //Do a raycast from the mouse cursor position
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;
            Debug.DrawLine(ray.origin, ray.direction * 100, Color.red);
            if (Physics.Raycast(ray, out hitInfo, 100, m_lHitLayers))
            {
                GUINavigation area = hitInfo.collider.GetComponent<GUINavigation>();
                if (area != GUINavigation.Selected && area != null)
                {
                    if (GUINavigation.Selected != null)
                    {
                        GUINavigation.Selected.Deselect();
                    }

                    GUINavigation.Selected = area;
                    GUINavigation.Selected.Select();
                }
            }

            if (GUINavigation.Selected != null)
            {
                Highlighter.Selected = GUINavigation.Selected.gameObject;
                if (Input.GetMouseButtonDown(0))
                {
                    GUINavigation.Selected.Click();
                }
            }
        }
        else
        {
            if (EWEyeTracking.GetFocusedObject() != null)
            {
                Highlighter.Selected = EWEyeTracking.GetFocusedObject().gameObject;

                //If we're looking at an object, check if it's a gui navigation object
                GUINavigation area = EWEyeTracking.GetFocusedObject().GetComponent<GUINavigation>();
                if (area != GUINavigation.Selected && area != null)
                {
                    if (GUINavigation.Selected != null)
                    {
                        GUINavigation.Selected.Deselect();
                    }

                    //Highlight that area
                    GUINavigation.Selected = area;
                    GUINavigation.Selected.Select();
                    m_fTimer = 0f;
                }
                else if (area == null)
                {
                    m_fTimer = 0f;
                }

                m_fTimer += Time.deltaTime;
                if (m_fTimer >= EWEyeTracking.Instance.m_fLookTime)
                {
                    //Select it if we look at it for long enough
                    GUINavigation.Selected.Click();
                }
            }
        }
    }

    public void PrevLevel()
    {
        if (Area.CurrentArea != null)
            Area.CurrentArea.PrevLevel();
    }

    public void NextLevel()
    {
        if (Area.CurrentArea != null)
            Area.CurrentArea.NextLevel();
    }

    public void ExitCurrentArea()
    {
        if (Area.CurrentArea != null)
            Area.CurrentArea.Exit();
    }
}
