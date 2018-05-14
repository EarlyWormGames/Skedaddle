using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Fade effect for a transition
/// </summary>
public class TransitionEffect : MonoBehaviour
{
    public static TransitionEffect instance;
    public Shader FadeShader;
    public float value;

    private Material FadeMaterial;

    private void Awake()
    {
        instance = this;
        FadeMaterial = new Material(FadeShader);
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        FadeMaterial.SetFloat("_FadeValue", value);
        Graphics.Blit(source, destination, FadeMaterial);
    }
}