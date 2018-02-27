using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public class OpenScene : EditorWindow
{
    Vector2 scrollPos;
    float buttonHeight = 100;

    [MenuItem("File/Open Level %#o")]
    public static void OpenLevel()
    {
        var window = GetWindow<OpenScene>();
        window.Show();
    }

    private void OnGUI()
    {
        float width = position.width - 25;

        var guids = AssetDatabase.FindAssets("t:Scene", new[] { "Assets/Resources/Scenes/Levels" });
        var scrollRect = new Rect(5, 5, position.width - 10, position.height - 10);
        scrollPos = GUI.BeginScrollView(scrollRect, scrollPos, new Rect(0, 0, width, guids.Length * buttonHeight));

        var btnStyle = new GUIStyle(GUI.skin.button);
        btnStyle.wordWrap = true;

        int i = 0;
        foreach (var guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
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
