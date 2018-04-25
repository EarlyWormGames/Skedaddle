using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StopwatchManager : MonoBehaviour {

    public GameObject AverageParent;
    public GameObject AverageTimesPrefab;
    internal List<StopwatchTimer> Times;
    private List<StopwatchTimer> AverageTimers;

	// Use this for initialization
	void Start () {
		if(Times == null)
        {
            Times = new List<StopwatchTimer>();
        }
	}
	
	public void ClearTimes()
    {
        foreach(StopwatchTimer x in Times)
        {
            Destroy(x.gameObject);
        }
        Times.Clear();
    }

    public void AverageTimes()
    {
        //clear Existing Averages
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
            List<StopwatchTimer> TimesToRemove = new List<StopwatchTimer>();
            string NameAverage = "";
            float sum = 0;
            int amount = 0;
            for(int i = 0; i < TimesToAverage.Count; i++)
            {
                if(i == 0)
                {
                    NameAverage = TimesToAverage[i].TimerName;
                    sum += TimesToAverage[i].TimerTime;
                    amount++;
                    TimesToRemove.Add(TimesToAverage[i]);
                }
                else
                {
                    if(TimesToAverage[i].TimerName == NameAverage)
                    {
                        sum += TimesToAverage[i].TimerTime;
                        amount++;
                        TimesToRemove.Add(TimesToAverage[i]);
                    }
                }
            }
            foreach (StopwatchTimer x in TimesToRemove)
            {
                TimesToAverage.Remove(x);
            }
            TimesToRemove.Clear();
            StopwatchTimer Average = Instantiate(AverageTimesPrefab, AverageParent.transform).GetComponent<StopwatchTimer>();
            Average.TimerName = NameAverage;
            Average.TimerTime = sum / amount;
            AverageTimers.Add(Average);
            Average = null;
        }

    }
}
