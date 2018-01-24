using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(HeightfieldWater))]
public class HeightfieldWaterEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Generate Mesh"))
        {
            ((HeightfieldWater)target).Awake();
        }
    }
}