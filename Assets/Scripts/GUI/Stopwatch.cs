using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// Trigger script for stopwatch
/// </summary>
public class Stopwatch : AnimalTrigger
{
    /// <summary>
    /// Checks whether to use only the head trigger for actiavtion
    /// </summary>
    protected override bool HeadTriggerOnly { get; set; }

    //Public Variables

    /// <summary>
    /// Lap Time Parent Object
    /// </summary>
    [Tooltip("Lap Time Parent Object")]
    public StopwatchManager GUIObject;
    /// <summary>
    /// Lap Time prefab used to create GUI Timers
    /// </summary>
    [Tooltip("Lap Time prefab used to create GUI Timers")]
    public GameObject TimerPrefab;

    //Private Static Variables

    /// <summary>
    /// Parent Timer object
    /// </summary>
    private static StopwatchTimer TimerObject;
    /// <summary>
    /// Animal being timed
    /// </summary>
    private static Animal TimedAnimal;
    /// <summary>
    /// Current Time
    /// </summary>
    private static float Timer;
    /// <summary>
    /// Total amount of Timers
    /// </summary>
    private static int TotalTimers;
    /// <summary>
    /// is the Animal running?
    /// </summary>
    private static bool Running;
    /// <summary>
    /// is the Animal Climbing?
    /// </summary>
    private static bool Climbing;

    void Awake()
    {
        TotalTimers++;
    }

    void Update()
    {
        //check if currently timed
        if (TimerObject != null)
        {
            // check if animal is running
            if (TimedAnimal.m_eName == ANIMAL_NAME.POODLE)
            {
                Running = TimedAnimal.GetComponent<Poodle>().m_bRunning;
            }
            else
            {
                Running = false;
            }
            // check if animal is climbing
            if (TimedAnimal.m_eName == ANIMAL_NAME.LORIS)
            {
                Climbing = TimedAnimal.GetComponent<Loris>().m_bClimbing;
            }
            else
            {
                Climbing = false;
            }
            //Update timer
            Timer += Time.deltaTime / TotalTimers;
            TimerObject.TimerName = TimedAnimal.name + (Running ? " Run" : "") + (Climbing ? " Climb" : "");
            TimerObject.TimerTime = Timer;
        }
        else
        {
            if (TimedAnimal != null) TimedAnimal = null;
        }
    }

    public override void AnimalEnter(Animal a_animal)
    {
        //check if timer is currently on
        if (TimedAnimal == null)
        {
            //start Timer
            TimedAnimal = a_animal;
            TimerObject = Instantiate(TimerPrefab, GUIObject.transform).GetComponent<StopwatchTimer>();
            TimerObject.Manager = GUIObject;
            GUIObject.Times.Add(TimerObject);
            Timer = 0;
        }
        else
        {
            //stop timer
            if (a_animal == TimedAnimal)
            {
                TimedAnimal = null;
                TimerObject = null;
            }
        }
    }

    //phantom function
    public override void AnimalExit(Animal animal)
    {
        
    }
}
