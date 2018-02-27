﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public class OpenScene : EditorWindow
{
    Vector2 scrollPos;
    float buttonHeight = 80;
    string search = "";

    [MenuItem("File/Open Level %#o")]
    public static void OpenLevel()
    {
        var window = GetWindow<OpenScene>();
        window.Show();
    }

    private void OnGUI()
    {
        search = GUI.TextField(new Rect(5, 5, position.width - 10, 20), search);

        float width = position.width - 25;
        float height = position.height - 35;

        var guids = AssetDatabase.FindAssets("t:Scene", new[] { "Assets/Resources/Scenes/Levels" });
        List<string> paths = new List<string>();
        foreach (var guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            if (path.IndexOf(search, System.StringComparison.OrdinalIgnoreCase) >= 0)
                paths.Add(path);
        }

        var scrollRect = new Rect(5, 30, position.width - 10, height);
        scrollPos = GUI.BeginScrollView(scrollRect, scrollPos, new Rect(0, 0, width, paths.Count * buttonHeight));

        var btnStyle = new GUIStyle(GUI.skin.button);
        btnStyle.wordWrap = true;
        btnStyle.fontSize = 13;

        int i = 0;
        foreach (var path in paths)
        {
            if (GUI.Button(new Rect(0, i * buttonHeight, width, buttonHeight), path, btnStyle))
            {
                if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                {
                    Close();
                    EditorSceneManager.OpenScene(path, OpenSceneMode.Single);
                }
            }
            ++i;
        }

        GUI.EndScrollView();

        Repaint();
    }
}
