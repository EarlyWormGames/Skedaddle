using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEditor;

[InitializeOnLoad]
class TestOpen
{
    static TestOpen()
    {
        if(!File.Exists(Application.persistentDataPath + "/testing.txt"))
        {
            File.CreateText(Application.persistentDataPath + "/testing.txt");
        }
        else if(string.IsNullOrEmpty(File.ReadAllText(Application.persistentDataPath + "/testing.txt")))
        {
            File.WriteAllText(Application.persistentDataPath + "/testing.txt", "huehuehue");
            var editor = EditorWindow.GetWindow<TestVideo>();
            editor.position = new Rect(editor.position.position, new Vector2(1280, 720));
        }
    }
}

public class TestVideo : EditorWindow
{
    private VideoPlayer player;
    private RenderTexture texture;

    private void OnEnable()
    {
        player = new GameObject().AddComponent<VideoPlayer>();
        var audio = player.gameObject.AddComponent<AudioSource>();

        texture = new RenderTexture(1280, 720, 0, RenderTextureFormat.ARGB32);
        var video = AssetDatabase.LoadAssetAtPath<VideoClip>("Assets/Resources/TestVideo1.mp4");

        player.clip = video;
        player.renderMode = VideoRenderMode.RenderTexture;
        player.targetTexture = texture;
        player.audioOutputMode = VideoAudioOutputMode.AudioSource;
        player.EnableAudioTrack(0, true);
        player.SetTargetAudioSource(0, audio);
        player.playOnAwake = false;

        player.Play();

    }

    // Update is called once per frame
    void Update()
    {
        Repaint();
    }

    private void OnGUI()
    {
        GUI.DrawTexture(new Rect(0, 0, position.width, position.height), texture, ScaleMode.ScaleToFit, false);
    }

    private void OnDestroy()
    {
        player.Stop();
        texture.Release();

        DestroyImmediate(player.gameObject);
    }
}