using UnityEngine;
using UnityEditor;
using System.Collections;

/// <summary>
/// Custom fog editor
/// create and edit fog
/// </summary>
[CustomEditor(typeof(EWFog))]
public class FogEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EWFog fog = (EWFog)target;
        fog.CreateTex();
    }
}
