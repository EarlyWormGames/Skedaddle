using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;
using XInputDotNetPure;

public enum GPExtra
{
    NONE,
    LeftStick,
    LeftStickX,
    LeftStickY,
    RightStick,
    RightStickX,
    RightStickY,
    LeftTrigger,
    RightTrigger,
}

public class GPValues
{
    public float fVal;
    public Vector2 v2Val;
}

[Serializable]
public class KBKey
{
    [SerializeField] public string name;
    [SerializeField] public KeyCode[] keyCodes;
    [SerializeField] public GPExtra[] gpExtras;

    public KBKey(string a_name, KeyCode a_code)
    {
        name = a_name;
        keyCodes = new KeyCode[] { a_code };
        gpExtras = new GPExtra[] { GPExtra.NONE };
    }

    public KBKey(string a_name, KeyCode[] a_code)
    {
        name = a_name;
        keyCodes = a_code;
        gpExtras = new GPExtra[] { GPExtra.NONE };
    }
}

[Serializable]
public class Keybinding : ScriptableObject
{
    //=====================================
    // STATIC OBJECTS
    public static Keybinding Instance
    {
        get
        {
            if (m_kInstance == null)
            {
                m_kInstance = Resources.Load<Keybinding>("Keybinding");
            }
            if (m_kInstance != null)
            {
                if (m_kInstance.m_kObject == null)
                {
                    m_kInstance.m_kObject = new GameObject().AddComponent<KeybindObject>();
                }
            }
            return m_kInstance;
        }
    }
    private static Keybinding m_kInstance;
    //=====================================

    //=====================================
    // PUBLIC VARS
    [SerializeField] public KBKey[] m_aKeys;

    //=====================================

    //=====================================
    // PRIVATE VARS
    private KeybindObject m_kObject;



    //=====================================
    // GET FUNCTIONS
    public static bool GetKey(string a_name)
    {
        return CheckKey(a_name);
    }

    public static bool GetKeyDown(string a_name)
    {
        return CheckKey(a_name, 1);
    }

    public static bool GetKeyUp(string a_name)
    {
        return CheckKey(a_name, 2);
    }

    private static bool CheckKey(string a_name, int type = 0)
    {
        if (Instance == null)
            return false;

        KBKey[] keys = Instance.m_aKeys;
        if (keys == null)
            return false;

        for (int i = 0; i < keys.Length; ++i)
        {
            if (keys[i].name == a_name)
            {
                for (int j = 0; j < keys[i].keyCodes.Length; ++j)
                {
                    if (type == 0)
                    {
                        if (Input.GetKey(keys[i].keyCodes[j]))
                        {
                            if (!Instance.m_kObject.m_Keys.Contains(keys[i].keyCodes[j]))
                                Instance.m_kObject.m_Keys.Add(keys[i].keyCodes[j]);
                            return true;
                        }
                    }
                    else if (type == 1)
                    {
                        if (Input.GetKeyDown(keys[i].keyCodes[j]) && !Instance.m_kObject.m_Keys.Contains(keys[i].keyCodes[j]))
                        {
                            Instance.m_kObject.m_Keys.Add(keys[i].keyCodes[j]);
                            return true;
                        }
                    }
                    else if (type == 2)
                    {
                        if (Input.GetKeyUp(keys[i].keyCodes[j]))
                            return true;
                    }
                }
                return false;
            }
        }
        return false;
    }

    public static bool AnyKeyDown()
    {
        if (Instance == null)
            return false;

        KBKey[] keys = Instance.m_aKeys;
        if (keys == null)
            return false;

        for (int i = 0; i < keys.Length; ++i)
        {
            for (int j = 0; j < keys[i].keyCodes.Length; ++j)
            {
                if (Input.GetKey(keys[i].keyCodes[j]))
                {
                    return true;
                }
            }
        }
        return false;
    }
    //=====================================

    //=====================================
    // KEY ADJUSTMENTS (DOES NOT AUTO-SAVE)
    public static bool AddKey(string a_name, KeyCode a_code)
    {
        if (Instance == null)
            return false;
        int len = Instance.m_aKeys == null ? 0 : Instance.m_aKeys.Length;

        for (int i = 0; i < len; ++i)
        {
            if (Instance.m_aKeys[i].name == a_name)
                return false;
        }

        KBKey[] keys = new KBKey[len + 1];
        for (int i = 0; i < len; ++i)
        {
            keys[i] = Instance.m_aKeys[i];
        }
        keys[keys.Length - 1] = new KBKey(a_name, a_code);
        Instance.m_aKeys = keys;
        return true;
    }

    public static void RemoveKey(string a_name)
    {
        if (Instance == null)
            return;
        if (Instance.m_aKeys == null)
            return;
        if (Instance.m_aKeys.Length < 1)
            return;

        bool hit = false;
        KBKey[] keys = new KBKey[Instance.m_aKeys.Length - 1];
        for (int i = 0; i < Instance.m_aKeys.Length; ++i)
        {
            if (!hit)
            {
                if (Instance.m_aKeys[i].name == a_name)
                {
                    hit = true;
                    continue;
                }
                keys[i] = Instance.m_aKeys[i];
            }
            else
            {
                keys[i - 1] = Instance.m_aKeys[i];
            }
        }
        Instance.m_aKeys = keys;
    }

    public static bool AddCode(string a_name, KeyCode a_code)
    {
        if (Instance == null)
            return false;
        if (Instance.m_aKeys == null)
            return false;
        if (Instance.m_aKeys.Length < 1)
            return false;

        int maindex = 0;
        for (int i = 0; i < Instance.m_aKeys.Length; ++i)
        {
            if (Instance.m_aKeys[i].name == a_name)
            {
                maindex = i;
                for (int j = 0; j < Instance.m_aKeys[i].keyCodes.Length; ++j)
                {
                    if (Instance.m_aKeys[i].keyCodes[j] == a_code)
                        return false;
                }
                break;
            }
        }

        KeyCode[] keys = new KeyCode[Instance.m_aKeys[maindex].keyCodes.Length + 1];
        for (int i = 0; i < Instance.m_aKeys[maindex].keyCodes.Length; ++i)
        {
            keys[i] = Instance.m_aKeys[maindex].keyCodes[i];
        }
        keys[keys.Length - 1] = a_code;
        Instance.m_aKeys[maindex].keyCodes = keys;
        return true;
    }

    public static void RemoveCode(string a_name, KeyCode a_code)
    {
        if (Instance == null)
            return;
        if (Instance.m_aKeys == null)
            return;
        if (Instance.m_aKeys.Length < 1)
            return;

        int maindex = 0;
        int removedex = 0;
        for (int i = 0; i < Instance.m_aKeys.Length; ++i)
        {
            if (Instance.m_aKeys[i].name == a_name)
            {
                if (Instance.m_aKeys[i].keyCodes.Length < 2)
                    return;

                maindex = i;                
                for (int j = 0; j < Instance.m_aKeys[i].keyCodes.Length; ++j)
                {
                    if (Instance.m_aKeys[i].keyCodes[j] == a_code)
                    {
                        removedex = j;
                        break;
                    }
                }
                break;
            }
        }

        KeyCode[] keys = new KeyCode[Instance.m_aKeys[maindex].keyCodes.Length - 1];
        bool hit = false;
        for (int i = 0; i < Instance.m_aKeys[maindex].keyCodes.Length; ++i)
        {
            if (!hit)
            {
                if (i == removedex)
                {
                    hit = true;
                    continue;
                }
                keys[i] = Instance.m_aKeys[maindex].keyCodes[i];
            }
            else
            {
                keys[i - 1] = Instance.m_aKeys[maindex].keyCodes[i];
            }
        }
        Instance.m_aKeys[maindex].keyCodes = keys;
    }

    public static void ChangeCode(string a_name, KeyCode a_oldCode, KeyCode a_newCode)
    {
        if (Instance == null)
            return;
        if (Instance.m_aKeys == null)
            return;
        if (Instance.m_aKeys.Length < 1)
            return;

        int maindex = 0;
        int keydex = 0;
        for (int i = 0; i < Instance.m_aKeys.Length; ++i)
        {
            if (Instance.m_aKeys[i].name == a_name)
            {
                maindex = i;
                for (int j = 0; j < Instance.m_aKeys[i].keyCodes.Length; ++j)
                {
                    if (Instance.m_aKeys[i].keyCodes[j] == a_oldCode)
                    {
                        keydex = j;
                    }
                    else if (Instance.m_aKeys[i].keyCodes[j] == a_newCode)
                        return;
                }
            }
        }

        Instance.m_aKeys[maindex].keyCodes[keydex] = a_newCode;
    }

    public static void RenameKey(string a_name, string a_newName)
    {
        if (Instance == null)
            return;
        if (Instance.m_aKeys == null)
            return;
        if (Instance.m_aKeys.Length < 1)
            return;

        for (int i = 0; i < Instance.m_aKeys.Length; ++i)
        {
            if (Instance.m_aKeys[i].name == a_name)
            {
                Instance.m_aKeys[i].name = a_newName;
                return;
            }
        }
    }
    //=====================================

    //=====================================
    // SAVE & LOAD
    public static void SaveKeys()
    {
        if (Instance == null)
            return;
        if (Instance.m_aKeys == null)
            return;
        if (Instance.m_aKeys.Length < 1)
            return;

        List<string> lines = new List<string>();
        KBKey[] keys = Instance.m_aKeys;
        for (int i = 0; i < keys.Length; ++i)
        {
            lines.Add(keys[i].name);
            lines.Add(keys[i].keyCodes.Length.ToString());

            for (int j = 0; j < keys[i].keyCodes.Length; ++j)
                lines.Add(keys[i].keyCodes[j].ToString());
        }

        if (File.Exists(Application.persistentDataPath + "/Keys.cfg"))
            File.SetAttributes(Application.persistentDataPath + "/Keys.cfg", FileAttributes.Normal);

        //Write all the data to file
        File.WriteAllLines(Application.persistentDataPath + "/Keys.cfg", lines.ToArray());
    }

    public static void LoadKeys()
    {
        if (Instance == null)
            return;

        string[] data = File.ReadAllLines(Application.persistentDataPath + "/Keys.cfg");
        List<KBKey> keys = new List<KBKey>();

        for (int i = 0; i < data.Length;)
        {
            KeyCode[] codes = new KeyCode[Convert.ToInt32(data[i + 1])];
            for (int j = 0; j < codes.Length; ++j)
            {
                codes[j] = (KeyCode)Enum.Parse(typeof(KeyCode), data[i + j + 2], true);
            }
            
            keys.Add(new KBKey(data[i], codes));

            i += 2 + codes.Length;
        }

        Instance.m_aKeys = keys.ToArray();
    }
    //=====================================

    //=====================================
    // MISC FUNCTIONS
    public string[] GetKeyNames()
    {
        if (m_aKeys == null)
            m_aKeys = new KBKey[0];

        string[] names = new string[m_aKeys.Length];
        for (int i = 0; i < names.Length; ++i)
        {
            names[i] = m_aKeys[i].name;
        }
        return names;
    }

    //=====================================
}
