using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[Serializable]
[PostProcess(typeof(TransitionPPRenderer), PostProcessEvent.AfterStack, "Early Worm/Transition")]
public sealed class TransitionPP : PostProcessEffectSettings
{
    [Range(0f, 1f), Tooltip("Grayscale effect intensity.")]
    public FloatParameter blend = new FloatParameter { value = 0 };
}

public sealed class TransitionPPRenderer : PostProcessEffectRenderer<TransitionPP>
{
    public override void Render(PostProcessRenderContext context)
    {
        var sheet = context.propertySheets.Get(Shader.Find("Hidden/TransitionShader"));
        sheet.properties.SetFloat("_FadeValue", settings.blend);
        context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
    }
}