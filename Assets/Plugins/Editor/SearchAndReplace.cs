using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SearchAndReplace : EditorWindow
{
    [MenuItem("Tools/Search And Replace %#&r")]
    public static void OpenWindow()
    {
        var window = GetWindow<SearchAndReplace>();
        window.Init();
    }

    private string searchTerm, replaceName;
    private bool matchCase, applyToAll, matchAll = true;

    void Init()
    {
        if (Selection.activeGameObject != null)
        {
            //Bonus feature to set the search term to the current item selected
            searchTerm = Selection.activeGameObject.name;
        }
    }

    private void OnGUI()
    {
        //Set the search term text
        GUILayout.BeginHorizontal();
        GUILayout.Label("Search Term");
        searchTerm = GUILayout.TextArea(searchTerm);
        GUILayout.EndHorizontal();

        //Set the replace term text
        GUILayout.BeginHorizontal();
        GUILayout.Label("Replace Term");
        replaceName = GUILayout.TextArea(replaceName);
        GUILayout.EndHorizontal();

        //Some toggles for extra options
        matchCase = GUILayout.Toggle(matchCase, "Match Case");
        applyToAll = GUILayout.Toggle(applyToAll, "Apply To All");
        matchAll = GUILayout.Toggle(matchAll, "Match Whole Name");

        if (GUILayout.Button("Replace"))
        {
            //Find all gameobjects in the scene
            string search = matchCase ? searchTerm : searchTerm.ToLower();
            GameObject[] objs = applyToAll ? FindObjectsOfType<GameObject>() : Selection.gameObjects;

            foreach (var item in objs)
            {
                //compare the names (using the options)
                bool rename = false;
                string itemName = matchCase? item.name : item.name.ToLower();
                if (itemName.CompareTo(search) == 0 && matchAll)
                    rename = true;
                else if (itemName.Contains(search) && !matchAll)
                    rename = true;

                //set an undo point and then rename them
                if (rename)
                {
                    Undo.RecordObject(item, "name change");
                    item.name = item.name.Replace(searchTerm, replaceName);
                }
            }
        }
    }
}
