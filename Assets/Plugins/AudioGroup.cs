using UnityEngine;
using System;
using System.Collections.Generic;
using FMODUnity;

public enum eAudioGroup
{
    MASTER,
    MUSIC,
    SFX,
    AMBIENCE,
}

public class AudioGroup : MonoBehaviour
{
    public static Dictionary<eAudioGroup, float> m_Volumes;
    public static Dictionary<eAudioGroup, List<StudioEventEmitter>> m_SoundGroups;

    private static AudioGroup Instance;

    private List<bool> m_WasPaused = new List<bool>();

    public static void AddToGroup(eAudioGroup a_group, StudioEventEmitter a_emitter)
    {
        if (m_SoundGroups == null)
        {
            m_SoundGroups = new Dictionary<eAudioGroup, List<StudioEventEmitter>>();
            int length = Enum.GetNames(typeof(eAudioGroup)).Length;
            for (int i = 0; i < length; ++i)
            {
                m_SoundGroups.Add((eAudioGroup)i, new List<StudioEventEmitter>());
            }
        }

        if (m_Volumes == null)
        {
            m_Volumes = new Dictionary<eAudioGroup, float>();
            int length = Enum.GetNames(typeof(eAudioGroup)).Length;
            for (int i = 0; i < length; ++i)
            {
                m_Volumes.Add((eAudioGroup)i, 0);
            }
        }

        m_SoundGroups[a_group].Add(a_emitter);

        if (Instance == null)
        {
            Instance = new GameObject().AddComponent<AudioGroup>();
            DontDestroyOnLoad(Instance.gameObject);
        }
    }

    public static void SetVolume(eAudioGroup a_group, float a_volume)
    {
        if (m_SoundGroups == null)
        {
            m_SoundGroups = new Dictionary<eAudioGroup, List<StudioEventEmitter>>();
            int length = Enum.GetNames(typeof(eAudioGroup)).Length;
            for (int i = 0; i < length; ++i)
            {
                m_SoundGroups.Add((eAudioGroup)i, new List<StudioEventEmitter>());
            }
        }

        if (m_Volumes == null)
        {
            m_Volumes = new Dictionary<eAudioGroup, float>();
            int length = Enum.GetNames(typeof(eAudioGroup)).Length;
            for (int i = 0; i < length; ++i)
            {
                m_Volumes.Add((eAudioGroup)i, 0);
            }
        }

        m_Volumes[a_group] = a_volume;
    }

    public static float GetRawVolume(eAudioGroup a_group)
    {
        if (m_Volumes == null)
        {
            m_Volumes = new Dictionary<eAudioGroup, float>();
            int length = Enum.GetNames(typeof(eAudioGroup)).Length;
            for (int i = 0; i < length; ++i)
            {
                m_Volumes.Add((eAudioGroup)i, 0);
            }
        }

        return m_Volumes[a_group];
    }

    public static float GetVolume(eAudioGroup a_group)
    {
        if (m_Volumes == null)
        {
            m_Volumes = new Dictionary<eAudioGroup, float>();
            int length = Enum.GetNames(typeof(eAudioGroup)).Length;
            for (int i = 0; i < length; ++i)
            {
                m_Volumes.Add((eAudioGroup)i, 0);
            }
        }

        return m_Volumes[a_group] + m_Volumes[eAudioGroup.MASTER];
    }

    void OnApplicationFocus(bool a_focus)
    {
#if UNITY_EDITOR
        a_focus = true;
#endif

        if (!a_focus)
        {
            m_WasPaused.Clear();
            foreach (var pair in m_SoundGroups)
            {
                foreach (var item in pair.Value)
                {
                    m_WasPaused.Add(item.isPaused);
                    item.isPaused = true;
                }
            }
        }
        else
        {
            if (m_WasPaused.Count <= 0)
                return;

            foreach (var pair in m_SoundGroups)
            {
                for (int i = 0; i < pair.Value.Count; ++i)
                {
                    pair.Value[i].isPaused = m_WasPaused[i];
                }
            }
            m_WasPaused.Clear();
        }
    }

    private float m_fTimer = 10f;
    void Update()
    {
        m_fTimer -= Time.deltaTime;
        if (m_fTimer <= 0f)
        {
            m_fTimer = 10f;
            foreach (var pair in m_SoundGroups)
            {
                pair.Value.RemoveAll(item => item == null);
            }
        }
    }

    void OnDestroy()
    {
        foreach (var pair in m_SoundGroups)
        {
            pair.Value.RemoveAll(item => item == null);
        }
    }
}