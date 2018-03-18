using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

[CustomEditor(typeof(Chest))]
public class ChestEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var chest = target as Chest;

        CheckGUID(chest);
    }

    static void NewID(Chest chest)
    {
        int id = -1;
        if (!chest.Manager.AddChest(chest, out id))
            return;

        if (id == chest.GUID)
            return;

        Undo.RecordObject(chest, "change GUID");
        chest.GUID = id;
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
    }

    static void CheckGUID(Chest chest)
    {
        if (Application.isPlaying)
            return;

        if (AssetDatabase.Contains(chest.gameObject))
            return;

        if (chest.Manager != null)
        {
            NewID(chest);
        }
    }

    [DrawGizmo(GizmoType.NonSelected | GizmoType.Selected)]
    static void DrawGizmoForChest(Chest scr, GizmoType type)
    {
        CheckGUID(scr);
    }
}

[CustomEditor(typeof(ChestManager))]
public class ChestManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        EditorGUILayout.PrefixLabel("Count: ");
        EditorGUILayout.LabelField((target as ChestManager).ChestLength.ToString());
    }
}