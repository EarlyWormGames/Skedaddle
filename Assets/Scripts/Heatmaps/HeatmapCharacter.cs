using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Plot the points that the player has traveled and save the data
/// </summary>
public class HeatmapCharacter : MonoBehaviour
{
    [Tooltip("How far the character must move before data is sent")]
    public float MinimumMoveDist = 0.1f;
    public Heatmaps.CHARACTER Character;

    [Tooltip("Refreshes heatmap data in the editor")]
    public bool RefreshData;
    [Tooltip("Should this heatmap be drawn?")]
    public bool DrawGizmos = true;

    [Tooltip("First = Coldest, Last = Hottest")]
    public Gradient HeatmapGradient;

    public float HotTime = 2;
    public bool DynamicHotTime = false;

    [Tooltip("How large to draw each heatmap point (in world space)")]
    public Vector3 ColdSize = Vector3.one, HotSize = Vector3.one * 2;

    private float timer;
    private Dictionary<Vector3, float> pointTimes = new Dictionary<Vector3, float>();
    private int totalCount;
    private float dynaHotTime;

    private Vector3? lastPos;

    private void Update()
    {
        timer += Time.deltaTime;

        if (lastPos == null)
            lastPos = transform.position;

        float dist = Vector3.Distance(lastPos.Value, transform.position);
        //plot a point if the player has moved a certian distance
        if (dist >= MinimumMoveDist)
        {
            lastPos = transform.position;

            if (HeatmapSettings.instance)
                Heatmaps.instance.SendData(HeatmapSettings.instance.LevelName, Character, transform.position, timer, true);
            else
                Debug.LogError("There is no HeatmapSettings instance located in the scene");

            timer = 0;
        }
    }

    /// <summary>
    /// Draw the heatmap
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        if (!DrawGizmos)
            return;

        if (RefreshData)
        {
            RefreshData = false;
            StartCoroutine(GetPointData());
        }

        foreach (var pair in pointTimes)
        {
            float t = pair.Value / HotTime;
            if (DynamicHotTime)
                t = pair.Value / dynaHotTime;

            var color = HeatmapGradient.Evaluate(t);
            Gizmos.color = color;
            Gizmos.DrawCube(pair.Key, Vector3.Lerp(ColdSize, HotSize, t));
        }
    }

    /// <summary>
    /// retrieve the point information
    ///x </summary>
    /// <returns></returns>
    IEnumerator GetPointData()
    {
        var settings = FindObjectOfType<HeatmapSettings>();

        string url = string.Format("http://earlyworm.com.au/heatmaps//skedaddle/get_data.php?levelName={0}&playerIndex={1}",
            new[] { settings.LevelName, ((int)Character).ToString()});

        var www = UnityWebRequest.Get(url);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }

        string text = www.downloadHandler.text;

        string[] rows = text.Split(new[] { "<br/>" }, StringSplitOptions.RemoveEmptyEntries);
        totalCount = rows.Length;

        if (totalCount == 0)
        {
            Debug.Log("No heatmap data was returned");
            yield break;
        }

        dynaHotTime = 0;

        pointTimes = new Dictionary<Vector3, float>();
        var counts = new Dictionary<Vector3, int>();

        foreach (var row in rows)
        {
            string[] columns = row.Split(',');
            float x = Convert.ToSingle(columns[0]);
            float y = Convert.ToSingle(columns[1]);
            float z = Convert.ToSingle(columns[2]);
            float time = Convert.ToSingle(columns[3]);

            Vector3 pos = new Vector3(x, y, z);
            if (pointTimes.ContainsKey(pos))
            {
                pointTimes[pos] += time;
                counts[pos]++;
            }
            else
            {
                pointTimes.Add(pos, time);
                counts.Add(pos, 1);
            }
        }

        foreach (var pair in counts)
        {
            float time = pointTimes[pair.Key] / pair.Value;
            pointTimes[pair.Key] = time;

            if (time > dynaHotTime)
                dynaHotTime = time;
        }
    }
}