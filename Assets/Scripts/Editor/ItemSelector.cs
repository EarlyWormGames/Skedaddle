using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections.Generic;
using System.Reflection;

/// <summary>
/// item selector Editor Window
/// a popup that allows you to change and modify items 
/// </summary>
public class ItemSelector : EditorWindow
{
    public static bool m_bJustSaved = false;

    public static void ShowWindow()
    {
        //Show existing window instance. If one doesn't exist, make one.
        GetWindow<ItemSelector>().Init();
    }



    private List<GameObject>    m_lGObjects;
    private List<Type>          m_lTypes;

    private List<string>        m_lEWTypes;
    private int                 m_iEWSelType;

    private List<string>        m_lOtherTypes;
    private int                 m_iOtherSelType;

    private List<GameObject>    m_lGOToAdd;
    private List<Type>          m_lTypesToAdd;
    private List<int>           m_lIndexToRemove;

    public void Init()
    {
        m_lGObjects = new List<GameObject>();
        m_lTypes = new List<Type>();
        m_lEWTypes = new List<string>();
        m_lOtherTypes = new List<string>();

        m_lGOToAdd = new List<GameObject>();
        m_lTypesToAdd = new List<Type>();
        m_lIndexToRemove = new List<int>();

        titleContent = new GUIContent("EW Settings");

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
                    m_lGObjects.Add(obj);
                }
                else
                {
                    m_lGObjects.Add(null);
                }
            }
            else
            {
                m_lGObjects.Add(null);
            }
            m_lTypes.Add(type);
        }
        reader.Close();

        FindDerivedTypes(Assembly.GetAssembly(typeof(EWApplication)), typeof(Component));
        FindDerivedTypes(Assembly.GetAssembly(typeof(Component)), typeof(Component));
        m_lEWTypes.Sort();
        m_lOtherTypes.Sort();
    }

    void OnGUI()
    {
        if (m_lEWTypes == null)
        {
            Init();
            return;
        }

        if (Event.current.type == EventType.Layout)
        {
            if (m_lIndexToRemove.Count > 0)
            {
                for (int i = 0; i < m_lIndexToRemove.Count; ++i)
                {
                    m_lGObjects.RemoveAt(m_lIndexToRemove[i]);
                    m_lTypes.RemoveAt(m_lIndexToRemove[i]);
                }
            }

            if (m_lGOToAdd.Count > 0)
            {
                for (int i = 0; i < m_lGOToAdd.Count; ++i)
                {
                    m_lGObjects.Add(m_lGOToAdd[i]);
                    m_lTypes.Add(m_lTypesToAdd[i]);
                }
            }

            m_lTypesToAdd.Clear();
            m_lGOToAdd.Clear();
            m_lIndexToRemove.Clear();
        }


        Rect pos = new Rect();

        EditorGUILayout.BeginHorizontal();

        {
            //Left Side goes here
            EditorGUILayout.BeginVertical();
            if (m_lGObjects.Count > 0)
            {
                float minXWidth = 0f;
                float maxXWidth = 0f;
                GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
                buttonStyle.CalcMinMaxWidth(new GUIContent("X"), out minXWidth, out maxXWidth);
                for (int i = 0; i < m_lGObjects.Count; ++i)
                {
                    EditorGUILayout.BeginHorizontal();
                    if (m_lTypes[i] == typeof(GameObject))
                    {
                        pos = EditorGUILayout.GetControlRect();
                        m_lGObjects[i] = EditorGUI.ObjectField(pos, m_lGObjects[i], typeof(GameObject), false) as GameObject;
                    }
                    else
                    {
                        GUILayout.Label(m_lTypes[i].ToString());
                    }

                    if (GUILayout.Button("X", GUILayout.Width(minXWidth)))
                    {
                        if (!m_lIndexToRemove.Contains(i))
                        {
                            m_lIndexToRemove.Add(i);
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }
            else
            {
                GUILayout.Label("No items found!");
            }
            EditorGUILayout.EndVertical();
            //End Left stuff
        }

        {
            //Right side
            EditorGUILayout.BeginVertical();

            if (GUILayout.Button("Add Prefab"))
            {
                //Add a new prefab
                m_lTypesToAdd.Add(typeof(GameObject));
                m_lGOToAdd.Add(null);
            }
            

            EditorGUILayout.BeginHorizontal();

            int selectedType = 0;
            GUILayout.Label("Add EW Type: ");
            GUILayout.Space(11f);
            pos = EditorGUILayout.GetControlRect();
            selectedType = EditorGUI.Popup(pos, m_iEWSelType, m_lEWTypes.ToArray());

            if (selectedType != m_iEWSelType)
            {
                //Add item
                m_iEWSelType = selectedType;
                m_lTypesToAdd.Add(Type.GetType(m_lEWTypes[selectedType] + "," + Assembly.GetAssembly(typeof(EWApplication)).ToString()));
                m_lGOToAdd.Add(null);
            }

            EditorGUILayout.EndHorizontal();


            EditorGUILayout.BeginHorizontal();

            selectedType = 0;
            GUILayout.Label("Add Unity Type: ");
            pos = EditorGUILayout.GetControlRect();
            selectedType = EditorGUI.Popup(pos, m_iOtherSelType, m_lOtherTypes.ToArray());

            if (selectedType != m_iOtherSelType)
            {
                //Add item
                m_iOtherSelType = selectedType;
                Type addType = Type.GetType("UnityEngine." + m_lOtherTypes[selectedType] + "," + Assembly.GetAssembly(typeof(Component)).ToString());
                if (!m_lTypes.Contains(addType))
                {
                    m_lTypesToAdd.Add(addType);
                    m_lGOToAdd.Add(null);
                }
            }

            EditorGUILayout.EndHorizontal();

            if (GUILayout.Button("Remove Last Item"))
            {
                if (m_lGObjects.Count > 0)
                {
                    if (!m_lIndexToRemove.Contains(m_lGObjects.Count - 1))
                    {
                        m_lIndexToRemove.Add(m_lGObjects.Count - 1);
                    }
                }
            }

            EditorGUILayout.EndVertical();
        }

        if (GUILayout.Button("Save"))
        {
            //Save stuff
            Save();
        }
    }

    public void FindDerivedTypes(Assembly a_assembly, Type a_baseType)
    {
        Type[] derivedTypes = a_assembly.GetTypes();
       
        for (int i = 0; i < derivedTypes.Length; ++i)
        {
            if (derivedTypes[i] != a_baseType && a_baseType.IsAssignableFrom(derivedTypes[i]))
            {
                string typeName = derivedTypes[i].ToString();
                if (typeName.Contains("UnityEngine."))
                {
                    //Add to unity engine types
                    if (!typeName.Contains("MonoBehaviour"))
                    {
                        if (!m_lOtherTypes.Contains(typeName))
                            m_lOtherTypes.Add(typeName.Replace("UnityEngine.", ""));
                    }
                }
                else
                {
                    if (!m_lEWTypes.Contains(typeName))
                        m_lEWTypes.Add(typeName);
                }
            }
        }
    }

    void Save()
    {
        FileStream stream = new FileStream(Application.persistentDataPath + "/ItemWindowPrefs.txt", FileMode.Create);
        StreamWriter writer = new StreamWriter(stream);
        for (int i = 0; i < m_lGObjects.Count; ++i)
        {
            string typeName = m_lTypes[i].ToString();
            Assembly objAssembly = Assembly.GetAssembly(m_lTypes[i]);
            typeName += "," + objAssembly.ToString();
            typeName += ":";

            if (m_lTypes[i] == typeof(GameObject) && m_lGObjects[i] != null)
            {
                typeName += AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(m_lGObjects[i]));
            }
            else
            {
                typeName += "null";
            }
            writer.WriteLine(typeName);
        }
        writer.Close();

        m_bJustSaved = true;
    }
}