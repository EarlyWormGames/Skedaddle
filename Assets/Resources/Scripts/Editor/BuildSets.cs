using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

[InitializeOnLoad]
public class BuildSets
{
    static int m_enabledCount = 0;
    static int m_sceneCount = 0;
    static string[] IgnoreScenes = new string[] { "Menu", "SpeedrunEnd", "Extras" };
    static string FilePath = "/Resources/BuildScenes.txt";

    static BuildSets()
    {
        EditorApplication.update += Update;
    }

    //Will check the scene enabled count every frame, then will write to file when it finds difference
    static void Update()
    {
        EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;
        
        //Check enabled count
        int enabled = 0;
        foreach (var scene in scenes)
        {
            if (scene.enabled)
                ++enabled;
        }

        //Diff against known
        if (m_enabledCount != enabled || m_sceneCount != scenes.Length)
        {
            m_enabledCount = enabled;
            m_sceneCount = scenes.Length;

            string textOutput = "";
            //Ignore disabled scenes, ignore scenes in IgnoreScenes array
            for (int i = 0; i < scenes.Length; ++i)
            {
                if (!scenes[i].enabled)
                    continue;

                string name = Path.GetFileNameWithoutExtension(scenes[i].path);
                bool cont = true;
                for (int j = 0; j < IgnoreScenes.Length; ++j)
                {
                    if (IgnoreScenes[j] == name)
                    {
                        cont = false;
                        break;
                    }
                }

                if (!cont)
                    continue;

                //Add to output with new line
                textOutput += name + "\n";
            }

            //Now write file
            if (File.Exists(Application.dataPath + FilePath))
                File.SetAttributes(Application.dataPath + FilePath, FileAttributes.Normal);

            File.WriteAllText(Application.dataPath + FilePath, textOutput);
            AssetDatabase.Refresh();
        }
    }
}