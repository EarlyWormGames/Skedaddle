using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HeatmapTracker : MonoBehaviour
{
    //=====================================================
    //INSTANCE VARS
    public string m_sName = "TrackedObject"; //Item must have a name
    public float m_TrackTime = 0f;
    private float m_Timer = 0f;

    private List<Vector3> m_Positions;
    private bool m_bDoInEditor = false;
    //=====================================================

    //=====================================================
    //INSTANCE FUNCTIONS
    void Awake()
    {
        if (m_sName.Length == 0)
            enabled = false;

        HeatmapClient.Init();
        HeatmapClient.StartClient();
    }

    // Update is called once per frame
    void Update()
    {
        m_Timer += Time.deltaTime;
        if (m_Timer >= m_TrackTime)
        {
            m_Timer = 0f;

            if (m_Positions == null)
                m_Positions = new List<Vector3>();

            m_Positions.Add(transform.position);
        }
    }

    void OnDestroy()
    {
#if UNITY_EDITOR
        if (!m_bDoInEditor)
            return;
#endif

        if (m_Positions == null)
            return;
        if (m_Positions.Count == 0)
            return;

        var myUniqueFileName = string.Format(@"{0}.txt", DateTime.Now.Ticks);

        string sceneName = SceneManager.GetActiveScene().name;

        //Create the temp directory if it dont exist boii
        //The actual heatmap data will be stored in a different folder
        string folderPath = Application.persistentDataPath + "/TempHeatmaps/" + sceneName + "/" + m_sName + "/";
        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);

        //Append all the points on a new line each
        string output = "";
        foreach (Vector3 point in m_Positions)
        {
            output += V3ToString(point) + "\n";
        }

        //Write all this to a new file and ask the net client to pls send if it can
        File.WriteAllText(folderPath + myUniqueFileName, output);
        HeatmapClient.CheckFiles();
    }

    void OnApplicationQuit()
    {
        HeatmapClient.Quitting = true;
    }

    private static string V3ToString(Vector3 a_vec)
    {
        //Wanted to format like this so I can easily string.split to put a position back together
        return a_vec.x.ToString() + "," + a_vec.y.ToString() + "," + a_vec.z.ToString();
    }
}