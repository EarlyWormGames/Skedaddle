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

    private static LevelNotes.TextPoint editPoint, deletePoint, joinNode;
    private static List<LevelNotes.TextPoint> otherNotes;

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

        int fontSize = currentFlow.MaximumFontSize;
        float dist = currentFlow.DistanceMultiplier;

        mystyle.fontSize = fontSize;

        foreach (var item in currentFlow.points)
        {
            float size = HandleUtility.GetHandleSize(item.position);

            if (currentFlow.ShrinkOverDistance)
                mystyle.fontSize = fontSize - Mathf.Clamp((int)(size * dist), 0, fontSize - 1);
            mystyle.normal.textColor = item.color;

            Camera cam = SceneView.lastActiveSceneView.camera;
            if (EditorApplication.isPlaying)
                cam = Camera.current;

            Plane[] frustum = GeometryUtility.CalculateFrustumPlanes(cam);
            if (item.showText &&
                GeometryUtility.TestPlanesAABB(frustum, new Bounds(item.position, Vector3.one)))
            {
                Handles.Label(item.position, item.text, mystyle);
            }

            if (item.joinedPoint > -1)
            {
                Vector3 p1 = Vector3.zero, p2 = Vector3.zero;

                Vector3 p0 = item.position;
                Vector3 p3 = currentFlow.points[item.joinedPoint].position;
                Vector3 dir = (p3 - p0).normalized;

                float dotLeft = Vector3.Dot(dir, Vector3.left);
                float dotUp = Vector3.Dot(dir, Vector3.up);

                float absLeft = Mathf.Abs(dotLeft);
                float absUp = Mathf.Abs(dotUp);

                //if (absLeft > absUp)
                //{
                    p1 = p0 + (Vector3.left * 1.4f * (dotLeft < 0 ? -1 : 1));
                    p2 = p3 - (Vector3.left * 1.4f * (dotLeft < 0 ? -1 : 1));
                //}
                //else
                //{
                //    p1 = p0 + (Vector3.up * (p3 - p0).magnitude * (dotUp < 0 ? -1 : 1));
                //    p2 = p3 - (Vector3.up * (p3 - p0).magnitude * (dotUp < 0 ? -1 : 1));
                //}

                Handles.DrawBezier(p0, p3, p1, p2, new Color(0.7f, 0.7f, 0.7f), null, 2f);
            }
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
                Selection.activeGameObject = null;
            }
        }

        if (currentPoint != null)
        {
            EditorGUI.BeginChangeCheck();
            var point = Handles.DoPositionHandle(currentPoint.position, Quaternion.identity);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(currentFlow, "Move Point");
                currentPoint.position = point;
            }

            Event e = Event.current;
            switch (e.type)
            {
                case EventType.KeyDown:
                    {
                        if (Event.current.keyCode == (KeyCode.Delete))
                        {
                            DeleteNote();
                        }
                        break;
                    }
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

        Undo.RecordObject(flow[0].gameObject, "Hide Notes");
        flow[0].gameObject.SetActive(false);
    }

    [MenuItem("Tools/Level Notes/Show All Notes")]
    static void ShowFlow()
    {
        LevelNotes[] flow = FindObjectsOfTypeAll<LevelNotes>().ToArray();
        if (flow.Length == 0)
            return;

        Undo.RecordObject(flow[0].gameObject, "Show Notes");
        flow[0].gameObject.SetActive(true);
    }

    [MenuItem("Tools/Level Notes/Hide Selected Note")]
    static void HideText()
    {
        if (currentPoint != null)
        {
            Undo.RecordObject(currentFlow, "Hide Note");

            currentPoint.showText = false;
        }
    }

    [MenuItem("Tools/Level Notes/Show Selected Note")]
    static void ShowText()
    {
        if (currentPoint != null)
        {
            Undo.RecordObject(currentFlow, "Show Note");

            currentPoint.showText = true;
        }
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

        Undo.RecordObject(currentFlow, "Edit Note");

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
            Undo.RecordObject(currentFlow, "Delete Note");

            currentFlow.RemoveItem(deletePoint);
        }

        deletePoint = null;
        currentPoint = null;
    }

    [MenuItem("Tools/Level Notes/Connect Note")]
    static void JoinNote()
    {
        if (currentPoint == null)
            return;

        List<string> notes = new List<string>();
        otherNotes = new List<LevelNotes.TextPoint>();
        foreach (var item in currentFlow.points)
        {
            if (item == currentPoint)
                continue;

            notes.Add(item.text);
            otherNotes.Add(item);
        }

        joinNode = currentPoint;
        SelectorWindow.Show(JoinCallback, notes.ToArray());
    }

    static void JoinCallback(int index)
    {
        Undo.RecordObject(currentFlow, "Join Notes");

        joinNode.joinedPoint = currentFlow.points.IndexOf(otherNotes[index]);
        joinNode = null;
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

public class SelectorWindow : EditorWindow
{
    private UnityAction<int> onFinish;
    private string[] texts;

    private float itemHeight = 100;
    private Vector2 scrollPos;

    public static void Show(UnityAction<int> finishEvent, string[] texts)
    {
        var window = GetWindow<SelectorWindow>();
        window.onFinish = finishEvent;
        window.position = new Rect(window.position.x, window.position.y, 300, 400);
        window.CenterOnMainWin();
        window.texts = texts;

        window.ShowPopup();
    }

    private void OnGUI()
    {
        float width = position.width - 25;
        scrollPos = GUI.BeginScrollView(new Rect(5, 5, position.width - 10, position.height - 10), scrollPos,
            new Rect(0, 0, width, (texts.Length + 1) * itemHeight), false, true);

        if (GUI.Button(new Rect(0, 0, width, itemHeight), "None"))
        {
            onFinish(-1);
            Close();
            return;
        }

        for (int i = 0; i < texts.Length; ++i)
        {
            if (GUI.Button(new Rect(0, (i + 1) * itemHeight, width, itemHeight), texts[i]))
            {
                onFinish(i);
                Close();
                return;
            }
        }

        GUI.EndScrollView();
    }
}