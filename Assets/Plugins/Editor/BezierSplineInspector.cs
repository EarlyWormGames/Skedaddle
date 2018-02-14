﻿using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BezierSpline))]
public class BezierSplineInspector : Editor
{
    private const float directionScale = 0.5f;
    private const int stepsPerCurve = 10;
    private const float handleSize = 0.04f;
    private const float pickSize = 0.06f;


    private BezierSpline spline;
    private Transform handleTransform;
    private int selectedIndex = -1;

    private float length;

    private static Color[] modeColors = {
        Color.white,
        Color.yellow,
        Color.cyan
    };

    public override void OnInspectorGUI()
    {
        spline = target as BezierSpline;

        EditorGUI.BeginChangeCheck();
        bool loop = EditorGUILayout.Toggle("Loop", spline.Loop);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(spline, "Toggle Loop");
            EditorUtility.SetDirty(spline);
            spline.Loop = loop;
        }

        if (selectedIndex >= 0 && selectedIndex < spline.ControlPointCount)
        {
            DrawSelectedPointInspector();
        }

        GUILayout.Label("Curve Count: " + spline.CurveCount);

        if (GUILayout.Button("Add Curve"))
        {
            Undo.RecordObject(spline, "Add Curve");
            spline.AddCurve();
            EditorUtility.SetDirty(spline);
        }

        if (spline.ControlPointCount > 4)
        {
            if (GUILayout.Button("Remove Curve"))
            {
                Undo.RecordObject(spline, "Remove Curve");
                spline.RemoveCurve();
                EditorUtility.SetDirty(spline);
            }
        }

        if (GUILayout.Button("Reverse"))
        {
            Undo.RecordObject(spline, "Reverse");
            spline.Reverse();
            EditorUtility.SetDirty(spline);
        }

        EditorGUI.BeginChangeCheck();
        int increments = EditorGUILayout.IntField("Increments Count", spline.ArcIncrements);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(spline, "Increments Count");
            EditorUtility.SetDirty(spline);
            spline.ArcIncrements = increments;
        }

        EditorGUI.BeginChangeCheck();
        int divs = EditorGUILayout.IntField("Sub Divisions", spline.ArcSubDivs);
        if(EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(spline, "Divs");
            EditorUtility.SetDirty(spline);
            spline.ArcSubDivs = divs;
        }

        if (GUILayout.Button("Calculate Increments"))
        {
            Undo.RecordObject(spline, "Calc Increments");
            spline.ArcLength();
            EditorUtility.SetDirty(spline);
        }

        EditorGUI.BeginChangeCheck();
        bool drawincrements = EditorGUILayout.Toggle("Draw Increments", spline.DrawIncrements);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(spline, "Draw Increments");
            spline.DrawIncrements = drawincrements;
            EditorUtility.SetDirty(spline);
        }

        EditorGUI.BeginChangeCheck();
        bool tangents = EditorGUILayout.Toggle("Draw Tangents", spline.DrawTangents);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(spline, "Draw Tangents");
            spline.DrawTangents = tangents;
            EditorUtility.SetDirty(spline);
        }

        EditorGUI.BeginChangeCheck();
        bool lowexit = EditorGUILayout.Toggle("Allow Low Exit", spline.LowExit);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(spline, "Allow Low Exit");
            spline.LowExit = lowexit;
            EditorUtility.SetDirty(spline);
        }

        EditorGUI.BeginChangeCheck();
        bool highexit = EditorGUILayout.Toggle("Allow High Exit", spline.HighExit);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(spline, "Allow High Exit");
            spline.LowExit = highexit;
            EditorUtility.SetDirty(spline);
        }

        EditorGUI.BeginChangeCheck();
        bool reverse = EditorGUILayout.Toggle("Allow Reverse", spline.AllowReverse);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(spline, "Allow Reverse");
            spline.AllowReverse = reverse;
            EditorUtility.SetDirty(spline);
        }

        int index = selectedIndex - 1;
        if (index < 0)
            index = 0;
    }

    private void DrawSelectedPointInspector()
    {
        GUILayout.Label("Selected Point");
        EditorGUI.BeginChangeCheck();
        Vector3 point = EditorGUILayout.Vector3Field("Position", spline.transform.TransformPoint(spline.GetControlPoint(selectedIndex)));
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(spline, "Move Point");
            EditorUtility.SetDirty(spline);
            spline.SetControlPoint(selectedIndex, spline.transform.InverseTransformPoint(point));
        }

        EditorGUI.BeginChangeCheck();
        BezierControlPointMode mode = (BezierControlPointMode)EditorGUILayout.EnumPopup("Mode", spline.GetControlPointMode(selectedIndex));
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(spline, "Change Point Mode");
            spline.SetControlPointMode(selectedIndex, mode);
            EditorUtility.SetDirty(spline);
        }
    }

    private void OnSceneGUI()
    {
        spline = target as BezierSpline;
        handleTransform = spline.transform;

        Vector3 p0 = ShowPoint(0);
        for (int i = 1; i < spline.ControlPointCount; i += 3)
        {
            Vector3 p1 = ShowPoint(i);
            Vector3 p2 = ShowPoint(i + 1);
            Vector3 p3 = ShowPoint(i + 2);

            Handles.color = Color.gray;
            Handles.DrawLine(p0, p1);
            Handles.DrawLine(p2, p3);

            p0 = p3;
        }
        if (spline.DrawTangents)
            ShowDirections();
    }

    private void ShowDirections()
    {
        Handles.color = Color.green;
        Vector3 point = spline.GetPoint(0f);
        Handles.DrawLine(point, point + spline.GetDirection(0f) * directionScale);
        int steps = stepsPerCurve * spline.CurveCount;
        for (int i = 1; i <= steps; i++)
        {
            point = spline.GetPoint(i / (float)steps);
            Handles.DrawLine(point, point + spline.GetDirection(i / (float)steps) * directionScale);
        }
    }

    private Vector3 ShowPoint(int index)
    {
        Vector3 point = handleTransform.TransformPoint(spline.GetControlPoint(index));
        float size = HandleUtility.GetHandleSize(point);
        if (index == 0)
        {
            size *= 2f;
        }
        Handles.color = modeColors[(int)spline.GetControlPointMode(index)];
        if (Handles.Button(point, handleTransform.rotation, size * handleSize, size * pickSize, Handles.DotHandleCap))
        {
            selectedIndex = index;
            Repaint();
        }
        if (selectedIndex == index)
        {
            EditorGUI.BeginChangeCheck();
            point = Handles.DoPositionHandle(point, handleTransform.rotation);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(spline, "Move Point");
                EditorUtility.SetDirty(spline);
                spline.SetControlPoint(index, handleTransform.InverseTransformPoint(point));
            }
        }
        return point;
    }
}