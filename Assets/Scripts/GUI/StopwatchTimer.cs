using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// Stopwatch timer GUI
/// </summary>
public class StopwatchTimer : MonoBehaviour {

    /// <summary>
    /// Internal Stopwatch manager container
    /// </summary>
    internal StopwatchManager Manager;
    /// <summary>
    /// Name of this stopwatch
    /// </summary>
    internal string TimerName;
    /// <summary>
    /// This timers current time
    /// </summary>
    internal float TimerTime;
    /// <summary>
    /// Text Display on GUI
    /// </summary>
    private TextMeshProUGUI TextField;

    void Start()
    {
        TextField = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        //display the timer length for the current level
        TextField.text = TimerName + " : " + TimerTime.ToString("0.00");
    }

    /// <summary>
    /// Destroy the timer
    /// </summary>
    public void DeleteTime()
    {
        Manager.Times.Remove(this);
        Destroy(gameObject);
    }

}
