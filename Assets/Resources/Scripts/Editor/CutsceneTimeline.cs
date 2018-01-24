using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

public class TimelineObject
{
    public Vector2 m_v2TimePos;
    public int m_iYIndex = 0;
    public GUIStyle m_gStyle;
    public GameObject m_goObject;

    public bool m_bResizeLeft = false;
    public bool m_bResizeRight = false;

    public bool m_bMoving = false;



    private Vector2 m_v2MouseStartDist;

    public void Update()
    {
        Rect left = new Rect(m_v2TimePos.x + CutsceneTimeline.LeftPadding, (m_iYIndex * 100) + 40, 10, 100f);
        Rect right = new Rect(m_v2TimePos.x + CutsceneTimeline.LeftPadding + (m_v2TimePos.y - m_v2TimePos.x), (m_iYIndex * 100) + 40, 10, 100f);
        Rect centre = new Rect(m_v2TimePos.x + CutsceneTimeline.LeftPadding + 10, (m_iYIndex * 100) + 40, (m_v2TimePos.y - m_v2TimePos.x) - 10, 100f);

        //GUI.DrawTexture(m_rCursorChangeRect, EditorGUIUtility.whiteTexture);
        EditorGUIUtility.AddCursorRect(left, MouseCursor.ResizeHorizontal);
        EditorGUIUtility.AddCursorRect(right, MouseCursor.ResizeHorizontal);
        EditorGUIUtility.AddCursorRect(centre, MouseCursor.MoveArrow);

        if (Event.current.type == EventType.MouseDown && left.Contains(Event.current.mousePosition))
        {
            m_bResizeLeft = true;
        }
        else if (Event.current.type == EventType.MouseDown && right.Contains(Event.current.mousePosition))
        {
            m_bResizeRight = true;
        }
        else if (Event.current.type == EventType.MouseDown && centre.Contains(Event.current.mousePosition))
        {
            m_bMoving = true;
            m_v2MouseStartDist = new Vector2(Event.current.mousePosition.x - m_v2TimePos.x, Event.current.mousePosition.y);
        }

        if (m_bResizeLeft)
        {
            if (Event.current.mousePosition.x - CutsceneTimeline.LeftPadding >= 0)
            {
                m_v2TimePos.x = Event.current.mousePosition.x - CutsceneTimeline.LeftPadding;
            }
        }
        else if (m_bResizeRight)
        {
            if (Event.current.mousePosition.x - CutsceneTimeline.LeftPadding <= CutsceneTimeline.TotalTime * 100f)
            {
                m_v2TimePos.y = Event.current.mousePosition.x - CutsceneTimeline.LeftPadding;
            }
        }
        else if (m_bMoving)
        {
            float width = m_v2TimePos.y - m_v2TimePos.x;
            m_v2TimePos.x = Event.current.mousePosition.x - m_v2MouseStartDist.x;
            m_v2TimePos.y = m_v2TimePos.x + width;
        }

        CheckUp();
    }

    public void CheckUp()
    {
        if (Event.current != null)
        {
            if (Event.current.type == EventType.MouseUp)
            {
                m_bResizeLeft = false;
                m_bResizeRight = false;
                m_bMoving = false;
            }
        }
    }

    public void ForceUp()
    {
        m_bResizeLeft = false;
        m_bResizeRight = false;
        m_bMoving = false;
    }
}

public class CutsceneTimeline : EditorWindow
{
    [MenuItem("Window/EW Cutscene Timeline", false, 2)]
    public static void ShowWindow()
    {
        GetWindow<CutsceneTimeline>().Init();
    }


    //==================================
    //          Public Vars
    //==================================
    public static float LeftPadding = 15f;

    public static float TotalTime = 10f;
    public bool m_bSceneEditEnabled = false;

    //==================================
    //          Internal Vars
    //==================================

    //==================================
    //          Protected Vars
    //================================== 

    //==================================
    //          Private Vars
    //==================================
    private int                     m_iWaitTime = 0;

    private Vector2                 m_v2TimelineScrollPos;

    private Dictionary<GameObject, TimelineObject>    m_dObjects;
    private Dictionary<GameObject, TimelineObject>    m_dObjectsToAdd;

    void Init()
    {
        titleContent = new GUIContent("EW Cutscene");

        m_dObjects = new Dictionary<GameObject, TimelineObject>();
        m_dObjectsToAdd = new Dictionary<GameObject, TimelineObject>();
    }

    void Update()
    {
        Repaint();
    }

    void OnGUI()
    {
        if (m_dObjects == null)
        {
            Init();
            return;
        }

        if (Event.current.type == EventType.Layout)
        {
            --m_iWaitTime;

            if (m_dObjectsToAdd.Count > 0)
            {
                foreach (KeyValuePair<GameObject, TimelineObject> child in m_dObjectsToAdd)
                {
                    if (!m_dObjects.ContainsKey(child.Key))
                    {
                        m_dObjects.Add(child.Key, child.Value);
                    }
                }
            }
            m_dObjectsToAdd.Clear();

            //List<KeyValuePair<GameObject, TimelineObject>> myList = m_dObjects.ToList();
            //
            //myList.Sort((firstPair, nextPair) =>
            //{
            //    return firstPair.Value.m_iYIndex.CompareTo(nextPair.Value.m_iYIndex);
            //}
            //);

            if (m_iWaitTime <= 0)
            {
                m_dObjects.OrderBy(x => x.Value.m_iYIndex);
                ReloadStyles();
                m_iWaitTime = 10000;
            }
        }



        GUIStyle bottomBorderStyle = new GUIStyle(GUI.skin.box);
        bottomBorderStyle.fixedHeight = 100;
        bottomBorderStyle.fixedWidth = position.width;

        GUIStyle enabledStyle = new GUIStyle();
        enabledStyle.normal.textColor = m_bSceneEditEnabled ? Color.red : Color.black;
        enabledStyle.fontStyle = FontStyle.Bold;
        enabledStyle.alignment = TextAnchor.LowerLeft;
        enabledStyle.fixedHeight = 17;

        //Begin main layout
        GUILayout.BeginVertical();

        {
            //Begin top section layout
            GUILayout.BeginVertical(bottomBorderStyle);
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Scene Edit Mode:", GUILayout.MaxWidth(120)))
            {
                m_bSceneEditEnabled = !m_bSceneEditEnabled;
            }

            GUILayout.Label(m_bSceneEditEnabled ? "ENABLED" : "DISABLED", enabledStyle);

            GUILayout.EndHorizontal();
            GUILayout.Space(10);

            if (GUILayout.Button("Add Scene End", GUILayout.MaxWidth(120)))
            {
                //Add scene end
            }

            GUILayout.EndVertical();
            //End top section layout
        }

        GUIStyle timeStyle = new GUIStyle(GUI.skin.box);
        timeStyle.padding = new RectOffset();
        timeStyle.fixedHeight = 30f;
        {
            //Begin bottom section (the timeline)
            GUILayout.BeginVertical(GUILayout.Width(position.width));
            m_v2TimelineScrollPos = GUILayout.BeginScrollView(m_v2TimelineScrollPos, true, true);

            EventType eventType = Event.current.type;
            if ((eventType == EventType.DragUpdated || eventType == EventType.DragPerform))// && dropArea.Contains(Event.current.mousePosition))
            {
                // Show a copy icon on the drag
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                if (eventType == EventType.DragPerform)
                {
                    //Check drop
                    if (DragAndDrop.objectReferences.Length > 0)
                    {
                        if ((DragAndDrop.objectReferences[0] as GameObject).GetComponent<CutsceneObject>() != null)
                        {
                            TimelineObject obj = new TimelineObject();
                            obj.m_goObject = DragAndDrop.objectReferences[0] as GameObject;
                            obj.m_iYIndex = m_dObjects.Count;
                            obj.m_v2TimePos = new Vector2(Event.current.mousePosition.x, Event.current.mousePosition.x + 100);
                            obj.m_gStyle = new GUIStyle(GUI.skin.box);
                            obj.m_gStyle.alignment = TextAnchor.MiddleCenter;
                            obj.m_gStyle.normal.background = EWTools.MakeTexture(1, 1, new Color(1, 0, 0));
                            m_dObjectsToAdd.Add(obj.m_goObject, obj);
                        }
                    }
                    DragAndDrop.AcceptDrag();
                }
                Event.current.Use();
            }

            GUILayout.BeginHorizontal();
            GUILayout.Space(15);
            GUILayout.BeginHorizontal(timeStyle);
            GUILayout.Space(TotalTime * 100f);
            GUILayout.EndHorizontal();
            GUILayout.EndHorizontal();

            GUILayout.BeginVertical();
            foreach (KeyValuePair<GameObject, TimelineObject> child in m_dObjects)
            {
                if (child.Value.m_bResizeLeft)
                {
                    if (child.Value.m_v2TimePos.x > child.Value.m_v2TimePos.y - 30f)
                    {
                        child.Value.m_v2TimePos.y = child.Value.m_v2TimePos.x + 30f;
                    }
                }
                else if (child.Value.m_bResizeRight)
                {
                    if (child.Value.m_v2TimePos.y < child.Value.m_v2TimePos.x + 30f)
                    {
                        child.Value.m_v2TimePos.x = child.Value.m_v2TimePos.y - 30f;
                    }
                }
                else if (child.Value.m_v2TimePos.y - child.Value.m_v2TimePos.x < 30f)
                {
                    child.Value.m_v2TimePos.y = child.Value.m_v2TimePos.x + 30f;
                }

                float width = child.Value.m_v2TimePos.y - child.Value.m_v2TimePos.x;

                if (child.Value.m_v2TimePos.x < 0)
                {
                    child.Value.m_v2TimePos.x = 0f;
                    if (child.Value.m_v2TimePos.x + width > TotalTime * 100f)
                    {
                        width = TotalTime * 100f;
                    }
                    child.Value.m_v2TimePos.y = child.Value.m_v2TimePos.x + width;
                }
                else if (child.Value.m_v2TimePos.y > TotalTime * 100f)
                {
                    child.Value.m_v2TimePos.y = TotalTime * 100f;
                    if (child.Value.m_v2TimePos.y - width < 0)
                    {
                        width = TotalTime * 100f;
                    }
                    child.Value.m_v2TimePos.x = child.Value.m_v2TimePos.y - width;
                }

                GUI.Label(new Rect(child.Value.m_v2TimePos.x + LeftPadding, (child.Value.m_iYIndex * 100) + 40, (width), 100f), child.Value.m_goObject.name, child.Value.m_gStyle);
                if (mouseOverWindow == this)
                {
                    child.Value.Update();
                }
            }
            GUILayout.EndVertical();

            GUILayout.EndScrollView();
            GUILayout.EndVertical();
            //End bottom section
        }


        GUILayout.EndVertical();
        //End main layout
    }

    void ReloadStyles()
    {
        if (m_dObjects.Count > 0)
        {
            foreach (KeyValuePair<GameObject, TimelineObject> child in m_dObjects)
            {
                Color col = child.Value.m_gStyle.normal.background.GetPixel(0, 0);
                DestroyImmediate(child.Value.m_gStyle.normal.background, true);
                child.Value.m_gStyle.normal.background = EWTools.MakeTexture(1, 1, col);
            }
        }
    }
}
