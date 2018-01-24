﻿using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.SceneManagement;
using System;
using System.Collections.Generic;

using FMODUnity;

public class GameManager : Singleton<GameManager>
{
    public GameObject m_goMenuPrefab;
    public bool m_bUseDoF = true;
    public string[] m_asDebugScenes;
    public string[] m_asNonGameScenes;

    private bool m_bDoOnce = false;

    private float m_fGameTimer = 0;

    private float m_fGCTimer = 0f;

    protected override void OnAwake()
    {
        base.OnAwake();
    }

    // Use this for initialization
    void Start()
    {
        DontDestroy();
        SceneManager.sceneLoaded += SceneLoaded;
        SceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
    }

    void SceneLoaded(Scene a_scene, LoadSceneMode a_mode)
    {
        m_bDoOnce = true;
        if (IsGameScene(SceneManager.GetActiveScene().name))
        {
            Instantiate(m_goMenuPrefab);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //m_fGameTimer += Time.deltaTime;

        if (m_bDoOnce)
        {
            Renderer[] renderers = FindObjectsOfType<Renderer>();

            foreach (Renderer child in renderers)
            {
                if (child.name.IndexOf("decal", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    child.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                }
            }

            m_bDoOnce = false;
        }

        if (Keybinding.GetKeyDown("Restart"))
        {
            EWApplication.ReloadLevel();
        }

        //m_fGCTimer += Time.deltaTime;
        //
        //if (m_fGCTimer >= 10f)
        //{
        //    m_fGCTimer = 0f;
        //    //GC.Collect();
        //}
    }

    void OnDestroy()
    {
        if (Instance == this)
        {
            Analytics.CustomEvent("Game End", new Dictionary<string, object>
            {
                { "Time", m_fGameTimer }
            });
        }
    }

    public static bool IsDebugScene(string a_sceneName)
    {
        for (int i = 0; i < Instance.m_asDebugScenes.Length; i++)
        {
            if (Instance.m_asDebugScenes[i] == a_sceneName)
                return true;
        }
        return false;
    }

    public bool IsGameScene(string a_sceneName)
    {
        for (int i = 0; i < Instance.m_asNonGameScenes.Length; i++)
        {
            if (Instance.m_asNonGameScenes[i] == a_sceneName)
                return false;
        }
        return true;
    }
}
