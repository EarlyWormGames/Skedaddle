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
        chest.Manager.chests.Add(chest);
        chest.GUID = chest.Manager.chests.Count;
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
    }

    static void CheckGUID(Chest chest)
    {
        if (AssetDatabase.Contains(chest.gameObject))
            return;

        if (chest.Manager != null)
        {
            if (chest.GUID == 0)
            {
                NewID(chest);
            }
            else
            {
                if (chest.Manager.chests.Count < chest.GUID)
                {
                    NewID(chest);
                }
                else if (chest.Manager.chests[chest.GUID - 1] != chest)
                {
                    NewID(chest);
                }
            }
        }
    }

    [DrawGizmo(GizmoType.NonSelected | GizmoType.Selected)]
    static void DrawGizmoForChest(Chest scr, GizmoType type)
    {
        CheckGUID(scr);
    }
}