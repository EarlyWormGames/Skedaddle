using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine.Events;
using System.Reflection;

[CustomEditor(typeof(EventOverTime))]
public class EventOverTimeInspector : Editor
{
    EventOverTime self;
    List<AnimBool> groups = new List<AnimBool>();

    private void OnEnable()
    {
        self = serializedObject.targetObject as EventOverTime;
        groups = new List<AnimBool>();
        foreach (var item in self.Events)
        {
            var toggle = new AnimBool(item._showInspector);
            toggle.valueChanged.AddListener(Repaint);
            groups.Add(toggle);
        }
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        if (groups.Count != self.Events.Count)
        {
            groups = new List<AnimBool>();
            foreach (var item in self.Events)
            {
                var toggle = new AnimBool(false);
                toggle.valueChanged.AddListener(Repaint);
                groups.Add(toggle);
            }
        }

        for (int i = 0; i < self.Events.Count; ++i)
        {
            if (self.Events[i] == null)
                self.Events[i] = new EventOverTime.EventTimer();

            var e = serializedObject.FindProperty("Events").GetArrayElementAtIndex(i);

            groups[i].target = EditorGUILayout.Foldout(groups[i].target, "Event " + i.ToString());

            self.Events[i]._showInspector = groups[i].target;

            using (var grp = new EditorGUILayout.FadeGroupScope(groups[i].faded))
            {
                if (grp.visible)
                {
                    EditorGUI.indentLevel++;

                    EditorGUILayout.PropertyField(e.FindPropertyRelative("_time"));
                    EditorGUILayout.PropertyField(e.FindPropertyRelative("_usePercent"));
                    EditorGUILayout.PropertyField(e.FindPropertyRelative("_curve"));
                    EditorGUILayout.PropertyField(e.FindPropertyRelative("_event"));

                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button("+"))
                    {
                        Undo.RecordObject(self, "insert event element");
                        var item = new EventOverTime.EventTimer();
                        item._time = 1;
                        item._usePercent = true;
                        item._curve = AnimationCurve.EaseInOut(0, 0, 1, 1);

                        var toggle = new AnimBool(false);
                        toggle.valueChanged.AddListener(Repaint);

                        self.Events.Insert(i + 1, item);
                        groups.Insert(i + 1, toggle);

                        Repaint();
                        return;
                    }
                    else if (GUILayout.Button("D"))
                    {
                        Undo.RecordObject(self, "duplicate event element");
                        var item = new EventOverTime.EventTimer();
                        item._time = self.Events[i]._time;
                        item._usePercent = self.Events[i]._usePercent;
                        item._curve = new AnimationCurve(self.Events[i]._curve.keys);
                        item._event = new FloatEvent();

                        var toggle = new AnimBool(false);
                        toggle.valueChanged.AddListener(Repaint);

                        self.Events.Insert(i + 1, item);
                        groups.Insert(i + 1, toggle);

                        Repaint();
                        return;
                    }
                    else if (GUILayout.Button("-"))
                    {
                        Undo.RecordObject(self, "remove event element");
                        self.Events.RemoveAt(i);
                        groups.RemoveAt(i);
                        --i;
                        Repaint();
                        return;
                    }
                    GUILayout.EndHorizontal();

                    EditorGUI.indentLevel--;
                }
            }
        }

        if (GUILayout.Button("+"))
        {
            Undo.RecordObject(self, "add event element");
            var item = new EventOverTime.EventTimer();
            item._time = 1;
            item._usePercent = true;
            item._curve = AnimationCurve.EaseInOut(0, 0, 1, 1);

            var toggle = new AnimBool(false);
            toggle.valueChanged.AddListener(Repaint);

            self.Events.Add(item);
            groups.Add(toggle);

            Repaint();
            return;
        }

        serializedObject.ApplyModifiedProperties();
    }
}