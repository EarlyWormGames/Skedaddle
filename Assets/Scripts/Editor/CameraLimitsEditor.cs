using UnityEngine;
using UnityEditor;
using System.Collections;

/// <summary>
/// Debug draw for the camera limits
/// </summary>
[CustomEditor(typeof(CameraLimits))]
public class CameraLimitsEditor : Editor
{
    private Texture2D m_Inner;
    private Texture2D m_Outline;
    private Texture2D m_Circle;

    public override void OnInspectorGUI()
    {
        CameraLimits limits = (CameraLimits)target;

        EditorGUI.BeginChangeCheck();
        Vector2 x = EditorGUILayout.Vector2Field("X Limits", limits.m_v2XLimits);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(target, "X limits");
            limits.m_v2XLimits = x;
            SceneView.RepaintAll();
        }

        EditorGUI.BeginChangeCheck();
        Vector2 y = EditorGUILayout.Vector2Field("Y Limits", limits.m_v2YLimits);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(target, "Y limits");
            limits.m_v2YLimits = y;
            SceneView.RepaintAll();
        }
    }

    void OnSceneGUI()
    {
        if (Camera.current == null)
            return;

        CameraLimits limits = (CameraLimits)target;

        if (m_Inner == null)
        {
            m_Circle = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Gizmos/Handle.png");

            m_Inner = new Texture2D(1, 1);
            m_Inner.SetPixel(0, 0, new Color(1, 1, 1, 0.5f));
            m_Inner.Apply();

            m_Outline = new Texture2D(1, 1);
            m_Outline.SetPixel(0, 0, new Color(1, 1, 1, 1));
            m_Outline.Apply();
        }

        Handles.BeginGUI();

        Vector2 tl = Camera.current.WorldToScreenPoint(new Vector3(limits.m_v2XLimits.x, limits.m_v2YLimits.y, limits.transform.position.z));
        Vector2 bl = Camera.current.WorldToScreenPoint(new Vector3(limits.m_v2XLimits.x, limits.m_v2YLimits.x, limits.transform.position.z));

        Vector2 tr = Camera.current.WorldToScreenPoint(new Vector3(limits.m_v2XLimits.y, limits.m_v2YLimits.y, limits.transform.position.z));
        Vector2 br = Camera.current.WorldToScreenPoint(new Vector3(limits.m_v2XLimits.y, limits.m_v2YLimits.x, limits.transform.position.z));

        //Draw the inner
        //GUI.DrawTexture(new Rect(tl.x, Screen.height - bl.y, tr.x - tl.x, bl.y - tl.y), m_Inner);

        //Draw the outlines
        {

        }

        //Draw the handles
        {
            GUI.DrawTexture(new Rect(tl.x, Screen.height - tl.y, 25, 25), m_Circle);
            GUI.DrawTexture(new Rect(bl.x, Screen.height - bl.y, 25, 25), m_Circle);
            GUI.DrawTexture(new Rect(tr.x, Screen.height - tr.y, 25, 25), m_Circle);
            GUI.DrawTexture(new Rect(br.x, Screen.height - br.y, 25, 25), m_Circle);
        }

        Handles.EndGUI();
    }    
}
