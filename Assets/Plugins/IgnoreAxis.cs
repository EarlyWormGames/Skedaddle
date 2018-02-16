using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class IgnoreUtils
{
    public static Vector3 Calculate(IgnoreAxis settings, Vector3 currrentPosition, Vector3 newPosition)
    {
        if ((settings & IgnoreAxis.X) > 0)
        {
            newPosition.x = currrentPosition.x;
        }
        if ((settings & IgnoreAxis.Y) > 0)
        {
            newPosition.y = currrentPosition.y;
        }
        if ((settings & IgnoreAxis.Z) > 0)
        {
            newPosition.z = currrentPosition.z;
        }
        return newPosition;
    }
}

public enum IgnoreAxis
{
    None = 0, // Custom name for "Nothing" option
    X = 1 << 0,
    Y = 1 << 1,
    Z = 1 << 2,
    Everything = ~0, // Custom name for "Everything" option
}