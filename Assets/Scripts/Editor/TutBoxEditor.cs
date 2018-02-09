using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Reflection;
using System.Collections.Generic;

[CustomEditor(typeof(TutBox))]
public class TutBoxEditor : Editor
{
    private int m_iSelectedObj;
    private int m_iSelectedName;
    private TutBox box;

    public override void OnInspectorGUI()
    {
        box = (TutBox)target;

        if (box.m_sKeyText == null)
        {
            box.m_sKeyText = "";
        }

        EditorGUI.BeginChangeCheck();
        string text = EditorGUILayout.TextField("Keyboard Text:", box.m_sKeyText);

        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(box, "Box edit");
            box.m_sKeyText = text;
        }

        EditorGUI.BeginChangeCheck();
        text = EditorGUILayout.TextField("Controller Text:", box.m_sGPText);

        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(box, "Box edit");
            box.m_sGPText = text;
        }

        EditorGUI.BeginChangeCheck();
        bool normal = EditorGUILayout.Toggle("Animal trigger only", box.m_bAnimalOnly);

        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(box, "Box edit");
            box.m_bAnimalOnly = normal;
        }

        if (box.m_atItems == null)
        {
            box.m_atItems = new TutItems[0];
        }

        if (GUILayout.Button("Add item"))
        {
            Undo.RecordObject(box, "Box edit");
            box.m_atItems = box.m_atItems.Add(new TutItems());
        }

        if (box.m_atItems.Length > 0)
        {
            for (int i = 0; i < box.m_atItems.Length; ++i)
            {
                EditorGUILayout.BeginVertical();

                EditorGUILayout.BeginHorizontal();
                EditorGUI.BeginChangeCheck();
                box.m_atItems[i].gameObject = (GameObject)EditorGUILayout.ObjectField(box.m_atItems[i].gameObject, typeof(GameObject), true);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(box, "Box edit");
                    box.m_atItems[i].type = null;
                    box.m_atItems[i].typeIndex = 0;                
                }
                if (GUILayout.Button("X", GUILayout.Width(20)))
                {
                    Undo.RecordObject(box, "Box edit");
                    box.m_atItems = box.m_atItems.RemoveAt(i);
                    --i;
                    continue;
                }
                EditorGUILayout.EndHorizontal();

                if (box.m_atItems[i].gameObject == null)
                    continue;

                Component[] comps = box.m_atItems[i].gameObject.GetComponents<Component>();
                EditorGUI.BeginChangeCheck();
                box.m_atItems[i].typeIndex = EditorGUILayout.Popup(box.m_atItems[i].typeIndex, comps.ToStringArray());
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(box, "Box edit");
                    box.m_atItems[i].type = comps[box.m_atItems[i].typeIndex];
                    box.m_atItems[i].values = new TutItemObject[0];
                    box.m_atItems[i].names = new string[0];
                }

                if (box.m_atItems[i].type != null)
                {
                    if (GUILayout.Button("Add parameter"))
                    {
                        Undo.RecordObject(box, "Box edit");
                        box.m_atItems[i].values = box.m_atItems[i].values.Add(new TutItemObject());
                        box.m_atItems[i].names = box.m_atItems[i].names.Add("");
                    }

                    for (int j = 0; j < box.m_atItems[i].names.Length; ++j)
                    {
                        EditorGUILayout.BeginHorizontal();
                        MemberInfo[] infos = box.m_atItems[i].type.GetType().GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                        if (GUILayout.Button(box.m_atItems[i].names[j]))
                        {
                            Undo.RecordObject(box, "Box edit");
                            m_iSelectedObj = i;
                            m_iSelectedName = j;
                            EditorWindow.GetWindow<MemberSelector>().Init(infos, Done);
                        }
                        if (GUILayout.Button("X", GUILayout.Width(20)))
                        {
                            Undo.RecordObject(box, "Box edit");
                            box.m_atItems[i].names = box.m_atItems[i].names.RemoveAt(j);
                            box.m_atItems[i].values = box.m_atItems[i].values.RemoveAt(j);
                            --j;
                            continue;
                        }
                        EditorGUILayout.EndHorizontal();

                        if (box.m_atItems[i].values[j].type != null && box.m_atItems[i].values[j].type != "")
                            RenderValue(box.m_atItems[i].values[j]);
                    }
                }
                EditorGUILayout.Space();
                EditorGUILayout.EndVertical();
            }
        }
    }

    public void Done(MemberInfo[] a_infos, int a_index)
    {
        Undo.RecordObject(box, "Box edit");
        box.m_atItems[m_iSelectedObj].values[m_iSelectedName].type = a_infos[a_index].UnderLyingType().AssemblyQualifiedName;
        box.m_atItems[m_iSelectedObj].names[m_iSelectedName] = a_infos[a_index].Name;
        m_iSelectedName = -1;
        m_iSelectedObj = -1;
    }

    public void RenderValue(TutItemObject a_item)
    {
        EditorGUI.BeginChangeCheck();
        System.Type t = ReflectionUtils.GetType(a_item.type);
        float f = 0;
        int i = 0;
        bool b = false;
        string s = "";
        Vector2 v2 = Vector2.zero;
        Vector3 v3 = Vector3.zero;
        Object o = null;

        if (t == typeof(float))
        {
            f = EditorGUILayout.FloatField(a_item.f);
        }
        else if (t == typeof(int))
        {
            i = EditorGUILayout.IntField(a_item.i);
        }
        else if (t == typeof(bool))
        {
            b = EditorGUILayout.Toggle(a_item.b);
        }
        else if (t == typeof(string))
        {
            s = EditorGUILayout.TextField(a_item.s);
        }
        else if (t == typeof(Vector2))
        {
            v2 = EditorGUILayout.Vector2Field("", a_item.v2);
        }
        else if (t == typeof(Vector3))
        {
            v3 = EditorGUILayout.Vector3Field("", a_item.v3);
        }
        else
        {
            o = EditorGUILayout.ObjectField(a_item.o, t, true);
        }

        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(box, "Box edit");

            if (t == typeof(float))
            {
                a_item.f = f;
            }
            else if (t == typeof(int))
            {
                a_item.i = i;
            }
            else if (t == typeof(bool))
            {
                a_item.b = b;
            }
            else if (t == typeof(string))
            {
                a_item.s = s;
            }
            else if (t == typeof(Vector2))
            {
                a_item.v2 = v2;
            }
            else if (t == typeof(Vector3))
            {
                a_item.v3 = v3;
            }
            else
            {
                a_item.o = o;
            }
        }
    }
}

public class MemberSelector : EditorWindow
{
    public delegate void EnumDel(MemberInfo[] a_infos, int a_index);

    public void Init(MemberInfo[] a_names, EnumDel a_del)
    {
        titleContent = new GUIContent("Enum Selector");
        m_aAllNames = a_names.MemberToStringArray();
        m_aSearchNames = new List<string>(m_aAllNames);
        m_search = "";
        m_searchPrev = "";
        m_eFunc = a_del;
        m_mInfos = a_names.RemoveMethods();
    }

    private string m_search;
    private string m_searchPrev;
    private string[] m_aAllNames;
    private List<string> m_aSearchNames;
    private Vector2 m_v2Scroll;
    private bool m_bPerfect;
    private EnumDel m_eFunc;
    private MemberInfo[] m_mInfos;

    void OnGUI()
    {
        GUIStyle textField = new GUIStyle(GUI.skin.textField);
        textField.fontSize = 15;
        textField.alignment = TextAnchor.MiddleLeft;
        GUIStyle button = new GUIStyle(GUI.skin.button);
        button.fontSize = 15;

        m_search = GUI.TextField(new Rect(5, 5, position.width - 25, 30), m_search, textField);

        m_v2Scroll = GUI.BeginScrollView(new Rect(5, 40, position.width - 10, position.height - 45), m_v2Scroll, new Rect(0, 0, position.width - 25, m_aSearchNames.Count * 50));
        for (int i = 0; i < m_aSearchNames.Count; ++i)
        {
            if (GUI.Button(new Rect(0, i * 50, position.width - 25, 50), m_aSearchNames[i], button))
            {
                m_eFunc(m_mInfos, m_aAllNames.IndexOf(m_aSearchNames[i]));
                Close();
            }
        }
        GUI.EndScrollView();

        if (Event.current.type == EventType.KeyUp)
        {
            if (Event.current.keyCode == KeyCode.Return)
            {
                m_bPerfect = true;
            }
        }
    }

    void Update()
    {
        if (m_searchPrev != m_search || m_bPerfect)
        {
            if (m_search != "")
            {
                m_aSearchNames.Clear();
                for (int i = 0; i < m_aAllNames.Length; ++i)
                {
                    if (!m_bPerfect)
                    {
                        //Compare the string (case insensitive)
                        if (m_aAllNames[i].IndexOf(m_search, System.StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            m_aSearchNames.Add(m_aAllNames[i]);
                        }
                    }
                    else
                    {
                        if (System.StringComparer.OrdinalIgnoreCase.Compare(m_aAllNames[i], m_search) == 0)
                        {
                            m_aSearchNames.Add(m_aAllNames[i]);
                            break;
                        }
                    }
                }
            }
            else
            {
                m_aSearchNames = new List<string>(m_aAllNames);
            }

            m_bPerfect = false;
            m_searchPrev = m_search;
            Repaint();
        }
    }
}