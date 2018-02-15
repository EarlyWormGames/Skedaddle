using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSplineManager : MonoBehaviour
{
    public static CameraSplineManager instance;

    private List<CameraSpline> Splines = new List<CameraSpline>();
    private Dictionary<ANIMAL_NAME, CameraSpline> CurrentSplines = new Dictionary<ANIMAL_NAME, CameraSpline>();

    private void Start()
    {
        instance = this;
        Splines.AddRange(FindObjectsOfType<CameraSpline>());
        var animals = FindObjectsOfType<Animal>();
        foreach (var animal in animals)
        {
            CurrentSplines.Add(animal.m_eName, null);
        }

        DisableAllSplines();
        foreach (var spline in Splines)
        {
            if (spline.IsDefaultSpline)
                spline.enabled = true;
        }
    }

    public void DisableAllSplines()
    {
        foreach (var spline in Splines)
        {
            spline.enabled = false;
        }
    }

    public void EnableSpline(ANIMAL_NAME a_name, CameraSpline spline)
    {
        if (CurrentSplines[a_name] != null)
            CurrentSplines[a_name].enabled = false;

        CurrentSplines[a_name] = spline;
        spline.enabled = true;
    }

    public void EnableSplineLoris(CameraSpline spline)
    {
        EnableSpline(ANIMAL_NAME.LORIS, spline);
    }

    public void EnableSplinePoodle(CameraSpline spline)
    {
        EnableSpline(ANIMAL_NAME.POODLE, spline);
    }

    public void EnableSplineAnteater(CameraSpline spline)
    {
        EnableSpline(ANIMAL_NAME.ANTEATER, spline);
    }

    public void EnableSplineZebra(CameraSpline spline)
    {
        EnableSpline(ANIMAL_NAME.ZEBRA, spline);
    }

    public void EnableSplineElephant(CameraSpline spline)
    {
        EnableSpline(ANIMAL_NAME.ELEPHANT, spline);
    }
}