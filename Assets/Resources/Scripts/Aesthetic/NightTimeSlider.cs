using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NightTimeSlider : MonoBehaviour {

    [Range(0,1)] public float m_fTimeOfNight;
    public Material[] m_matEffectedMaterials;

    private Color[] StartColor;

    void Start()
    {
        StartColor = new Color[m_matEffectedMaterials.Length];
        int i = 0;
        foreach (Material mat in m_matEffectedMaterials)
        {
            StartColor[i] = mat.color;
            i++;
        }
    }
	
	void Update ()
    {
        int i = 0;
        foreach (Material mat in m_matEffectedMaterials)
        {
            mat.color = new Color(StartColor[i].r * m_fTimeOfNight, StartColor[i].g * m_fTimeOfNight, StartColor[i].b * m_fTimeOfNight);
            i++;
        }
	}
}
