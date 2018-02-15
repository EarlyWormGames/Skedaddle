using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Reflection;

[Serializable]
public struct ScriptEnable
{
    public string m_sScriptType;
    public string[] m_sLevelNames;  
}

public class ScriptController : Singleton<ScriptController>
{
    //============================
    //       Public Vars
    //============================
    public ScriptEnable[]   m_aseScripts;

    public bool             m_bStarted = false;

    //============================
    //       Private Vars
    //============================
    private int             m_iWaitFrames = 2;

    void Start()
    {
        DontDestroy();
        SceneManager.sceneLoaded += SceneLoaded;
    }

    void OnEnable()
    {
        foreach (ScriptEnable child in m_aseScripts)
        {
            bool enable = false;
            for (int i = 0; i < child.m_sLevelNames.Length; ++i)
            {
                if (string.Compare(SceneManager.GetActiveScene().name, child.m_sLevelNames[i], true) == 0)
                {
                    enable = true;
                }
            }
            if (GetComponent(child.m_sScriptType) != null)
            {
                (GetComponent(child.m_sScriptType) as MonoBehaviour).enabled = enable;
            }
        }
    }

    void SceneLoaded(Scene a_scene, LoadSceneMode a_mode)
    {
        foreach (ScriptEnable child in m_aseScripts)
        {
            bool enable = false;
            for (int i = 0; i < child.m_sLevelNames.Length; ++i)
            {
                if (string.Compare(a_scene.name, child.m_sLevelNames[i], true) == 0)
                {
                    enable = true;
                }
            }
            if (GetComponent(child.m_sScriptType) != null)
            {
                (GetComponent(child.m_sScriptType) as MonoBehaviour).enabled = enable;
            }
        }
    }

    void Update()
    {
        if (!m_bStarted)
        {
            --m_iWaitFrames;
            if (m_iWaitFrames <= 0)
            {
                m_bStarted = true;
            }
        }
    }

    public int GetIndexOf(string a_scriptType)
    {
        for (int i = 0; i < m_aseScripts.Length; ++i)
        {
            if (m_aseScripts[i].m_sScriptType == a_scriptType)
            {
                return i;
            }
        }
        return -1;
    }

    public void AddLevelToScript(int a_scriptIndex, string a_levelName)
    {
        if (a_scriptIndex >= 0 && a_scriptIndex < m_aseScripts.Length)
        {
            if (ContainsLevel(a_scriptIndex, a_levelName))
            {
                return;
            }

            string[] names = new string[m_aseScripts[a_scriptIndex].m_sLevelNames.Length + 1];

            for (int i = 0; i < m_aseScripts[a_scriptIndex].m_sLevelNames.Length; ++i)
            {
                names[i] = m_aseScripts[a_scriptIndex].m_sLevelNames[i];
            }
            names[m_aseScripts[a_scriptIndex].m_sLevelNames.Length] = a_levelName;
            m_aseScripts[a_scriptIndex].m_sLevelNames = names;
        }
    }

    public bool ContainsLevel(int a_scriptIndex, string a_levelName)
    {
        foreach (string name in m_aseScripts[a_scriptIndex].m_sLevelNames)
        {
            if (name == a_levelName)
            {
                return true;
            }
        }

        return false;
    }
}
