using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Stopwatch : AnimalTrigger
{

    //Public Variables

    public StopwatchManager GUIObject;
    public GameObject TimerPrefab;

    //Private Static Variables

    private static StopwatchTimer TimerObject;
    private static Animal TimedAnimal;
    private static float Timer;
    private static int TotalTimers;
    private static bool Running;
    private static bool Climbing;

    void Awake()
    {
        TotalTimers++;
    }

    void Update()
    {
        if (TimerObject != null)
        {
            if (TimedAnimal.m_eName == ANIMAL_NAME.POODLE)
            {
                Running = TimedAnimal.GetComponent<Poodle>().m_bRunning;
            }
            else
            {
                Running = false;
            }
            if (TimedAnimal.m_eName == ANIMAL_NAME.LORIS)
            {
                Climbing = TimedAnimal.GetComponent<Loris>().m_bClimbing;
            }
            else
            {
                Climbing = false;
            }
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
        if (TimedAnimal == null)
        {
            TimedAnimal = a_animal;
            TimerObject = Instantiate(TimerPrefab, GUIObject.transform).GetComponent<StopwatchTimer>();
            TimerObject.Manager = GUIObject;
            GUIObject.Times.Add(TimerObject);
            Timer = 0;
        }
        else
        {
            if (a_animal == TimedAnimal)
            {
                TimedAnimal = null;
                TimerObject = null;
            }
        }
    }

    public override void AnimalExit(Animal animal)
    {
        
    }
}
