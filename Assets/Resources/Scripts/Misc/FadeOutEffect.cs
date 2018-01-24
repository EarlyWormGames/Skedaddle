using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class FadeOutEffect : MonoBehaviour
{
    public static float Amount;

    public Material material;
    [Range(0, 1)] public float m_Amount;
    public float m_Multiplier = 1f;
    public bool m_Invert = false;

    private float m_PrevAmount;

    void Start()
    {
        m_PrevAmount = m_Amount;
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (m_PrevAmount != m_Amount)
        {
            Amount = m_Amount;
            m_PrevAmount = m_Amount;
        }
        else
        {
            m_Amount = Amount;
            m_PrevAmount = Amount;
        }

        if (material != null)
        {
            material.SetFloat("_Amount", m_Amount);
            material.SetFloat("_Mult", m_Multiplier);
            material.SetInt("_Invert", m_Invert ? 1 : 0);
            Graphics.Blit(source, destination, material);
        }
    }
}
