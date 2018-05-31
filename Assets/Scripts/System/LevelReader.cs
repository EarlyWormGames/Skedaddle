using UnityEngine;
using System;

/// <summary>
/// read the level names from file.
/// </summary>
public class LevelReader
{
    public static string[] LevelNames
    {
        get
        {
            if (names == null)
                ReadNames();
            return names;
        }
    }

    private static string[] names = null;
    private static string FilePath = "BuildScenes";

    private static void ReadNames()
    {
        TextAsset asset = Resources.Load<TextAsset>(FilePath);
        names = asset.text.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
    }
}
