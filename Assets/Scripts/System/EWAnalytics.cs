using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System.Net;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// management of sending the analytics data to the server
/// </summary>
public class EWAnalytics : Singleton<EWAnalytics>
{
    public string m_sAPIKey;

    private static string m_sAnalPath;
    private static string m_sProjectName;

    // Use this for initialization
    void Start()
    {
        DontDestroy();
        //run on a different thread
        StartCoroutine(Startup());
    }

    /// <summary>
    /// try connect ot the EW Server
    /// </summary>
    /// <returns></returns>
    IEnumerator Startup()
    {
        WWW req = new WWW("http://www.earlyworm.com.au/analytics/Projects/getProject.php?key=" + m_sAPIKey);

        yield return req;

        if (req.error == null)
        {
            bool exit = true;
            string result = req.text;

            if (result == "" || result == null || result == "none")
            {
                exit = true;
            }
            else
            {
                m_sProjectName = result;
                m_sAnalPath = "http://www.earlyworm.com.au/analytics/Projects/" + result + "/add.php?apikey=" + m_sAPIKey;
                exit = false;
            }

            if (exit)
            {
                EWDebug.LogError("Analytics API Key is incorrect!");
                Destroy(this);
                yield break;
            }

            EWDebug.Log("Analytics connected for project: " + result);
            Dictionary<string, string> myDic = new Dictionary<string, string>();
            SendTable("PlayStart", myDic);
        }
        else
        {
            EWDebug.LogError("Startup Error!");
        }
    }

    // Update is called once per frame
    void Update()
    {
    }

    /// <summary>
    /// Send the data to the server
    /// </summary>
    /// <param name="a_tableName"></param>
    /// <param name="a_dic"></param>
    public static void SendTable(string a_tableName, Dictionary<string, string> a_dic = null)
    {
        if (Instance == null)
        {
            return;
        }

        if (a_tableName == "" || a_tableName == null)
        {
            EWDebug.Log("You must enter a table name!");
            return;
        }

        a_tableName = a_tableName.Replace(" ", "");

        string requestInfo = "&tableName=" + a_tableName;
        if (a_dic != null)
        {
            foreach (KeyValuePair<string, string> entry in a_dic)
            {
                requestInfo += "&" + entry.Key + "=" + entry.Value;
            }
        }

        bool m_bIsDev = false;
#if UNITY_EDITOR
        m_bIsDev = true;
#endif
        requestInfo += "&isDev=" + m_bIsDev.ToString();
        Instance.StartCoroutine(Instance.SendData(m_sAnalPath + requestInfo));
    }

    /// <summary>
    /// SEND ALL
    /// </summary>
    /// <param name="a_url"></param>
    /// <returns></returns>
    public IEnumerator SendData(string a_url)
    {
        WWW req = new WWW(a_url);

        yield return req;

        if (req.error == null)
        {
            EWDebug.Log(req.text);
        }
        else
        {
            EWDebug.LogError("WWW Error!");
        }
    }

    /// <summary>
    /// SEND ALL
    /// </summary>
    /// <param name="a_name"></param>
    /// <param name="a_data"></param>
    public static void SendHeatmap(string a_name, string a_data)
    {
        if (Instance == null)
        {
            return;
        }

        Instance.StartCoroutine(Instance.SendData("http://www.earlyworm.com.au/analytics/Projects/" + m_sProjectName + "/Heatmaps/" + SceneManager.GetActiveScene().name + "/upload.php?objname=" + a_name + "&data=" + a_data));
    }
    /// <summary>
    /// BYEEEEE
    /// </summary>
    public void OnApplicationQuit()
    {
        SendTable("PlayEnd", null);
    }
}
