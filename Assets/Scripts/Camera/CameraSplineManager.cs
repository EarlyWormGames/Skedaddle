using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSplineManager : MonoBehaviour
{
    public static CameraSplineManager instance;
    public static bool HasAnySplines
    {
        get
        {
            if (instance == null)
                return false;

            return instance.Splines.Count > 0;
        }
    }

    public CameraMover OverrideSpline;

    [HideInInspector]
    public List<ANIMAL_NAME> DefaultAnimals;
    [HideInInspector]
    public List<CameraMover> DefaultSplines;

    private List<CameraMover> Splines = new List<CameraMover>();
    private Dictionary<ANIMAL_NAME, CameraMover> CurrentSplines = new Dictionary<ANIMAL_NAME, CameraMover>();

    private void Start()
    {
        instance = this;
        Splines.AddRange(FindObjectsOfType<CameraMover>());
        var animals = FindObjectsOfType<Animal>();
        foreach (var animal in animals)
        {
            CurrentSplines.Add(animal.m_eName, null);
        }

        SetupSplines();
    }

    public int GetSplineCount()
    {
        return Splines.Count;
    }

    public void SetupSplines()
    {
        foreach (var spline in Splines)
        {
            spline.EnableForAnimals = new bool[spline.MyAnimals.Count];
        }

        for(int i = 0; i < DefaultAnimals.Count; ++i)
        {
            if (DefaultSplines[i] == null)
                continue;

            EnableSpline(DefaultAnimals[i], DefaultSplines[i]);
        }
    }

    public bool EnableSpline(ANIMAL_NAME a_name, CameraMover spline)
    {
        if (!CurrentSplines.ContainsKey(a_name))
            return false;

        bool wasSet = false;
        if (CurrentSplines[a_name] != null)
        {
            if (!CurrentSplines[a_name].SetAnimalEnabled(a_name, false))
                return true;
            wasSet = true;
        }

        CurrentSplines[a_name] = spline;
        CurrentSplines[a_name].SetAnimalEnabled(a_name, true);
        return wasSet;
    }

    public void EnableSplineLoris(CameraMover spline)
    {
        EnableSpline(ANIMAL_NAME.LORIS, spline);
    }

    public void EnableSplinePoodle(CameraMover spline)
    {
        EnableSpline(ANIMAL_NAME.POODLE, spline);
    }

    public void EnableSplineAnteater(CameraMover spline)
    {
        EnableSpline(ANIMAL_NAME.ANTEATER, spline);
    }

    public void EnableSplineZebra(CameraMover spline)
    {
        EnableSpline(ANIMAL_NAME.ZEBRA, spline);
    }

    public void EnableSplineElephant(CameraMover spline)
    {
        EnableSpline(ANIMAL_NAME.ELEPHANT, spline);
    }

    public void EnableSplineALL(CameraMover spline)
    {
        foreach(var pair in CurrentSplines)
        {
            if (pair.Value != null)
            {
                if (!pair.Value.SetAnimalEnabled(pair.Key, false))
                    continue;
            }

            CurrentSplines[pair.Key] = spline;
            CurrentSplines[pair.Key].SetAnimalEnabled(pair.Key, true);
        }
    }
}