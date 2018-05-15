using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

/// <summary>
/// Fade effect for a transition
/// </summary>
public class TransitionEffect : MonoBehaviour
{
    public static TransitionEffect instance;
    public LayerMask volumeLayer;
    public float value;

    private PostProcessVolume volume;

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        volume = PostProcessManager.instance.GetHighestPriorityVolume(volumeLayer);

        if (volume == null)
            return;

        TransitionPP settings = null;
        volume.profile.TryGetSettings(out settings);

        if (settings != null)
            settings.blend.value = value;
    }
}