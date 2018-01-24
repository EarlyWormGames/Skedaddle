using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

public class KeybindingWindow : EditorWindow
{
    [MenuItem("Early Worm/Keybinding")]
    public static void ShowWindow()
    {
        GetWindow<KeybindingWindow>().titleContent = new GUIContent("Keybinding");
    }

    private Keybinding m_kObject;
    private int m_iSelectedIndex;
    private int m_iSelectedKey;
    private Vector2 m_v2NamesPos;
    private Vector2 m_v2CodePos;

    void OnGUI()
    {
        if (m_kObject == null)
        {
            m_kObject = AssetDatabase.LoadAssetAtPath<Keybinding>("Assets/Resources/Keybinding.asset");

            if (m_kObject == null)
            {
                m_kObject = CreateInstance<Keybinding>();

                if (!AssetDatabase.IsValidFolder("Assets/Resources"))
                    AssetDatabase.CreateFolder("Assets", "Resources");

                AssetDatabase.CreateAsset(m_kObject, "Assets/Resources/Keybinding.asset");
                AssetDatabase.SaveAssets();
                AssetDatabase.ImportAsset("Assets/Resources/Keybinding.asset");
            }
        }

        GUIStyle centre = new GUIStyle(GUI.skin.label);
        centre.alignment = TextAnchor.MiddleCenter;
        centre.fontSize = 18;

        string[] names = m_kObject.GetKeyNames();

        m_v2NamesPos = GUI.BeginScrollView(new Rect(5, 5, 200, position.height - 35), m_v2NamesPos, new Rect(0, 0, 100, names.Length * 20));
        m_iSelectedIndex = GUI.SelectionGrid(new Rect(0, 0, 180, names.Length * 20), m_iSelectedIndex, names, 1);
        GUI.EndScrollView();

        if (m_iSelectedIndex < m_kObject.m_aKeys.Length)
        {
            GUI.Label(new Rect(210, 5, 40, 20), "Name:");
            m_kObject.m_aKeys[m_iSelectedIndex].name = GUI.TextField(new Rect(255, 5, 200, 20), m_kObject.m_aKeys[m_iSelectedIndex].name);

            GUI.Label(new Rect(210, 30, 245, 50), "Codes", centre);

            bool stop = false;
            if (GUI.Button(new Rect(210, 80, 260, 20), "Add"))
            {
                string[] keynames = Enum.GetNames(typeof(KeyCode));
                for (int i = 0; i < keynames.Length; ++i)
                {
                    if (Keybinding.AddCode(m_kObject.m_aKeys[m_iSelectedIndex].name, (KeyCode)Enum.Parse(typeof(KeyCode), keynames[i])))
                    {
                        EditorUtility.SetDirty(m_kObject);
                        break;
                    }
                }
            }

            m_v2CodePos = GUI.BeginScrollView(new Rect(210, 110, 260, position.height - 115), m_v2CodePos, new Rect(0, 0, 245, m_kObject.m_aKeys[m_iSelectedIndex].keyCodes.Length * 20));
            for (int i = 0; i < m_kObject.m_aKeys[m_iSelectedIndex].keyCodes.Length; ++i)
            {
                if (GUI.Button(new Rect(0, i * 20, 225, 20), m_kObject.m_aKeys[m_iSelectedIndex].keyCodes[i].ToString()))
                {
                    GetWindow<EnumSelector>().Init(typeof(KeyCode), Done);
                    m_iSelectedKey = i;
                }

                if (GUI.Button(new Rect(225, i * 20, 20, 20), "X"))
                {
                    Keybinding.RemoveCode(m_kObject.m_aKeys[m_iSelectedIndex].name, m_kObject.m_aKeys[m_iSelectedIndex].keyCodes[i]);
                    stop = true;
                    break;
                }
            }
            GUI.EndScrollView();

            if (stop)
            {
                Repaint();
                EditorUtility.SetDirty(m_kObject);
            }
        }

        if (GUI.Button(new Rect(5, position.height - 25, 100, 20), "Add"))
        {
            int number = 0;
            bool result = false;
            do
            {
                result = Keybinding.AddKey("new key (" + number.ToString() + ")", KeyCode.A);
                ++number;
            } while (!result);

            EditorUtility.SetDirty(m_kObject);
            Repaint();
        }

        if (GUI.Button(new Rect(110, position.height - 25, 100, 20), "Remove"))
        {
            if (m_iSelectedIndex < m_kObject.m_aKeys.Length)
            {
                Keybinding.RemoveKey(m_kObject.m_aKeys[m_iSelectedIndex].name);
                EditorUtility.SetDirty(m_kObject);
                Repaint();
            }
        }
    }

    public void Done(string a_name)
    {
        if (m_iSelectedIndex < m_kObject.m_aKeys.Length)
        {
            Keybinding.ChangeCode(m_kObject.m_aKeys[m_iSelectedIndex].name, m_kObject.m_aKeys[m_iSelectedIndex].keyCodes[m_iSelectedKey], (KeyCode)Enum.Parse(typeof(KeyCode), a_name));
        }
        Repaint();
        EditorUtility.SetDirty(m_kObject);
    }
}

public class EnumSelector : EditorWindow
{
    public delegate void EnumDel(string a_name);

    public void Init(Type a_type, EnumDel a_del)
    {
        titleContent = new GUIContent("Enum Selector");
        m_aAllNames = Enum.GetNames(a_type);
        m_aSearchNames = new List<string>(m_aAllNames);
        m_search = "";
        m_searchPrev = "";
        m_eFunc = a_del;
    }

    private string m_search;
    private string m_searchPrev;
    private string[] m_aAllNames;
    private List<string> m_aSearchNames;
    private Vector2 m_v2Scroll;
    private bool m_bPerfect;
    private EnumDel m_eFunc;

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
                m_eFunc(m_aSearchNames[i]);
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
                        if (m_aAllNames[i].IndexOf(m_search, StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            m_aSearchNames.Add(m_aAllNames[i]);
                        }
                    }
                    else
                    {
                        if (StringComparer.OrdinalIgnoreCase.Compare(m_aAllNames[i], m_search) == 0)
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