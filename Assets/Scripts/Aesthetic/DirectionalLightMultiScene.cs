using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// container class for level lighting information
/// </summary>
[System.Serializable]public class LevelLightingDetails
{
    public int level;
    public Color lightColour;
    public Vector3 lightRotation;
    public float lightIntensity;
}

/// <summary>
/// management of the level lighting details and applied to the scene
/// </summary>
[ExecuteInEditMode]
public class DirectionalLightMultiScene : MonoBehaviour {

    public static float currentLevel;

    public float m_CurrentLevel;
    public LevelLightingDetails[] lightSettings;

    private Light sourceLight;
    private float previousLevel;

    void Update()
    {
        if (previousLevel != m_CurrentLevel)
        {
            currentLevel = m_CurrentLevel;
            previousLevel = m_CurrentLevel;
        }
        else
        {
            m_CurrentLevel = currentLevel;
            previousLevel = currentLevel;
        }
        sourceLight = GetComponent<Light>();
        if (m_CurrentLevel >= 0 && m_CurrentLevel <= lightSettings.Length - 1)
        {
            sourceLight.color = Color.Lerp(
                lightSettings[Mathf.FloorToInt(m_CurrentLevel)].lightColour,
                lightSettings[Mathf.CeilToInt(m_CurrentLevel)].lightColour,
                m_CurrentLevel - Mathf.FloorToInt(m_CurrentLevel));
            sourceLight.transform.rotation = Quaternion.Lerp(
                Quaternion.Euler(lightSettings[Mathf.FloorToInt(m_CurrentLevel)].lightRotation),
                Quaternion.Euler(lightSettings[Mathf.CeilToInt(m_CurrentLevel)].lightRotation),
                m_CurrentLevel - Mathf.FloorToInt(m_CurrentLevel));
            sourceLight.intensity =
                Mathf.Lerp(lightSettings[Mathf.FloorToInt(m_CurrentLevel)].lightIntensity,
                lightSettings[Mathf.CeilToInt(m_CurrentLevel)].lightIntensity,
                m_CurrentLevel - Mathf.FloorToInt(m_CurrentLevel));
        }
    }
}
