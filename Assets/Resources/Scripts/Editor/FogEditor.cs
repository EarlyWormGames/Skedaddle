using UnityEngine;
using UnityEditor;
using System.Collections;

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
