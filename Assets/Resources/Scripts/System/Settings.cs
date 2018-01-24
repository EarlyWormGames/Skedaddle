using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityStandardAssets.CinematicEffects;
using UnityEngine.SceneManagement;
using System.Collections;

public class Settings : MonoBehaviour
{
    public float Brightness { get { return m_Brightness; } set { m_Brightness = value; } }
    public Slider m_BrightnessSlider;

    public static UnityAction OnSettingsChanged;

    private static float m_Brightness = 1.17f;
    private TonemappingColorGrading m_Tone;
    private static bool m_AmbientOcclusion = true;

    void Start()
    {
        m_BrightnessSlider.value = Brightness;
        m_Tone = FindObjectOfType<TonemappingColorGrading>();
        if (m_Tone != null)
        {
            TonemappingColorGrading.TonemappingSettings map = m_Tone.tonemapping;
            map.exposure = m_Brightness;
            m_Tone.tonemapping = map;
        }
    }

    void Update()
    {
        if (m_Tone != null)
        {
            TonemappingColorGrading.TonemappingSettings map = m_Tone.tonemapping;
            map.exposure = m_Brightness;
            m_Tone.tonemapping = map;
        }
    }

    public static void LoadSettings()
    {

    }

    public static int AA
    {
        get
        {
            return QualitySettings.antiAliasing;
        }
        set
        {
            QualitySettings.antiAliasing = value;
        }
    }

    public static bool AO
    {
        get
        {
            return m_AmbientOcclusion;
        }
        set
        {
            m_AmbientOcclusion = value;
        }
    }
}
