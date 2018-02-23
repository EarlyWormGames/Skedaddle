using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEditor;
using UnityEngine.SceneManagement;

[InitializeOnLoad]
public class LevelNotesEditor
{
    private static LevelNotes.TextPoint currentPoint;
    private static LevelNotes currentFlow;

    private static LevelNotes.TextPoint editPoint, deletePoint;

    static LevelNotesEditor()
    {
        SceneView.onSceneGUIDelegate += ShowGUI;
        Selection.selectionChanged += SelectChanged;
    }

    [DrawGizmo(GizmoType.NonSelected | GizmoType.Selected)]
    static void DrawGameObjectName(LevelNotes flowObject, GizmoType gizmoType)
    {
        if (currentFlow == null)
            currentFlow = flowObject;

        GUIStyle mystyle = new GUIStyle(GUI.skin.label);
        mystyle.normal.textColor = Color.white;

        foreach (var item in currentFlow.points)
        {
            float size = HandleUtility.GetHandleSize(item.position) * 0.05f;

            if (item.showText)
                Handles.Label(item.position, item.text, mystyle);
        }
    }

    static void ShowGUI(SceneView sceneView)
    {
        if (currentFlow == null)
            return;

        foreach (var item in currentFlow.points)
        {
            float size = HandleUtility.GetHandleSize(item.position) * 0.05f;

            if (Handles.Button(item.position, Quaternion.identity, size, size * 1.1f, Handles.DotHandleCap))
            {
                currentPoint = item;
            }
        }

        if (currentPoint != null)
        {
            EditorGUI.BeginChangeCheck();
            var point = Handles.DoPositionHandle(currentPoint.position, Quaternion.identity);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(currentFlow, "Move Point");
                EditorUtility.SetDirty(currentFlow);
                currentPoint.position = point;
            }
        }
    }

    static void SelectChanged()
    {
        currentPoint = null;
    }

    [MenuItem("Tools/Level Notes/Add Note")]
    static void AddPoint()
    {
        LevelNotes[] flowArray = FindObjectsOfTypeAll<LevelNotes>().ToArray();
        LevelNotes flow = null;
        if (flowArray.Length != 0)
            flow = flowArray[0];
        else
        {
            flow = new GameObject("Level Notes").AddComponent<LevelNotes>();
        }

        Vector3 pos = SceneView.lastActiveSceneView.camera.transform.position;
        pos += SceneView.lastActiveSceneView.camera.transform.forward * 2;

        flow.points.Add(new LevelNotes.TextPoint(pos, "Level Note"));

        currentPoint = flow.points[flow.points.Count - 1];
        EditText();
    }

    [MenuItem("Tools/Level Notes/Hide All Notes")]
    static void HideFlow()
    {
        LevelNotes[] flow = FindObjectsOfTypeAll<LevelNotes>().ToArray();
        if (flow.Length == 0)
            return;

        flow[0].gameObject.SetActive(false);
    }

    [MenuItem("Tools/Level Notes/Show All Notes")]
    static void ShowFlow()
    {
        LevelNotes[] flow = FindObjectsOfTypeAll<LevelNotes>().ToArray();
        if (flow.Length == 0)
            return;

        flow[0].gameObject.SetActive(true);
    }

    [MenuItem("Tools/Level Notes/Hide Selected Note")]
    static void HideText()
    {
        if (currentPoint != null)
            currentPoint.showText = false;
    }

    [MenuItem("Tools/Level Notes/Show Selected Note")]
    static void ShowText()
    {
        if (currentPoint != null)
            currentPoint.showText = true;
    }

    [MenuItem("Tools/Level Notes/Edit Selected Note")]
    static void EditText()
    {
        if (currentPoint == null)
            return;

        editPoint = currentPoint;
        InputWindow.Show(EditCallback, editPoint.text);
    }

    static void EditCallback(string text)
    {
        if (editPoint == null)
            return;

        editPoint.text = text;
        editPoint = null;
    }

    [MenuItem("Tools/Level Notes/Delete Selected Note")]
    static void DeleteNote()
    {
        if (currentPoint == null)
            return;

        deletePoint = currentPoint;
        ConfirmWindow.Show(DeleteCallback, "Are you sure you want to delete this note?",
            "The note with the following text will be deleted: " + deletePoint.text);
    }

    static void DeleteCallback(bool confirmed)
    {
        if (confirmed)
        {
            currentFlow.points.Remove(deletePoint);
        }
        deletePoint = null;
        currentPoint = null;
    }


    /// Use this method to get all loaded objects of some type, including inactive objects. 
    /// This is an alternative to Resources.FindObjectsOfTypeAll (returns project assets, including prefabs), and GameObject.FindObjectsOfTypeAll (deprecated).
    public static List<T> FindObjectsOfTypeAll<T>()
    {
        List<T> results = new List<T>();
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            var s = SceneManager.GetSceneAt(i);
            if (s.isLoaded)
            {
                var allGameObjects = s.GetRootGameObjects();
                for (int j = 0; j < allGameObjects.Length; j++)
                {
                    var go = allGameObjects[j];
                    results.AddRange(go.GetComponentsInChildren<T>(true));
                }
            }
        }
        return results;
    }
}

public class InputWindow : EditorWindow
{
    private UnityAction<string> onFinish;
    private string text = "Level Note";

    public static void Show(UnityAction<string> finishEvent, string text)
    {
        var window = GetWindow<InputWindow>();
        window.onFinish = finishEvent;
        window.position = new Rect(window.position.x, window.position.y, 400, 200);
        window.CenterOnMainWin();
        window.text = text;

        window.ShowPopup();
    }

    private void OnGUI()
    {
        float halfHeight = position.height / 2;
        text = EditorGUI.TextArea(new Rect(5, 5, position.width - 10, halfHeight - 10), text);
        if (GUI.Button(new Rect(5, halfHeight + 5, position.width - 10, halfHeight - 10), "Submit"))
        {
            onFinish(text);
            Close();
        }
    }
}

public class ConfirmWindow : EditorWindow
{
    private UnityAction<bool> onFinish;
    private string text;

    public static void Show(UnityAction<bool> finishEvent, string titleText, string messageText)
    {
        var window = GetWindow<ConfirmWindow>();
        window.onFinish = finishEvent;
        window.text = messageText;
        window.titleContent = new GUIContent(titleText);
        window.position = new Rect(window.position.x, window.position.y, 400, 200);
        window.CenterOnMainWin();

        window.ShowPopup();
    }

    private void OnGUI()
    {
        float halfWidth = position.width / 2;
        float halfHeight = position.height / 2;

        GUI.Label(new Rect(5, 5, position.width - 10, halfHeight - 10), text);
        if (GUI.Button(new Rect(5, halfHeight + 5, halfWidth - 10, halfHeight - 10), "Yes"))
        {
            onFinish(true);
            Close();
        }
        else if (GUI.Button(new Rect(halfWidth + 5, halfHeight + 5, halfWidth - 10, halfHeight - 10), "No"))
        {
            onFinish(false);
            Close();
        }
    }
}