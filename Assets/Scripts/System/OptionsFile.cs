using UnityEngine;
using System;
using System.IO;

/// <summary>
/// Save data for the settings/options   
/// </summary>
[Serializable]
public class OptionsFile
{
    //=========================================================================================================================================
    // STATIC
    private static OptionsFile[] m_aoFiles;

    /// <summary>
    /// Load the options data from file
    /// </summary>
    /// <param name="a_file">Filepath</param>
    /// <returns></returns>
    public static OptionsFile LoadFile(string a_file)
    {
        if (m_aoFiles == null)
            m_aoFiles = new OptionsFile[0];

        for (int i = 0; i < m_aoFiles.Length; ++i)
        {
            if (m_aoFiles[i].m_sName == a_file)
            {
                if (!m_aoFiles[i].m_bLoaded)
                    m_aoFiles[i].Load();
                return m_aoFiles[i];
            }
        }

        OptionsFile file = new OptionsFile(a_file);
        file.Load();
        OptionsFile[] options = new OptionsFile[m_aoFiles.Length + 1];
        for (int i = 0; i < m_aoFiles.Length; ++i)
        {
            options[i] = m_aoFiles[i];
        }
        options[options.Length - 1] = file;
        m_aoFiles = options;
        return file;
    }

    /// <summary>
    /// Save the Options data
    /// </summary>
    /// <param name="a_file">filepath</param>
    public static void SaveFile(string a_file)
    {
        for (int i = 0; i < m_aoFiles.Length; ++i)
        {
            if (m_aoFiles[i].m_sName == a_file)
            {
                if (m_aoFiles[i].m_sName == a_file)
                {
                    m_aoFiles[i].Save();
                    return;
                }
            }
        }
    }

    //=============================================================
    // GET
    public static string GetString(string a_file, string a_name, out bool a_defined)
    {
        OptionsFile file = null;
        for (int i = 0; i < m_aoFiles.Length; ++i)
        {
            if (m_aoFiles[i].m_sName == a_file)
            {
                if (!m_aoFiles[i].m_bLoaded)
                    m_aoFiles[i].Load();
                file = m_aoFiles[i];
            }
        }
        if (file == null)
        {
            a_defined = false;
            return null;
        }
        return file.Get(a_name, out a_defined);
    }

    /// <summary>
    /// Attempt to get a float from the Data file.
    /// </summary>
    /// <param name="a_file">filpath</param>
    /// <param name="a_name">Variable name</param>
    /// <param name="a_defined"> Returned value</param>
    /// <returns></returns>
    public static float GetFloat(string a_file, string a_name, out bool a_defined)
    {
        string item = GetString(a_file, a_name, out a_defined);
        try
        {
            float val = Convert.ToSingle(item);
            return val;
        }
        catch
        {
            Debug.Log("Failed to convert option (" + a_file + ", " + item + ") to bool");
        }

        return 0f;
    }

    /// <summary>
    /// Attempt to get an Int from the Data file
    /// </summary>
    /// <param name="a_file">filepath</param>
    /// <param name="a_name">Variable name</param>
    /// <param name="a_defined">Returned Value</param>
    /// <returns></returns>
    public static int GetInt(string a_file, string a_name, out bool a_defined)
    {
        string item = GetString(a_file, a_name, out a_defined);
        try
        {
            int val = Convert.ToInt32(item);
            return val;
        }
        catch
        {
            Debug.Log("Failed to convert option (" + a_file + ", " + item + ") to bool");
        }

        return 0;
    }

    /// <summary>
    /// Attempt to get a bool from the data file
    /// </summary>
    /// <param name="a_file">Filepath</param>
    /// <param name="a_name">Variable Name</param>
    /// <param name="a_defined">Returned Value</param>
    /// <returns></returns>
    public static bool GetBool(string a_file, string a_name, out bool a_defined)
    {
        string item = GetString(a_file, a_name, out a_defined);
        try
        {
            bool val = Convert.ToBoolean(item);
            return val;
        }
        catch
        {
            Debug.Log("Failed to convert option (" + a_file + ", " + item + ") to bool");
        }

        return false;
    }

    /// <summary>
    /// Get file name Length
    /// </summary>
    /// <param name="a_file">File name</param>
    /// <returns></returns>
    public static int GetNameLength(string a_file)
    {
        OptionsFile file = null;
        for (int i = 0; i < m_aoFiles.Length; ++i)
        {
            if (m_aoFiles[i].m_sName == a_file)
            {
                if (!m_aoFiles[i].m_bLoaded)
                    m_aoFiles[i].Load();
                file = m_aoFiles[i];
            }
        }
        if (file == null)
        {
            return 0;
        }
        return file.m_asNames.Length;
    }
    //=============================================================


    /// <summary>
    /// Create a string in the file
    /// </summary>
    /// <param name="a_file">Filename</param>
    /// <param name="a_name">Variable Name</param>
    /// <param name="a_value">Given Value</param>
    public static void SetString(string a_file, string a_name, string a_value)
    {
        OptionsFile file = null;
        for (int i = 0; i < m_aoFiles.Length; ++i)
        {
            if (m_aoFiles[i].m_sName == a_file)
            {
                file = m_aoFiles[i];
            }
        }
        if (file == null)
            file = LoadFile(a_file);

        file.Set(a_name, a_value);
    }

    //=============================================================
    // SET
    /// <summary>
    /// Create a Float in the file
    /// </summary>
    /// <param name="a_file">Filename</param>
    /// <param name="a_name">Variable Name</param>
    /// <param name="a_value">Given Value</param>
    public static void SetFloat(string a_file, string a_name, float a_value)
    {
        SetString(a_file, a_name, a_value.ToString());
    }

    /// <summary>
    /// Create a Int in the file
    /// </summary>
    /// <param name="a_file">Filename</param>
    /// <param name="a_name">Variable Name</param>
    /// <param name="a_value">Given Value</param>
    public static void SetInt(string a_file, string a_name, int a_value)
    {
        SetString(a_file, a_name, a_value.ToString());
    }

    /// <summary>
    /// Create a Bool in the file
    /// </summary>
    /// <param name="a_file">Filename</param>
    /// <param name="a_name">Variable Name</param>
    /// <param name="a_value">Given Value</param>
    public static void SetBool(string a_file, string a_name, bool a_value)
    {
        SetString(a_file, a_name, a_value.ToString());
    }

    //=============================================================
    //=========================================================================================================================================


    //=========================================================================================================================================
    // ACTUAL FILE
    public string m_sName;
    public bool m_bLoaded;
    private string[] m_asNames;
    private string[] m_asValues;

    /// <summary>
    /// Constructor for Options file
    /// </summary>
    /// <param name="a_name">Given name of the new file</param>
    public OptionsFile(string a_name)
    {
        m_sName = a_name;
    }


    //=============================================================
    // LOAD & SAVE
    public void Load()
    {
        m_bLoaded = true;
        string filename = Application.persistentDataPath + "/Options/" + m_sName + ".ini";

        if (!Directory.Exists(Application.persistentDataPath + "/Options"))
            Directory.CreateDirectory(Application.persistentDataPath + "/Options");

        if (!File.Exists(filename))
        {
            m_asNames = new string[0];
            m_asValues = new string[0];
            return;
        }

        File.SetAttributes(filename, FileAttributes.Normal);
        string[] lines = File.ReadAllText(filename).Replace("\r", "").Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
        m_asNames = new string[lines.Length];
        m_asValues = new string[lines.Length];

        for (int i = 0; i < lines.Length; ++i)
        {
            string[] item = lines[i].Split('=');
            m_asNames[i] = item[0];
            m_asValues[i] = item[1];
        }
    }

    public void Save()
    {
        string filename = Application.persistentDataPath + "/Options/" + m_sName + ".ini";

        if (!Directory.Exists(Application.persistentDataPath + "/Options"))
            Directory.CreateDirectory(Application.persistentDataPath + "/Options");

        if (File.Exists(filename))
        {
            File.SetAttributes(filename, FileAttributes.Normal);
        }

        StreamWriter writer = new StreamWriter(File.Create(filename));
        for (int i = 0; i < m_asNames.Length; ++i)
        {
            writer.WriteLine(m_asNames[i] + "=" + m_asValues[i]);
        }
        writer.Close();
    }
    //=============================================================

    public string Get(string a_name, out bool a_defined)
    {
        for (int i = 0; i < m_asNames.Length; ++i)
        {
            if (m_asNames[i] == a_name)
            {
                a_defined = true;
                return m_asValues[i];
            }
        }
        a_defined = false;
        return null;
    }

    public void Set(string a_name, string a_value)
    {
        bool fek = false; ///lol nice1 @heatblayze
        if (Get(a_name, out fek) == null)
        {
            string[] names = new string[m_asNames.Length + 1];
            string[] values = new string[m_asNames.Length + 1];
            for (int i = 0; i < m_asNames.Length; ++i)
            {
                names[i] = m_asNames[i];
                values[i] = m_asValues[i];
            }
            names[names.Length - 1] = a_name;
            values[values.Length - 1] = a_value;

            m_asNames = names;
            m_asValues = values;
        }
        else
        {
            for (int i = 0; i < m_asNames.Length; ++i)
            {
                if (m_asNames[i] == a_name)
                {
                    m_asValues[i] = a_value;
                }
            }
        }
    }
    //=============================================================
    //=========================================================================================================================================
}