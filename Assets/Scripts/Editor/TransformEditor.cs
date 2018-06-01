using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Reflection;
using UnityEngine.Internal;


/// <summary>
/// Transform editor that can be included in an editor window.
/// </summary>
[CustomEditor(typeof(Transform))]
public class TransformEditor : Editor
{
    public override void OnInspectorGUI()
    {
        Transform obj = (Transform)target;

        float maxElemWidth = 0f;
        float minElemWidth = 0f;

        GUIStyle.none.CalcMinMaxWidth(new GUIContent("Position"), out minElemWidth, out maxElemWidth);
        minElemWidth += 7f;

        //The spacing between the types
        EditorGUILayout.BeginVertical();

        //Position
        {
            EditorGUILayout.BeginHorizontal();

            //Vector3 rot = obj.localRotation.eulerAngles;
            EditorGUI.BeginChangeCheck();
            Vector3 pos = EditorGUILayout.Vector3Field("Position:", obj.localPosition);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(obj, "Position");
                obj.localPosition = pos;
            }            

            //End Position
            EditorGUILayout.EndHorizontal();
        }

        //Rotation
        {
            EditorGUILayout.BeginHorizontal();

            //Vector3 rot = obj.localRotation.eulerAngles;
            EditorGUI.BeginChangeCheck();
            Vector3 rot = EditorGUILayout.Vector3Field("Rotation:", obj.localRotation.eulerAngles);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(obj, "Rotation");
                obj.localRotation = Quaternion.Euler(rot);
            }

            //End Position
            EditorGUILayout.EndHorizontal();
        }

        //Scale
        {
            EditorGUILayout.BeginHorizontal();

            //Vector3 rot = obj.localRotation.eulerAngles;
            EditorGUI.BeginChangeCheck();
            Vector3 scale = EditorGUILayout.Vector3Field("Scale:", obj.localScale);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(obj, "Scale");
                obj.localScale = scale;
            }

            //End Position
            EditorGUILayout.EndHorizontal();
        }

        //Reset buttons
        {
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField("Zero", GUILayout.Width(minElemWidth));
            if (GUILayout.Button("Pos"))
            {
                Undo.RecordObject(obj, "Position");
                obj.localPosition = Vector3.zero;
            }

            if (GUILayout.Button("Rot"))
            {
                Undo.RecordObject(obj, "Rotation");
                obj.localRotation = Quaternion.identity;
            }

            if (GUILayout.Button("Scale"))
            {
                Undo.RecordObject(obj, "Scale");
                obj.localScale = new Vector3(1, 1, 1);
            }

            EditorGUILayout.EndHorizontal();
        }

        if (GUILayout.Button("Remove all comps"))
        {
            if (EditorUtility.DisplayDialog("Delete all?", "Are you sure you want to delete all components on this object?", "Delete", "Cancel"))
            {
                Undo.RecordObject(obj, "Delete all comps");

                // Create a serialized object so that we can edit the component list
                var serializedObject = new SerializedObject(obj.gameObject);
                // Find the component list property
                var prop = serializedObject.FindProperty("m_Component");

                Component[] comps = obj.GetComponents<Component>();

                int removed = 0;
                for (int i = 0; i < comps.Length; ++i)
                {
                    if (comps[i] != null)
                    {
                        if (comps[i].GetType() != typeof(Transform))
                        {
                            prop.DeleteArrayElementAtIndex(i - removed);
                            removed++;
                        }
                    }
                    else
                    {
                        prop.DeleteArrayElementAtIndex(i - removed);
                        removed++;
                    }
                }
                serializedObject.ApplyModifiedProperties();
            }
        }

        EditorGUILayout.EndVertical();
    }
}
