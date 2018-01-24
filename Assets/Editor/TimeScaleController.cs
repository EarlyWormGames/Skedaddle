// Created by Hugo "HRuivo" Ruivo

using UnityEditor;
using UnityEngine;

public class TimeScaleController : EditorWindow
{
    private float lastTimeScale = 1.0f;

    // You may change "Aealo Games" to something else
    [MenuItem("Aealo Games/Time Scale Controller")]
    static void Init()
    {
        TimeScaleController window = (TimeScaleController)EditorWindow.GetWindow(typeof(TimeScaleController));
        window.titleContent = new GUIContent("Time Scaler");
        window.Show();
        window.minSize = new Vector2(10, 30);
        window.position = new Rect(100, 200, 560, 32);
    }

    void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();

        Time.timeScale = EditorGUILayout.Slider(Time.timeScale, 0, 3);

        if (position.width > 468)
        {
            if (GUILayout.Button("0.25x", GUILayout.Height(20), GUILayout.Width(42)))
            {
                Time.timeScale = 0.25f;
            }
            if (GUILayout.Button("0.5x", GUILayout.Height(20), GUILayout.Width(42)))
            {
                Time.timeScale = 0.5f;
            }
            /*if (GUILayout.Button(".75", GUILayout.Height(20), GUILayout.Width(32)))
            {
                Time.timeScale = 0.75f;
            }*/

            if (Time.timeScale == 0)
            {
                if (GUILayout.Button("Resume", GUILayout.Height(20), GUILayout.Width(64)))
                {
                    Time.timeScale = lastTimeScale;
                }
            }
            else
            {
                if (GUILayout.Button("Pause", GUILayout.Height(20), GUILayout.Width(64)))
                {
                    lastTimeScale = Time.timeScale;
                    Time.timeScale = 0.0f;
                }
            }
            if (GUILayout.Button("Reset", GUILayout.Height(20), GUILayout.Width(64)))
            {
                Time.timeScale = 1.0f;
            }


            if (GUILayout.Button("2x", GUILayout.Height(20), GUILayout.Width(42)))
            {
                Time.timeScale = 2.0f;
            }
            /*if (GUILayout.Button("x3", GUILayout.Height(20), GUILayout.Width(32)))
            {
                Time.timeScale = 3.0f;
            }*/


        }

        EditorGUILayout.EndHorizontal();
    }
}
