using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(AudioMaterial))]
public class AudioMatEditor : Editor
{
    public override void OnInspectorGUI()
    {
        AudioMaterial mat = (AudioMaterial)target;

        if (!mat.m_IsSurface)
        {
            EditorGUI.BeginChangeCheck();
            AudioMaterial.eMaterial em = (AudioMaterial.eMaterial)EditorGUILayout.EnumPopup("Material", mat.m_Material);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(target, "Mat change");
                mat.m_Material = em;
            }

            if (GUILayout.Button("Switch to Surface"))
            {
                Undo.RecordObject(target, "Surface change");
                mat.m_IsSurface = true;
                Repaint();
            }
        }
        else
        {
            EditorGUI.BeginChangeCheck();
            AudioMaterial.eSurface es = (AudioMaterial.eSurface)EditorGUILayout.EnumPopup("Surface", mat.m_Surface);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(target, "Surface change");
                mat.m_Surface = es;
            }

            if (GUILayout.Button("Switch to Material"))
            {
                Undo.RecordObject(target, "Mat change");
                mat.m_IsSurface = false;
                Repaint();
            }
        }
    }
}
