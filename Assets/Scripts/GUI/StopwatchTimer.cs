using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// Stopwatch timer 
/// </summary>
public class StopwatchTimer : MonoBehaviour {

    internal StopwatchManager Manager;
    internal string TimerName;
    internal float TimerTime;
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
