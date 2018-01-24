using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class MissingMaterial : EditorWindow
{
    [MenuItem("Missing Material/Fix Mising Materials")]
    static void FixMaterials()
    {
        MissingMaterial window = GetWindow<MissingMaterial>();
        window.Init();
        window.Show();
    }


    private List<Renderer> m_Renderer;
    private GUIStyle m_LargeStyle;

    void Init()
    {
        m_Renderer = new List<Renderer>();
        position = new Rect(Screen.width / 2, Screen.height / 2, 500, 100);
        maxSize = new Vector2(500, 100);
        minSize = new Vector2(500, 100);

        Renderer[] renders = FindObjectsOfType<Renderer>();
        foreach (Renderer render in renders)
        {
            if (render.sharedMaterial == null)
                m_Renderer.Add(render);
        }

        if (m_Renderer.Count == 0)
        {
            Close();
            return;
        }
    }

    void OnGUI()
    {
        if (m_LargeStyle == null)
        {
            m_LargeStyle = new GUIStyle(GUI.skin.label);
            m_LargeStyle.fontSize = 25;
            m_LargeStyle.alignment = TextAnchor.MiddleCenter;
        }

        GUILayout.BeginHorizontal();
        GUILayout.Space(10);
        GUILayout.BeginVertical();
        GUILayout.Space(10);


        if (m_Renderer.Count > 0)
        {
            Selection.activeGameObject = m_Renderer[0].gameObject;
            SceneView.lastActiveSceneView.FrameSelected();

            m_Renderer[0].sharedMaterial = (Material)EditorGUILayout.ObjectField("Select new material", m_Renderer[0].sharedMaterial, typeof(Material), false);

            GUILayout.Space(20);

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Next"))
            {
                m_Renderer.RemoveAt(0);
            }
            if (GUILayout.Button("Close"))
            {
                Close();
            }
            GUILayout.EndHorizontal();
            if (GUILayout.Button("Apply to all"))
            {
                for (int i = 1; i < m_Renderer.Count; ++i)
                {
                    m_Renderer[i].sharedMaterial = m_Renderer[0].sharedMaterial;
                }
                m_Renderer.Clear();
            }
        }
        else
        {
            GUI.Label(new Rect(5, 5, position.width - 10, 65), "All done!", m_LargeStyle);

            if (GUI.Button(new Rect(5, 75, position.width - 10, 20), "Close"))
            {
                Close();
            }
        }

        GUILayout.EndVertical();
        GUILayout.EndHorizontal();
    }
}