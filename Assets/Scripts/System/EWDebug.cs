using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class EWDebug : Singleton<EWDebug>
{
    public bool LogDebugMessages = false;

    public static void Log(object a_msg)
    {
        if (Instance.LogDebugMessages && Debug.isDebugBuild)
        {
            Debug.Log(a_msg);
        }
    }

    public static void LogError(object a_msg)
    {
        if (Instance.LogDebugMessages && Debug.isDebugBuild)
        {
            Debug.LogError(a_msg);
        }
    }

    public override void Reset()
    {
        
    }

    void Start()
    {
        DontDestroy();
    }

    void Update()
    {

    }
}
