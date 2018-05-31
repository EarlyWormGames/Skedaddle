using UnityEngine;
using System.Collections;

/// <summary>
/// GUI on the menu select items that displays a preview of whats inside the level.s
/// </summary>
[RequireComponent(typeof(Renderer))]
public class LevelPreview : MonoBehaviour
{
    public bool m_Forward = false;
    public float m_OpacityMax = 0.6f;
    public float m_OpacitySpeed = 1f;

    public float scale = 1.0F;

    private float m_OpacityClip = 0f;
    private Renderer m_Renderer;

    void Start()
    {
        m_Renderer = GetComponent<Renderer>();
        m_Renderer.material.SetTexture("_ClipTex", PerlinNoise.Generate2D(1024, 1024, Random.value, Random.value, scale));
    }

    // Update is called once per frame
    void Update()
    {
        m_OpacityClip = Mathf.Clamp01(m_OpacityClip + Time.deltaTime * (m_Forward ? m_OpacitySpeed : -m_OpacitySpeed));
        m_Renderer.material.SetFloat("_ClipAmount", m_OpacityClip);

        m_Forward = false;
    }
}
