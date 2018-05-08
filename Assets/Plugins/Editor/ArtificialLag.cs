using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ArtificialLag : EditorWindow
{
    static int LagValue;
    static bool lagging = false;

    [MenuItem("Tools/Artificial Lag")]
    public static void DoShow()
    {
        GetWindow<ArtificialLag>().Show();
    }

    private void OnEnable()
    {
        EditorApplication.update += OnUpdate;
    }

    private void OnDisable()
    {
        EditorApplication.update -= OnUpdate;
    }

    private void OnGUI()
    {
        LagValue = EditorGUILayout.IntSlider(LagValue, 0, 100000000);

        if (GUILayout.Button(lagging ? "Stop lagging" : "Lag!"))
        {
            lagging = !lagging;
        }
    }

    private void OnUpdate()
    {
        if (lagging)
        {
            float x = 1;
            for (int i = 0; i < LagValue; ++i)
            {
                //Do nothing! Yay!
                x *= i;
            }
        }
    }
}