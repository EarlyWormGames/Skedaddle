using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

public class EditorScreenshot : EditorWindow
{
    [MenuItem("Tools/Screenshot/Screenshot Settings %#&s")]
    public static void OpenWindow()
    {
        var window = GetWindow<EditorScreenshot>();
        window.titleContent = new GUIContent("Screenshot");
    }

    [MenuItem("Tools/Screenshot/Take Screenshot %&s")]
    public static void TakeScreenshot()
    {
        DoScreenshot();
    }

    private ScreenshotSettings settings;

    private void OnGUI()
    {
        if (settings == null)
        {
            bool useDefault = true;
            settings = ScreenshotSettings.Load(out useDefault);

            if (useDefault)
            {
                settings.cam = Camera.main;
                if (settings.cam != null)
                {
                    settings.width = settings.cam.pixelWidth;
                    settings.height = settings.cam.pixelHeight;
                }
                settings.rectPos = Vector2.zero;
                settings.rectSize = Vector2.one;
            }
        }

        GUILayout.BeginVertical();

        //SET SCREENSHOT RESOLUTION
        EditorGUILayout.LabelField("Screenshot Settings", EditorStyles.boldLabel);
        settings.width = EditorGUILayout.IntField("Width:", settings.width);
        settings.height = EditorGUILayout.IntField("Height:", settings.height);

        settings.width = Mathf.Clamp(settings.width, 1, int.MaxValue);
        settings.height = Mathf.Clamp(settings.height, 1, int.MaxValue);

        if (GUILayout.Button("Reset"))
        {
            Camera cam = settings.cam;
            if (settings.useSceneCamera)
                cam = SceneView.lastActiveSceneView.camera;
            else if (settings.useDefaultCamera)
                cam = Camera.main;

            if (cam != null)
            {             
                settings.width = cam.pixelWidth;
                settings.height = cam.pixelHeight;
            }
        }
        var screenRect = new Rect(settings.rectPos, settings.rectSize);
        screenRect = EditorGUILayout.RectField("Screen rect:", screenRect);

        settings.rectPos = screenRect.position;
        settings.rectSize = screenRect.size;

        //Clamp the rect to 0,0,0,0 -> 1,1,1,1
        settings.rectPos.x = Mathf.Clamp01(settings.rectPos.x);
        settings.rectPos.y = Mathf.Clamp01(settings.rectPos.y);
        settings.rectSize.x = Mathf.Clamp01(settings.rectSize.x);
        settings.rectSize.y = Mathf.Clamp01(settings.rectSize.y);

        GUILayout.Space(5);

        if (settings.cam == null && settings.useDefaultCamera)
            settings.cam = Camera.main;

        settings.cam = EditorGUILayout.ObjectField("Camera:", settings.cam, typeof(Camera), true) as Camera;
        settings.useSceneCamera = EditorGUILayout.Toggle("Use Scene Camera: ", settings.useSceneCamera);

        settings.useDefaultCamera = settings.cam == Camera.main;

        GUILayout.Space(5);

        //SELECT FOLDER
        EditorGUILayout.LabelField("Folder settings", EditorStyles.boldLabel);
        GUILayout.BeginHorizontal();
        settings.folder = EditorGUILayout.TextField("Folder:", settings.folder);
        if (GUILayout.Button("Select"))
        {
            settings.folder = EditorUtility.OpenFolderPanel("Select screenshot folder", settings.folder, "");
        }
        GUILayout.EndHorizontal();

        GUILayout.Space(5);
 
        settings.Save();

        //TAKE SCREENSHOT
        if (GUILayout.Button("Take Screenshot"))
        {
            DoScreenshot();
        }

        GUILayout.EndVertical();
    }

    public static void DoScreenshot()
    {
        ScreenshotSettings settings = null;
        bool useDefault = true;
        settings = ScreenshotSettings.Load(out useDefault);

        if (useDefault)
        {
            settings.cam = Camera.main;
            if (settings.cam != null)
            {
                settings.width = settings.cam.pixelWidth;
                settings.height = settings.cam.pixelHeight;
            }
            settings.rectPos = Vector2.zero;
            settings.rectSize = Vector2.one;
        }

        Camera cam = settings.cam;

        if (settings.useSceneCamera)
            cam = SceneView.lastActiveSceneView.camera;
        else if (settings.useDefaultCamera)
            cam = Camera.main;

        if (cam == null)
        {
            Debug.LogWarning("Screenshot camera was null!");
            return;
        }

        Texture2D tex = new Texture2D(settings.width, settings.height, TextureFormat.ARGB32, false);
        RenderTexture rt = new RenderTexture(settings.width, settings.height, 24, RenderTextureFormat.ARGB32);

        //Store the old texture
        var oldTex = cam.targetTexture;
        cam.targetTexture = rt;

        try
        {
            cam.Render();
        }
        catch
        {
            Debug.LogError("Screenshot camera render failed!");
            cam.targetTexture = oldTex;
            return;
        }
        cam.targetTexture = oldTex;

        //Store old render texture
        var oldrt = RenderTexture.active;
        RenderTexture.active = rt;
        try
        {
            var rect = new Rect(settings.rectPos, settings.rectSize);
            rect.width = rect.width * settings.width;
            rect.height = rect.height * settings.height;

            tex.ReadPixels(rect, 0, 0);
            tex.Apply();
        }
        catch
        {
            Debug.LogError("Failed to read pixels from screen!");
            RenderTexture.active = oldrt;
            return;
        }
        RenderTexture.active = oldrt;

        try
        {
            byte[] bytes = tex.EncodeToPNG();
            DestroyImmediate(tex);
            DestroyImmediate(rt);

            File.WriteAllBytes(settings.folder + "/Screenshot_" + DateTime.Now.Ticks + ".png", bytes);
        }
        catch
        {
            Debug.LogError("Failed to write screenshot to file!");
            return;
        }

        AssetDatabase.Refresh();
    }
}

[Serializable]
public class ScreenshotSettings
{
    public string folder;
    public int width, height;
    public SV2 rectPos, rectSize;
    [NonSerialized]
    public Camera cam;
    public bool useDefaultCamera;
    public bool useSceneCamera;

    public static ScreenshotSettings Load(out bool wasDefault)
    {
        string path = Application.persistentDataPath + "/EditorScreenshotSettings.data";
        var save = new ScreenshotSettings();
        if (!File.Exists(path))
        {
            wasDefault = true;
            return save;
        }

        //Open the file
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(path, FileMode.Open);

        //Read the data
        var s = (ScreenshotSettings)bf.Deserialize(file);

        //Close (to avoid leaks and such)
        file.Close();

        //Set the local value
        save = s;
        wasDefault = false;
        return save;
    }

    public void Save()
    {
        string path = Application.persistentDataPath + "/EditorScreenshotSettings.data";

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(path);

        bf.Serialize(file, this);
        file.Close();
    }
}

[Serializable]
public class SV2
{
    public float x, y;

    public SV2(float x, float y)
    {
        this.x = x;
        this.y = y;
    }

    public static implicit operator Vector2(SV2 x)
    {
        return new Vector2(x.x, x.y);        
    }

    public static implicit operator SV2(Vector2 x)
    {
        return new SV2(x.x, x.y);
    }
}