using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field)]
public class LabelColorAttribute : PropertyAttribute
{
    public readonly Color color;
    public readonly bool onNullOnly;

    public LabelColorAttribute(float r, float g, float b)
    {
        color = new Color(r, g, b);
    }

    public LabelColorAttribute(float r, float g, float b, bool onNullOnly)
    {
        color = new Color(r, g, b);
        this.onNullOnly = onNullOnly;
    }
}