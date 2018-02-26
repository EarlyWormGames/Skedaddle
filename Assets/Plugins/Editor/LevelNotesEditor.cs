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

    private static EditorWindow currentWindow;

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

            if (item.ropes == null)
                item.ropes = new List<int>();

            if (item.ropes.Count > 0)
            {
                foreach (var rope in item.ropes)
                {
                    Vector3 p1 = Vector3.zero, p2 = Vector3.zero;

                    Vector3 p0 = item.position;
                    Vector3 p3 = currentFlow.points[rope].position;
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
        }

        if (EditorWindow.focusedWindow == SceneView.currentDrawingSceneView)
        {
            Event e = Event.current;
            switch (e.type)
            {
                case EventType.KeyDown:
                    {
                        if (e.keyCode == KeyCode.Delete)
                        {
                            DeleteNote();
                        }
                        else if (e.modifiers == (EventModifiers.Shift | EventModifiers.Control))
                        {
                            switch (e.keyCode)
                            {
                                case KeyCode.G:
                                    AddPoint();
                                    break;
                                case KeyCode.E:
                                    EditNote();
                                    break;
                                case KeyCode.D:
                                    DuplicateNote();
                                    break;
                                case KeyCode.R:
                                    RopeNote();
                                    break;
                                case KeyCode.U:
                                    UnropeNote();
                                    break;
                                case KeyCode.T:
                                    ToggleNote();
                                    break;
                            }
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
        if (currentWindow != null)
            return;

        if (currentFlow == null)
        {
            LevelNotes[] flowArray = FindObjectsOfTypeAll<LevelNotes>().ToArray();
            if (flowArray.Length > 0)
                currentFlow = flowArray[0];
            else
            {
                currentFlow = new GameObject("Level Notes").AddComponent<LevelNotes>();
                Undo.RegisterCreatedObjectUndo(currentFlow.gameObject, "Create Notes");
            }
        }

        Vector3 pos = SceneView.lastActiveSceneView.camera.transform.position;
        pos += SceneView.lastActiveSceneView.camera.transform.forward * 2;

        Undo.RecordObject(currentFlow, "Add Note");
        currentFlow.points.Add(new LevelNotes.TextPoint(pos, "Level Note"));

        currentPoint = currentFlow.points[currentFlow.points.Count - 1];
        EditNote();
    }

    [MenuItem("Tools/Level Notes/Hide All Notes")]
    static void HideFlow()
    {
        if (currentWindow != null)
            return;

        LevelNotes[] flow = FindObjectsOfTypeAll<LevelNotes>().ToArray();
        if (flow.Length == 0)
            return;

        Undo.RecordObject(flow[0].gameObject, "Hide Notes");
        flow[0].gameObject.SetActive(false);
    }

    [MenuItem("Tools/Level Notes/Show All Notes")]
    static void ShowFlow()
    {
        if (currentWindow != null)
            return;

        LevelNotes[] flow = FindObjectsOfTypeAll<LevelNotes>().ToArray();
        if (flow.Length == 0)
            return;

        Undo.RecordObject(flow[0].gameObject, "Show Notes");
        flow[0].gameObject.SetActive(true);
    }

    [MenuItem("Tools/Level Notes/Toggle Note")]
    static void ToggleNote()
    {
        if (currentWindow != null)
            return;

        if (currentPoint != null)
        {
            Undo.RecordObject(currentFlow, "Toggle Note");
            currentPoint.showText = !currentPoint.showText;
        }
    }

    [MenuItem("Tools/Level Notes/Edit Note")]
    static void EditNote()
    {
        if (currentPoint == null)
            return;

        if (currentWindow != null)
            return;

        editPoint = currentPoint;
        currentWindow = EditNodeWindow.Show(EditCallback, editPoint.text, editPoint.color);
    }

    static void EditCallback(string text, Color color)
    {
        if (editPoint == null)
            return;

        Undo.RecordObject(currentFlow, "Edit Note");

        editPoint.text = text;
        editPoint.color = color;
        editPoint = null;
    }

    [MenuItem("Tools/Level Notes/Rope Note")]
    static void RopeNote()
    {
        if (currentPoint == null)
            return;

        if (currentWindow != null)
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
        currentWindow = SelectorWindow.Show(JoinCallback, notes.ToArray());
    }

    static void JoinCallback(int index)
    {
        Undo.RecordObject(currentFlow, "Join Notes");

        if (index >= 0)
        {
            int nodeIndex = currentFlow.points.IndexOf(otherNotes[index]);
            if (!joinNode.ropes.Contains(nodeIndex))
                joinNode.ropes.Add(nodeIndex);
        }

        joinNode = null;
    }

    [MenuItem("Tools/Level Notes/Unrope Note")]
    static void UnropeNote()
    {
        if (currentPoint == null)
            return;

        if (currentWindow != null)
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
        currentWindow = RemoveRopesWindow.Show(joinNode, currentFlow);
    }

    static void UnropeCallback(int index)
    {
        Undo.RecordObject(currentFlow, "Join Notes");

        if (index >= 0)
        {
            int nodeIndex = currentFlow.points.IndexOf(otherNotes[index]);
            if (!joinNode.ropes.Contains(nodeIndex))
                joinNode.ropes.Add(nodeIndex);
        }

        joinNode = null;
    }

    [MenuItem("Tools/Level Notes/Duplicate Note")]
    static void DuplicateNote()
    {
        if (currentPoint == null)
            return;

        if (currentWindow != null)
            return;

        Undo.RecordObject(currentFlow, "Duplicate Note");
        currentFlow.points.Add(new LevelNotes.TextPoint(currentPoint));
    }

    [MenuItem("Tools/Level Notes/Delete Note")]
    static void DeleteNote()
    {
        if (currentPoint == null)
            return;

        if (currentWindow != null)
            return;

        deletePoint = currentPoint;
        //ConfirmWindow.Show(DeleteCallback, "Are you sure you want to delete this note?",
        //    "The note with the following text will be deleted: " + deletePoint.text);
        Undo.RecordObject(currentFlow, "Delete Note");
        currentFlow.RemoveItem(deletePoint);
        deletePoint = null;
        currentPoint = null;
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

public class ForceWindow : EditorWindow
{
    private void OnEnable()
    {
        Focus();
    }

    private void OnLostFocus()
    {
        Focus();
    }
}

public class EditNodeWindow : ForceWindow
{
    private UnityAction<string, Color> onFinish;
    private string text = "Level Note";
    private Color color;

    public static EditorWindow Show(UnityAction<string, Color> finishEvent, string text, Color color)
    {
        var window = GetWindow<EditNodeWindow>();
        window.onFinish = finishEvent;
        window.position = new Rect(window.position.x, window.position.y, 400, 200);
        window.CenterOnMainWin();
        window.text = text;
        window.color = color;

        window.ShowPopup();

        return window;
    }

    private void OnGUI()
    {
        float halfHeight = position.height / 2;
        text = EditorGUI.TextArea(new Rect(5, 5, position.width - 10, halfHeight - 10), text);

        color = EditorGUI.ColorField(new Rect(5, halfHeight, position.width - 10, 20), color);

        if (GUI.Button(new Rect(5, halfHeight + 35, position.width - 10, halfHeight - 10 - 30), "Submit"))
        {
            onFinish(text, color);
            Close();
        }
    }
}

public class ConfirmWindow : ForceWindow
{
    private UnityAction<bool> onFinish;
    private string text;

    public static EditorWindow Show(UnityAction<bool> finishEvent, string titleText, string messageText)
    {
        var window = GetWindow<ConfirmWindow>();
        window.onFinish = finishEvent;
        window.text = messageText;
        window.titleContent = new GUIContent(titleText);
        window.position = new Rect(window.position.x, window.position.y, 400, 200);
        window.CenterOnMainWin();

        window.ShowPopup();

        return window;
    }

    private void OnGUI()
    {
        float halfWidth = position.width / 2;
        float halfHeight = position.height / 2;

        GUIStyle labelstyle = new GUIStyle(GUI.skin.label);
        labelstyle.wordWrap = true;

        GUI.Label(new Rect(5, 5, position.width - 10, halfHeight - 10), text, labelstyle);
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

public class SelectorWindow : ForceWindow
{
    private UnityAction<int> onFinish;
    private string[] texts;

    private float itemHeight = 100;
    private Vector2 scrollPos;

    public static EditorWindow Show(UnityAction<int> finishEvent, string[] texts)
    {
        var window = GetWindow<SelectorWindow>();
        window.onFinish = finishEvent;
        window.position = new Rect(window.position.x, window.position.y, 300, 400);
        window.CenterOnMainWin();
        window.texts = texts;

        window.ShowPopup();

        return window;
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

        GUIStyle buttonstyle = new GUIStyle(GUI.skin.button);
        buttonstyle.wordWrap = true;

        for (int i = 0; i < texts.Length; ++i)
        {
            if (GUI.Button(new Rect(0, (i + 1) * itemHeight, width, itemHeight), texts[i], buttonstyle))
            {
                onFinish(i);
                Close();
                return;
            }
        }

        GUI.EndScrollView();
    }
}

public class RemoveRopesWindow : ForceWindow
{
    private LevelNotes.TextPoint note;
    private LevelNotes parent;

    private float itemHeight = 100;
    private Vector2 scrollPos;

    private int lastCount;

    public static EditorWindow Show(LevelNotes.TextPoint note, LevelNotes parent)
    {
        var window = GetWindow<RemoveRopesWindow>();
        window.note = note;
        window.parent = parent;
        window.lastCount = note.ropes.Count;

        window.position = new Rect(window.position.x, window.position.y, 300, 400);
        window.CenterOnMainWin();

        window.ShowPopup();

        return window;
    }

    private void OnGUI()
    {
        if (lastCount != note.ropes.Count)
        {
            Repaint();
            return;
        }

        float width = position.width - 25;
        scrollPos = GUI.BeginScrollView(new Rect(5, 5, position.width - 10, position.height - 10), scrollPos,
            new Rect(0, 0, width, (note.ropes.Count) * itemHeight), false, true);

        GUIStyle buttonstyle = new GUIStyle(GUI.skin.button);
        buttonstyle.wordWrap = true;

        for (int i = 0; i < note.ropes.Count; ++i)
        {
            if (GUI.Button(new Rect(0, i * itemHeight, width, itemHeight), parent.points[note.ropes[i]].text, buttonstyle))
            {
                note.ropes.RemoveAt(i);
                --lastCount;
                Repaint();
                return;
            }
        }

        GUI.EndScrollView();
    }
}