using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

[CustomEditor(typeof(Chest))]
public class ChestEditor : Editor
{
    Chest self;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        self = target as Chest;

        if (self.Manager != null)
        {
            if (self.GUID == 0)
            {
                NewID();
            }
            else
            {
                if (self.Manager.chests.Count < self.GUID)
                {
                    NewID();
                }
                else if (self.Manager.chests[self.GUID - 1] != self)
                {
                    NewID();
                }
            }
        }
    }

    void NewID()
    {
        self.Manager.chests.Add(self);
        self.GUID = self.Manager.chests.Count;
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
    }
}