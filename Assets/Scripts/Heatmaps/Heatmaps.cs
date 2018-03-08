using System.Net;
using System.Timers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Text;
using System.IO;

[AddComponentMenu("")]
public class Heatmaps : MonoBehaviour
{
    public enum CHARACTER
    {
        LORIS,
        POODLE,
        ANTEATER,
        ZEBRA,
        ELEPHANT
    }

    private class SendRequest
    {
        public string levelName;
        public CHARACTER player;
        public string xPos;
        public string yPos;
        public string zPos;
        public float time;

        public SendRequest(string name, CHARACTER character, string xpos, string ypos, string zpos, float t)
        {
            levelName = name;
            player = character;
            xPos = xpos;
            yPos = ypos;
            zPos = zpos;
            time = t;
        }
    }

    public static Heatmaps instance;
    private List<SendRequest> sendList = new List<SendRequest>();
    private string lastDebug = null;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void OnGameStart()
    {
        new GameObject().AddComponent<Heatmaps>();
    }

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        Timer t = new Timer(1000);
        t.Elapsed += TimerTick;
        t.Start();
    }

    private void Update()
    {
        if (lastDebug != null)
            Debug.Log(lastDebug);
        lastDebug = null;
    }

    public void SendData(string levelName, CHARACTER player, Vector3 position, float time, bool allowInEditor = false)
    {
#if UNITY_EDITOR
        if (!allowInEditor)
            return;
#endif
        string url = string.Format("http://earlyworm.com.au/heatmaps//skedaddle/add_data.php?levelName={0}&playerIndex={1}&xPos={2}&yPos={3}&zPos={4}&time={5}",
            new[] { levelName, ((int)player).ToString(), position.x.ToString(), position.y.ToString(), position.z.ToString(), time.ToString() });

        sendList.Add(new SendRequest(levelName, player, position.x.ToString(), position.y.ToString(), position.z.ToString(), time));
    }

    private void TimerTick(object sender, ElapsedEventArgs e)
    {
        SendRequest[] data = sendList.ToArray();
        sendList.Clear();

        if (data.Length == 0)
            return;
        
        string levelNames = "", playerIndicies = "", times = "";
        string xPos = "", yPos = "", zPos = "";

        int index = 0;
        foreach (var item in data)
        {
            string front = "";
            if (index != 0)
                front = ";";

            string newline = front + index;
            levelNames += front + item.levelName;
            playerIndicies += front + ((int)item.player);
            times += front + item.time;

            xPos += front + item.xPos;
            yPos += front + item.yPos;
            zPos += front + item.zPos;

            ++index;
        }

        var request = (HttpWebRequest)WebRequest.Create("http://earlyworm.com.au/heatmaps/skedaddle/add_data.php");
        string postData = "levelName=" + levelNames;
        postData += "&playerIndex=" + playerIndicies;
        postData += "&xPos=" + xPos;
        postData += "&yPos=" + yPos;
        postData += "&zPos=" + zPos;
        postData += "&time=" + times;
        var sendingData = Encoding.Default.GetBytes(postData);

        request.Method = "POST";
        request.ContentType = "application/x-www-form-urlencoded";
        request.ContentLength = sendingData.Length;

        var stream = request.GetRequestStream();
        stream.Write(sendingData, 0, sendingData.Length);

        var response = (HttpWebResponse)request.GetResponse();

        lastDebug = new StreamReader(response.GetResponseStream()).ReadToEnd();
    }
}