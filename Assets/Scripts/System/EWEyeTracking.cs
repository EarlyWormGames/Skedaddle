using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;


public class EWEyeTracking : Singleton<EWEyeTracking>
{
    [System.Serializable]
    public enum HoldLength
    {
        LONG,
        SHORT
    }

    public static Vector2 position
    {
        get
        {
            return m_v2Position;
        }
    }

    public static Vector3 worldPosition
    {
        get
        {
            return m_v3WorldPos;
        }
    }

    public static bool active
    {
        get
        {
            return Tobii.EyeTracking.EyeTracking.GetGazeTrackingStatus().IsTrackingEyeGaze;
        }
    }

    public static float holdTime
    {
        get
        {
            return Instance.m_fLookTime;
        }
    }

    public static float shortHoldTime
    {
        get
        {
            return Instance.m_fShortLookTime;
        }
    }

    public static float lerpSpeed
    {
        get
        {
            return Instance.m_fLerpSpeed;
        }
    }

    public float m_fLookTime = 1.5f;
    public float m_fShortLookTime = 0.5f;
    public float m_fLerpSpeed = 0.5f;

    public static float FocusZ = 10;

    private static Vector2 m_v2Position;
    private static Vector3 m_v3WorldPos;

    private static EWGazeObject FocusedObject;
    private static EWGazeObject FocusedUI;

    // Use this for initialization
    protected override void OnAwake()
    {
        DontDestroy();
    }

    // Update is called once per frame
    void Update()
    {
        if (!active)
            return;        

        Vector2 display = Tobii.EyeTracking.EyeTracking.GetGazePoint().Screen;
        m_v2Position = Vector3.Lerp(m_v2Position, new Vector2(display.x, display.y + (Screen.height * 0.0f)), m_fLerpSpeed);

        if (float.IsNaN(m_v2Position.x))
            m_v2Position.x = 0;
        if (float.IsNaN(m_v2Position.y))
            m_v2Position.y = 0;

        FocusedObject = null;
        FocusedUI = null;

        m_v3WorldPos = CameraController.Instance.camera.ScreenToWorldPoint(new Vector3(m_v2Position.x, m_v2Position.y, CameraController.Instance.transform.position.z));
    }

    /// <summary>
    /// Raycasts for an object
    /// </summary>
    /// <param name="a_AllowFamilySearch">Allow looking for the Gaze Aware script on parents and children?</param>
    /// <returns></returns>
    public static EWGazeObject GetFocusedObject()
    {
        if (!active)
            return null;

        if (FocusedObject != null)
            return FocusedObject;

        //Grab the Tobii settings
        var sets = Tobii.EyeTracking.GazeFocusSettings.Get();
        Ray ray = Camera.main.ScreenPointToRay(position);
        RaycastHit hit;

        //Cast a ray out from the gaze position, using the Tobii settings
        if (Physics.Raycast(ray, out hit, sets.MaximumDistance, sets.LayerMask))
        {
            EWGazeObject gaze = hit.collider.GetComponent<EWGazeObject>();
            //Then check if the hit object has a gazeaware component
            if (gaze != null)
            {
                FocusedObject = gaze;
                return gaze;
            }
            else
            {
                //Search family for this component
                var parent = hit.collider.GetComponentInParent<EWGazeObject>();
                if (parent != null)
                {
                    FocusedObject = parent;
                    return parent;
                }

                var child = hit.collider.GetComponentInChildren<EWGazeObject>();
                if (child != null)
                {
                    FocusedObject = child;
                    return child;
                }
            }
        }

        return null;
    }

    public static EWGazeObject GetFocusedUI()
    {
        if (!active)
            return null;

        if (FocusedUI != null)
            return FocusedUI;

        //Code to be place in a MonoBehaviour with a GraphicRaycaster component
        GraphicRaycaster[] casters = FindObjectsOfType<GraphicRaycaster>();
        //Create the PointerEventData with null for the EventSystem
        PointerEventData ped = new PointerEventData(null);
        //Set required parameters, in this case, mouse position
        ped.position = m_v2Position;
        //Create list to receive all results
        List<RaycastResult> results = new List<RaycastResult>();

        foreach (var gr in casters)
        {
            //Raycast it
            gr.Raycast(ped, results);
            foreach (var result in results)
            {
                EWGazeObject gaze = result.gameObject.GetComponent<EWGazeObject>();
                //Return first object found with gaze aware script
                if (gaze != null)
                {
                    FocusedUI = gaze;
                    return gaze;
                }
            }
        }

        return null;
    }

    void OnDrawGizmos()
    {
        if (!Application.isPlaying)
            return;

        if (active)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawCube(m_v3WorldPos, new Vector3(0.5f, 0.5f, 0.5f));
        }
    }
}
