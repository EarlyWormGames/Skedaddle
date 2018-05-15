using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class CameraSplineSetter : AnimalTrigger
{
    public CameraMover Spline;
    public LayerMask RequiredLayer;

    public override void AnimalEnter(Animal animal)
    {
        CameraSplineManager.instance.EnableSpline(animal.m_eName, Spline);
    }

    public override void AnimalExit(Animal animal)
    {
    }
}
