using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeatmapSettings : MonoBehaviour
{
    public static HeatmapSettings instance;

    [Tooltip("The name specified in the Heatmap settings")]
    public string LevelName;    

    private void Awake()
    {
        instance = this;
    }
}