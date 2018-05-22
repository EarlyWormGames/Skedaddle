using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// add an event timer that will be called back once the timer has run out
/// Use:
/// cut scenes?
/// </summary>
public class EventOverTime : MonoBehaviour
{
    [Serializable]
    public class EventTimer
    {
        public float _time;
        public bool _usePercent = true;
        public AnimationCurve _curve = AnimationCurve.Linear(0, 0, 1, 1);
        public FloatEvent _event;

        public bool _showInspector = false;
    }

    public List<EventTimer> Events = new List<EventTimer>();
    [HideInInspector] public float timer;
    private int index = -1;

    private void Update()
    {
        if (index >= 0)
        {
            EventTimer current = Events[index];

            //Increase the time and clamp it
            timer = Mathf.Clamp(timer + Time.deltaTime, 0, current._time);

            //Calculate the time using the curve, with percentage if requested
            float t = timer / (current._usePercent ? current._time : 1);
            t = current._curve.Evaluate(t);

            //Invoke the event with the current time
            current._event.Invoke(t);

            if (timer >= current._time)
            {
                //Step to the next event
                ++index;
                //Disable the calls when finished
                if (index >= Events.Count)
                    index = -1;
                timer = 0;
            }
        }
    }

    public void Begin()
    {
        index = 0;
        timer = 0;
    }
}