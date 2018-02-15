using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

public class Item
{
    public GameObject m_goPrefab;
    public GameObject m_goObject;

    public Item(GameObject a_prefab)
    {
        m_goPrefab = a_prefab;
    }
}

public class ItemWindow : EditorWindow
{
    [MenuItem("Window/EW Item Editor %i", false, 1)]
    public static void ShowWindow()
    {
        List<GameObject> objs = new List<GameObject>();
        List<Type> types = new List<Type>();

        if (File.Exists(Application.persistentDataPath + "/ItemWindowPrefs.txt"))
        {
            StreamReader reader = new StreamReader(File.OpenRead(Application.persistentDataPath + "/ItemWindowPrefs.txt"));

            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                string[] split = line.Split(':');
                Type type = Type.GetType(split[0]);

                if (type == typeof(GameObject))
                {
                    if (split[1] != "null")
                    {
                        GameObject obj = AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(split[1]));
                        objs.Add(obj);
                    }
                    else
                    {
                        objs.Add(null);
                    }
                }
                else
                {
                    objs.Add(null);
                }
                types.Add(type);
            }
            reader.Close();
        }
        else
        {
            File.Create(Application.persistentDataPath + "/ItemWindowPrefs.txt");
        }

        //Show existing window instance. If one doesn't exist, make one.
        GetWindow<ItemWindow>().Init(objs, types);
    }



    private List<List<Item>> m_llItems;
    private List<Type> m_lTypes;

    private Vector2 m_v2ListScrollPos;
    private Vector2 m_v2ObjectScrollPos;

    private Item m_iCurrentItem;

    private GUIStyle m_gBackgroundStyle;
    private GUIStyle m_gComponentStyle;

    private GUIStyle m_gItemsStyle;
    private GUIStyle m_gObjectStyle;

    private string m_sCurrentIndices;

    private bool set = false;
    private int m_iWaitTime = 50;

    private float m_fCurrentScrollWidth = 200f;
    private Rect m_rCursorChangeRect;
    private bool m_bResizing = false;

    private bool m_bDoOnce = true;

    public void Init(List<GameObject> a_objects, List<Type> a_types)
    {
        m_llItems = new List<List<Item>>();

        for (int i = 0; i < a_objects.Count; ++i)
        {
            m_llItems.Add(new List<Item>());
            if (a_types[i] == typeof(GameObject))
            {
                Item item = new Item(a_objects[i]);
                m_llItems[i].Add(item);
            }
        }

        m_lTypes = a_types;
        m_v2ListScrollPos = new Vector2();
        m_v2ObjectScrollPos = new Vector2();
        titleContent = new GUIContent("EW Item Editor");

        m_rCursorChangeRect = new Rect(m_fCurrentScrollWidth - 6f, 0, 5f, position.height);

        RecheckItems();
        set = true;
        m_bDoOnce = true;
    }

    void OnGUI()
    {
        if (m_lTypes == null || m_llItems == null || ItemSelector.m_bJustSaved)
        {
            ShowWindow();
            ItemSelector.m_bJustSaved = false;
            return;
        }

        if (m_bDoOnce)
        {
            ReloadStyles();
            m_bDoOnce = false;
        }

        /////////////////////////////////////////
        //The left column of all the items

        GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
        buttonStyle.fontStyle = FontStyle.Bold;
        buttonStyle.fontSize = 14;
        buttonStyle.padding = new RectOffset(10, 0, 0, 0);
        buttonStyle.stretchHeight = false;
        buttonStyle.fixedHeight = 50;
        buttonStyle.alignment = TextAnchor.MiddleCenter;

        GUIStyle headerStyle = new GUIStyle();
        headerStyle.fontStyle = FontStyle.Bold;
        headerStyle.fontSize = 17;
        headerStyle.padding = new RectOffset(10, 0, 0, 0);
        headerStyle.stretchHeight = false;
        headerStyle.fixedHeight = 50;
        headerStyle.alignment = TextAnchor.MiddleCenter;

        //***********************************************************************************************
        //All of the code that changes the list of items MUST go here otherwise there will be errors
        //***********************************************************************************************
        if (Event.current.type == EventType.Layout)
        {
            ReloadStyles();

            --m_iWaitTime;
            if (m_iWaitTime <= 0)
            {
                set = false;
                m_iWaitTime = 50;
                RecheckItems();
                set = true;
            }
        }

        if (!set)
        {
            return;
        }

        ResizeScrollView();

        if (GUILayout.Button("Change Settings"))
        {
            ItemSelector.ShowWindow();
        }

        GUILayout.BeginHorizontal();

        GUILayout.BeginVertical(GUILayout.Width(m_fCurrentScrollWidth));
        m_v2ListScrollPos = GUILayout.BeginScrollView(m_v2ListScrollPos, false, true);

        for (int i = 0; i < m_lTypes.Count; ++i)
        {
            GUILayout.BeginVertical(m_gItemsStyle);

            string[] splits = m_lTypes[i].ToString().Split('.');
            GUILayout.Label(splits[splits.Length - 1], headerStyle);

            int j = 0;
            foreach (Item item in m_llItems[i])
            {
                string name = "null";

                if (item.m_goObject != null)
                {
                    name = item.m_goObject.name;
                }

                if (GUILayout.Button(name, buttonStyle))
                {
                    m_iCurrentItem = item;
                    m_sCurrentIndices = i.ToString() + j.ToString();
                }
                ++j;
            }

            GUILayout.EndVertical();
        }
        GUILayout.EndScrollView();
        GUILayout.EndVertical();


        //GUILayout.Space(1);

        ///////////////////////////////////////////////
        //Draw the inspector for the object

        m_v2ObjectScrollPos = GUILayout.BeginScrollView(m_v2ObjectScrollPos, false, true);

        GUILayout.BeginVertical(m_gBackgroundStyle);
        GUILayout.Space(5);

        if (m_iCurrentItem != null)
        {
            if (m_iCurrentItem.m_goObject != null)
            {
                Undo.RecordObject(m_iCurrentItem.m_goObject, "CurrentItem" + m_sCurrentIndices);

                GUIStyle boxStyle = new GUIStyle(GUI.skin.box);
                boxStyle.padding.bottom = 10;

                GUILayout.BeginVertical(boxStyle);

                //Name & enabled
                {
                    GUILayout.BeginHorizontal();

                    float labelWidth = EditorGUIUtility.fieldWidth;
                    EditorGUIUtility.fieldWidth = 0;

                    GUIStyle activeStyle = new GUIStyle(GUI.skin.toggle);
                    activeStyle.margin = new RectOffset();
                    activeStyle.fixedWidth = 20;

                    m_iCurrentItem.m_goObject.SetActive(GUILayout.Toggle(m_iCurrentItem.m_goObject.activeInHierarchy, "", activeStyle));
                    m_iCurrentItem.m_goObject.name = GUILayout.TextField(m_iCurrentItem.m_goObject.name);

                    EditorGUIUtility.fieldWidth = labelWidth;

                    GUILayout.EndHorizontal();
                }

                GUILayout.Space(5);

                //Tag and layer
                {
                    GUILayout.BeginHorizontal();

                    float labelWidth = EditorGUIUtility.labelWidth;
                    EditorGUIUtility.labelWidth = 0;

                    GUILayout.Label("Tag");
                    m_iCurrentItem.m_goObject.tag = EditorGUILayout.TagField(m_iCurrentItem.m_goObject.tag);

                    GUILayout.Label("Layer");
                    m_iCurrentItem.m_goObject.layer = EditorGUILayout.LayerField(m_iCurrentItem.m_goObject.layer);

                    EditorGUIUtility.labelWidth = labelWidth;

                    GUILayout.EndHorizontal();
                }

                GUILayout.Space(5);

                if (m_iCurrentItem.m_goPrefab != null)
                {
                    GUILayout.BeginHorizontal();
                    float minWidth;
                    float maxWidth;

                    GUIStyle.none.CalcMinMaxWidth(new GUIContent("Prefab"), out minWidth, out maxWidth);

                    GUILayout.Label("Prefab", GUILayout.Width(minWidth + 5));

                    if (GUILayout.Button("Select"))
                    {
                        string path = AssetDatabase.GetAssetPath(m_iCurrentItem.m_goPrefab);
                        Selection.activeObject = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                    }

                    if (GUILayout.Button("Revert"))
                    {
                        PrefabUtility.RevertPrefabInstance(m_iCurrentItem.m_goObject);
                    }

                    if (GUILayout.Button("Apply"))
                    {
                        m_iCurrentItem.m_goPrefab = PrefabUtility.ReplacePrefab(m_iCurrentItem.m_goObject, m_iCurrentItem.m_goPrefab, ReplacePrefabOptions.ConnectToPrefab);
                    }

                    GUILayout.EndHorizontal();

                    GUILayout.Space(5);
                }

                if (GUILayout.Button("Select In Hierarchy"))
                {
                    Selection.activeGameObject = m_iCurrentItem.m_goObject;
                }

                GUILayout.EndVertical();

                GUIStyle boldStyle = new GUIStyle(GUI.skin.label);
                boldStyle.fontStyle = FontStyle.Bold;

                GUIStyle toggleStyle = new GUIStyle(GUI.skin.toggle);
                toggleStyle.fontStyle = boldStyle.fontStyle;

                Component[] comps = m_iCurrentItem.m_goObject.GetComponents<Component>();


                for (int i = 0; i < comps.Length; ++i)
                {
                    GUILayout.BeginVertical(m_gComponentStyle);
                    string[] splits = comps[i].GetType().ToString().Split('.');

                    if (comps[i].GetType().GetProperty("enabled") != null)
                    {
                        object[] setEnabled = new object[1];
                        bool? valerie = comps[i].GetType().InvokeMember("enabled", BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty, Type.DefaultBinder, comps[i], null) as bool?;
                        if (valerie != null)
                        {
                            setEnabled[0] = GUILayout.Toggle(valerie.Value, splits[splits.Length - 1], toggleStyle);
                            comps[i].GetType().InvokeMember("enabled", BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty, Type.DefaultBinder, comps[i], setEnabled);
                        }
                    }
                    else
                    {
                        GUILayout.Label(splits[splits.Length - 1], boldStyle);
                    }

                    GUILayout.Space(5);

                    Editor editor = Editor.CreateEditor(comps[i]);
                    editor.OnInspectorGUI();
                    GUILayout.Space(20);
                    GUILayout.EndVertical();
                }
            }
            else
            {
                GUILayout.Label("The object either doesn't exist or the tags do not match");
                if (m_iCurrentItem.m_goPrefab != null)
                {
                    if (GUILayout.Button("Create Instance of Prefab"))
                    {
                        m_iCurrentItem.m_goObject = PrefabUtility.InstantiatePrefab(m_iCurrentItem.m_goPrefab) as GameObject;
                    }
                }
            }
        }
        else
        {
            GUILayout.Label("No item selected");
        }

        GUILayout.EndVertical();
        GUILayout.EndScrollView();

        GUILayout.EndHorizontal();

    }

    private Texture2D MakeTex(int width, int height, Color col)
    {
        Color[] pix = new Color[width * height];

        for (int i = 0; i < pix.Length; i++)
            pix[i] = col;

        Texture2D result = new Texture2D(width, height);
        result.SetPixels(pix);
        result.Apply();

        return result;
    }

    private bool ListContains(int a_index, GameObject a_childObj)
    {
        for (int i = 0; i < m_llItems[a_index].Count; ++i)
        {
            if (m_llItems[a_index][i].m_goObject == a_childObj)
            {
                return true;
            }
        }
        return false;
    }

    private void ResizeScrollView()
    {
        //GUI.DrawTexture(m_rCursorChangeRect, EditorGUIUtility.whiteTexture);
        EditorGUIUtility.AddCursorRect(m_rCursorChangeRect, MouseCursor.ResizeHorizontal);
        if (Event.current.type == EventType.MouseDown && m_rCursorChangeRect.Contains(Event.current.mousePosition))
        {
            m_bResizing = true;
        }
        if (m_bResizing)
        {
            //EditorGUIUtility.AddCursorRect(new Rect(0, 0, position.width, position.height), MouseCursor.ResizeHorizontal);

            if(Event.current.mousePosition.x > 40)
                m_fCurrentScrollWidth = Event.current.mousePosition.x;
        }
        m_rCursorChangeRect.Set(m_fCurrentScrollWidth - 6f, 0, 5f, position.height);
        if (Event.current.type == EventType.MouseUp)
            m_bResizing = false;
    }

    private void RecheckItems()
    {
        for (int i = 0; i < m_llItems.Count; ++i)
        {
            if (m_lTypes[i] == typeof(GameObject))
            {
                if (m_llItems[i][0].m_goPrefab != null)
                {
                    GameObject[] foundObjs = FindObjectsOfType<GameObject>();

                    foreach (GameObject child in foundObjs)
                    {
                        if (PrefabUtility.GetPrefabType(child) != PrefabType.None)
                        {
                            if (PrefabUtility.GetPrefabParent(child) == m_llItems[i][0].m_goPrefab)
                            {
                                m_llItems[i][0].m_goObject = child;
                                break;
                            }
                        }
                    }
                }
            }
            else
            {
                UnityEngine.Object[] foundObjs = FindObjectsOfType(m_lTypes[i]);

                if (foundObjs.Length != m_llItems[i].Count)
                {
                    m_llItems[i].Clear();
                    foreach (UnityEngine.Object childObj in foundObjs)
                    {
                        Component child = childObj as Component;
                        Item item = new Item(null);
                        if (PrefabUtility.GetPrefabType(child) != PrefabType.None)
                        {
                            item.m_goPrefab = PrefabUtility.GetPrefabParent(child.gameObject) as GameObject;
                        }

                        item.m_goObject = child.gameObject;

                        if (!ListContains(i, item.m_goObject))
                        {
                            m_llItems[i].Add(item);
                        }
                    }
                }
            }
        }

        if (m_iCurrentItem != null)
        {
            if (m_iCurrentItem.m_goObject != null)
            {
                if (PrefabUtility.GetPrefabType(m_iCurrentItem.m_goObject) != PrefabType.None)
                {
                    m_iCurrentItem.m_goPrefab = PrefabUtility.GetPrefabParent(m_iCurrentItem.m_goObject) as GameObject;
                }
                else
                {
                    m_iCurrentItem.m_goPrefab = null;
                }
            }
        }
    }

    void ReloadStyles()
    {
        if (m_gBackgroundStyle != null)
        {
            DestroyImmediate(m_gBackgroundStyle.normal.background, true);
            DestroyImmediate(m_gComponentStyle.normal.background, true);
            DestroyImmediate(m_gItemsStyle.normal.background, true);
            DestroyImmediate(m_gObjectStyle.normal.background, true);
        }

        m_gBackgroundStyle = new GUIStyle(GUI.skin.box);
        m_gBackgroundStyle.margin = new RectOffset(5, 5, 5, 5);
        m_gBackgroundStyle.normal.background = EWTools.MakeTexture(1, 1, new Color(0.3f, 0.3f, 0.3f));

        m_gComponentStyle = new GUIStyle(GUI.skin.box);
        m_gComponentStyle.margin = new RectOffset(5, 5, 5, 5);
        m_gComponentStyle.normal.background = EWTools.MakeTexture(1, 1, new Color(0.7f, 0.7f, 0.7f));

        m_gItemsStyle = new GUIStyle(GUI.skin.box);
        m_gItemsStyle.margin = new RectOffset(5, 5, 5, 5);
        m_gItemsStyle.normal.background = EWTools.MakeTexture(1, 1, Color.blue);

        m_gObjectStyle = new GUIStyle(GUI.skin.box);
        m_gObjectStyle.margin = new RectOffset(5, 5, 5, 5);
        m_gObjectStyle.normal.background = EWTools.MakeTexture(1, 1, Color.blue);
    }
}
