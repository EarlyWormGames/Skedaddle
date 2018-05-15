﻿using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.InputNew;
using UnityEngine.SceneManagement;
using System;
using System.Collections.Generic;

/// <summary>
/// handles Input and scene management 
/// </summary>
public class GameManager : Singleton<GameManager>
{
    public static MainMapping mainMap;

    public GameObject m_goMenuPrefab;
    public bool m_bUseDoF = true;
    public string[] m_asDebugScenes;
    public string[] m_asNonGameScenes;

    [HideInInspector]
    public PlayerInput input;
    public Dictionary<string, InputControl> controlsList;

    internal float m_fGameTimer = 0;

    private bool m_bDoOnce = false;
    private float m_fGCTimer = 0f;
    private PlayerInput pInput;

    protected override void OnAwake()
    {
        base.OnAwake();
    }

    // Use this for initialization
    void Start()
    {
        DontDestroy();

        pInput = GetComponent<PlayerInput>();

        SceneManager.sceneLoaded += SceneLoaded;
        SceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);

        Application.targetFrameRate = 200;
    }

    void SceneLoaded(Scene a_scene, LoadSceneMode a_mode)
    {
        m_bDoOnce = true;
        if (IsGameScene(SceneManager.GetActiveScene().name))
        {
            Instantiate(m_goMenuPrefab);
        }

        pInput = GetComponent<PlayerInput>();

        var obj = new GameObject();
        input = Utilities.CopyComponent(pInput, obj);
        input.autoAssignGlobal = pInput.autoAssignGlobal;
        input.actionMaps = pInput.actionMaps;
        mainMap = input.GetActions<MainMapping>();
        
        RefreshInputs();
    }

    void RefreshInputs()
    {
        //Make a dictionary of inputs that are case insensitive
        controlsList = new Dictionary<string, InputControl>(StringComparer.OrdinalIgnoreCase);
        foreach (var item in mainMap.actionMap.actions)
        {
            var map = input.handle.GetActions(mainMap.actionMap);
            controlsList.Add(item.name, map[item.actionIndex]);
        }
    }

    // Update is called once per frame
    void Update()
    {
        m_fGameTimer += Time.deltaTime;

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

        if (mainMap.restart.wasJustPressed)
        {
            EWApplication.ReloadLevel();
        }
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
