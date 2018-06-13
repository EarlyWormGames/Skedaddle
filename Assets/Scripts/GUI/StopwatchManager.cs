using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// Manager to handle all the stopwatchs created from testing Stopwatch Triggers.
/// </summary>
public class StopwatchManager : MonoBehaviour {

    ///////////////////////////////////////
    ///         Public Variables        ///
    ///////////////////////////////////////

    /// <summary>
    /// Average Times Parent GUI Object
    /// </summary>
    [Tooltip("Average Times Parent GUI Object")]
    public GameObject AverageParent;
    /// <summary>
    /// Average Timer Prefab used to create GUI
    /// </summary>
    [Tooltip("Average Timer Prefab used to create GUI")]
    public GameObject AverageTimesPrefab;
    /// <summary>
    /// Contains a list of all Timers
    /// </summary>
    internal List<StopwatchTimer> Times;
    /// <summary>
    /// Contains a list of all Average Timers
    /// </summary>
    private List<StopwatchTimer> AverageTimers;

	// Use this for initialization
	void Start () {
		if(Times == null)
        {
            Times = new List<StopwatchTimer>();
        }
	}
	
    /// <summary>
    /// Clears all timers
    /// </summary>
	public void ClearTimes()
    {
        foreach(StopwatchTimer x in Times)
        {
            Destroy(x.gameObject);
        }
        Times.Clear();
    }

    /// <summary>
    /// creates average times for current animals being timed
    /// </summary>
    public void AverageTimes()
    {
        //clear Existing Averages if they exist
        if(AverageTimers == null)
        {
            AverageTimers = new List<StopwatchTimer>();
        }
        else
        {
            foreach(StopwatchTimer x in AverageTimers)
            {
                Destroy(x.gameObject);
            }
            AverageTimers.Clear();
        }
        //Iinitialise new Average variables
        List<StopwatchTimer> TimesToAverage = new List<StopwatchTimer>(Times);
        while (TimesToAverage.Count != 0)
        {
            List<StopwatchTimer> TimesToRemove = new List<StopwatchTimer>();    //creates list of times to remove from list to average
            string NameAverage = "";                                            //What is currently being averaged
            float sum = 0;                                                      //sum of times
            int amount = 0;                                                     //amount of times
            //scan for items of the same timer types and sum up totals
            for(int i = 0; i < TimesToAverage.Count; i++)
            {
                if(i == 0)
                {
                    //initialise current average loop
                    NameAverage = TimesToAverage[i].TimerName;
                    sum += TimesToAverage[i].TimerTime;
                    amount++;
                    TimesToRemove.Add(TimesToAverage[i]);
                }
                else
                {
                    if(TimesToAverage[i].TimerName == NameAverage)
                    {
                        //adds same timers to the current totals
                        sum += TimesToAverage[i].TimerTime;
                        amount++;
                        TimesToRemove.Add(TimesToAverage[i]);
                    }
                }
            }
            //remove times from times to average
            foreach (StopwatchTimer x in TimesToRemove)
            {
                TimesToAverage.Remove(x);
            }
            TimesToRemove.Clear();
            //average total
            StopwatchTimer Average = Instantiate(AverageTimesPrefab, AverageParent.transform).GetComponent<StopwatchTimer>();
            Average.TimerName = NameAverage;
            Average.TimerTime = sum / amount;
            AverageTimers.Add(Average);
            Average = null;
        }

    }
}
