using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomPropertyDrawer(typeof(FootAttribute))]
public class FootDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        Texture browseIcon = EditorGUIUtility.Load("FMOD/SearchIconBlack.png") as Texture;

        EditorGUI.BeginProperty(position, label, property);

        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
        Rect objfield = new Rect(position.x, position.y, position.width - browseIcon.width - 7, position.height);
        Rect iconfield = new Rect(position.x + position.width - browseIcon.width - 7, position.y, browseIcon.width + 7, position.height);

        EditorGUI.PropertyField(objfield, property, GUIContent.none);

        if (property.type != "PPtr<$Transform>")
        {
            EditorGUI.EndProperty();
            return;
        }

        if (GUI.Button(iconfield, browseIcon))
        {
            var windowRect = position;
            windowRect.position = GUIUtility.GUIToScreenPoint(windowRect.position);
            windowRect.height = objfield.height + 1;

            var window = EditorWindow.CreateInstance<TransformChildFinder>();
            window.Init(property, ((Component)property.serializedObject.targetObject).transform);
            window.ShowAsDropDown(windowRect, new Vector2(windowRect.width, 400));
        }
        EditorGUI.EndProperty();
    }
}

public class TransformChildFinder : EditorWindow
{
    private Transform[] m_Children;
    private SerializedProperty m_Parent;
    private Vector2 m_v2Pos;

    public void Init(SerializedProperty a_property, Transform a_trans)
    {
        m_Children = a_trans.GetComponentsInChildren<Transform>();
        m_Parent = a_property;
    }

    void OnGUI()
    {
        m_v2Pos = GUI.BeginScrollView(new Rect(2, 2, position.width - 4, position.height - 4), m_v2Pos, new Rect(0, 0, position.width - 20, m_Children.Length * 25), false, true);

        int i = 0;
        foreach (Transform child in m_Children)
        {
            if (GUI.Button(new Rect(0, i * 25, position.width - 20, 25), child.name))
            {
                m_Parent.objectReferenceValue = child;
                m_Parent.serializedObject.ApplyModifiedProperties();
                Close();
            }
            ++i;
        }

        GUI.EndScrollView();
    }
}