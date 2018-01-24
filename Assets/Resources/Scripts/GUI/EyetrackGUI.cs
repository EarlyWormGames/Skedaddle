using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class EyetrackGUI : MonoBehaviour
{
    public EWEyeTracking.HoldLength m_HoldLength = EWEyeTracking.HoldLength.LONG;
    public UnityEvent m_Event;

    private EWGazeObject m_GazeObject;
    private Button m_Button;
    private float m_fGazeTimer;

    // Use this for initialization
    void Start()
    {
        m_GazeObject = GetComponent<EWGazeObject>();
        m_Button = GetComponent<Button>();
    }

    // Update is called once per frame
    void Update()
    {
        if (EWEyeTracking.GetFocusedUI() == m_GazeObject && m_GazeObject != null && m_Button.interactable)
        {
            m_fGazeTimer += Time.unscaledDeltaTime;

            if (m_fGazeTimer >= (m_HoldLength == EWEyeTracking.HoldLength.LONG ? EWEyeTracking.holdTime : EWEyeTracking.shortHoldTime))
            {
                m_fGazeTimer = 0f;
                m_Event.Invoke();
            }
        }
        else
        {
            m_fGazeTimer = 0f;
        }
    }
}
