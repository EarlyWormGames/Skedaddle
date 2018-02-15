using UnityEngine;
using UnityEditor;
using System.Collections;

public class CutsceneEditor : EditorWindow
{
    [MenuItem("Window/EW Cutscene Item View", false, 3)]
    public static void ShoWindow()
    {
        GetWindow<CutsceneEditor>().Init();
    }

    void Init()
    {
        titleContent = new GUIContent("EW Item View");
    }

    void OnGUI()
    {

    }
}
