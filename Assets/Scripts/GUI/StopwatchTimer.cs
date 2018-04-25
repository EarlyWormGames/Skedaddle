using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

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
        TextField.text = TimerName + " : " + TimerTime.ToString("0.00");
    }

    public void DeleteTime()
    {
        Manager.Times.Remove(this);
        Destroy(gameObject);
    }

}
