using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

public class DefaultMoverWindow : EditorWindow
{
    private CameraSplineManager manager;
    private bool waitOne;
    private Animal[] animals;

    [MenuItem("Early Worm/Default Animal")]
    public static void OpenWindow()
    {
        var window = GetWindow<DefaultMoverWindow>();
        window.titleContent = new GUIContent("Default Mover");
    }

    private void OnEnable()
    {
        EditorApplication.playModeStateChanged += StateChange;
        Update(); //Force an initial update check
    }

    private void OnDisable()
    {
        EditorApplication.playModeStateChanged -= StateChange;
    }

    void StateChange(PlayModeStateChange stateChange)
    {
        if (stateChange != PlayModeStateChange.EnteredEditMode)
            waitOne = true;
        else
            waitOne = false;
    }

    private void OnGUI()
    {
        if (Event.current.type == EventType.Layout)
            Update();

        if(waitOne)
        {
            GUILayout.Label("Cannot make changes during play mode");
            return;
        }
  
        foreach(var animal in animals)
        {
            var name = animal.GetName();
            int index = manager.DefaultAnimals.IndexOf(name);

            EditorGUI.BeginChangeCheck();
            var mover = EditorGUILayout.ObjectField(name + ": ", manager.DefaultSplines[index], typeof(CameraMover), true);
            if(EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(manager, "Change defaults");
                manager.DefaultSplines[index] = (CameraMover)mover;
                EditorUtility.SetDirty(manager);
            }
        }

        if(GUILayout.Button("Select Manager"))
        {
            Selection.activeGameObject = manager.gameObject;
        }
    }

    private void Update()
    {
        if (waitOne)
        {
            return;
        }

        manager = FindObjectOfType<CameraSplineManager>();
        if (manager == null)
        {
            manager = new GameObject("Camera Spline Manager").AddComponent<CameraSplineManager>();
            return;
        }

        animals = FindObjectsOfType<Animal>().OrderBy((x) => x.GetName()).ToArray();

        if (manager.DefaultAnimals == null)
        {
            manager.DefaultAnimals = new List<ANIMAL_NAME>();
        }
        if (manager.DefaultSplines == null)
        {
            manager.DefaultSplines = new List<CameraMover>();
        }

        foreach (var animal in animals)
        {
            var name = animal.GetName();

            if (!manager.DefaultAnimals.Contains(name))
            {
                manager.DefaultAnimals.Add(name);
                manager.DefaultSplines.Add(null);
                EditorUtility.SetDirty(manager);
            }
        }
    }
}
