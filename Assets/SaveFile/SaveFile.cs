using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;

public class SaveFile
{
    public string name;
    public string[] items;
    public string[] values;

    public SaveFile(string a_name)
    {
        name = a_name;
        items = new string[0];
        values = new string[0];
    }

    //==========================================
    // GET
    public string GetString(string a_item)
    {
        for (int i = 0; i < items.Length; ++i)
        {
            if (items[i] == a_item)
            {
                return values[i];
            }
        }
        return null;
    }

    public float GetFloat(string a_item)
    {
        string val = GetString(a_item);
        try
        {
            return Convert.ToSingle(val);
        }
        catch
        {
            SetFloat(a_item, 0);
            return 0;
        }
    }

    public int GetInt(string a_item)
    {
        string val = GetString(a_item);
        try
        {
            return Convert.ToInt32(val);
        }
        catch
        {
            SetInt(a_item, 0);
            return 0;
        }
    }

    public bool GetBool(string a_item)
    {
        string val = GetString(a_item);
        try
        {
            return Convert.ToBoolean(val);
        }
        catch
        {
            SetBool(a_item, false);
            return false;
        }
    }
    //==========================================


    //==========================================
    // SET
    public void SetString(string a_item, string a_value)
    {
        for (int i = 0; i < items.Length; ++i)
        {
            if (items[i] == a_item)
            {
                values[i] = a_value;
                return;
            }
        }

        string[] nItems = new string[items.Length + 1];
        string[] nValues = new string[items.Length + 1];
        for (int i = 0; i < items.Length; ++i)
        {
            nItems[i] = items[i];
            nValues[i] = values[i];
        }
        nItems[nItems.Length - 1] = a_item;
        nValues[nValues.Length - 1] = a_value;

        items = nItems;
        values = nValues;
    }

    public void SetFloat(string a_item, float a_value)
    {
        SetString(a_item, a_value.ToString());
    }

    public void SetInt(string a_item, int a_value)
    {
        SetString(a_item, a_value.ToString());
    }

    public void SetBool(string a_item, bool a_value)
    {
        SetString(a_item, a_value.ToString());
    }
    //==========================================
}

public class Saves
{
    public static string[] GetFiles()
    {
        string savePath = Application.persistentDataPath + "/Saves";
        if (!Directory.Exists(savePath))
        {
            Directory.CreateDirectory(savePath);
            return new string[0];
        }
        string[] files = Directory.GetFiles(savePath);
        string[] names = new string[files.Length];
        for (int i = 0; i < files.Length; ++i)
        {
            names[i] = new FileInfo(files[i]).Name;
        }
        return names; 
    }

    public static SaveFile Load(string a_name)
    {
        Encryptonator encryp = new Encryptonator();

        string savePath = Application.persistentDataPath + "/Saves";
        string saveFile = savePath + "/" + a_name + ".ews";
        if (!Directory.Exists(savePath))
        {
            Directory.CreateDirectory(savePath);
            return new SaveFile(a_name);
        }

        if (!File.Exists(saveFile))
            return new SaveFile(a_name);

        SaveFile save = new SaveFile(a_name);

        File.SetAttributes(saveFile, FileAttributes.Normal);
        string text = File.ReadAllText(saveFile);
        string[] lines = encryp.DecryptString(text).Replace("\r", "").Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

        save.items = new string[lines.Length];
        save.values = new string[lines.Length];

        for (int i = 0; i < lines.Length; ++i)
        {
            string[] line = lines[i].Split('=');
            save.items[i] = line[0];
            save.values[i] = line[1];
        }

        return save;
    }

    public static void Save(SaveFile a_save)
    {
        string savePath = Application.persistentDataPath + "/Saves";
        string saveFile = savePath + "/" + a_save.name + ".ews";
        if (!Directory.Exists(savePath))
            Directory.CreateDirectory(savePath);

        if (File.Exists(saveFile))
            File.SetAttributes(saveFile, FileAttributes.Normal);

        Encryptonator enc = new Encryptonator();
        string text = "";
        for (int i = 0; i < a_save.items.Length; ++i)
        {
            text += a_save.items[i] + "=" + a_save.values[i] + "\n";
        }
        File.WriteAllText(saveFile, enc.EncryptToString(text));
    }
}
