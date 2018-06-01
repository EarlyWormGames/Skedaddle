using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

/// <summary>
/// colour of the labels in the scene
/// </summary>
[CustomPropertyDrawer(typeof(LabelColorAttribute))]
public class LabelColorDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var attr = (LabelColorAttribute)attribute;

        EditorGUI.BeginProperty(position, label, property);

        bool isNull = false;
        //Check if the target is null
        if (PropertyHelpers.GetTargetObjectOfProperty(property) == null)
            isNull = true;

        //Save this for later so we don't set the color of all proceeding elements
        var oldColor = GUI.contentColor;

        //If it's null, or if we don't care, make it so.
        if (!attr.onNullOnly || isNull)
            GUI.contentColor = attr.color;

        //Draw the property
        EditorGUI.PropertyField(position, property, label);
        //Reset the color
        GUI.contentColor = oldColor;

        EditorGUI.EndProperty();
    }
}