using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class IgnoreUtils
{
    /// <summary>
    /// Calculate the newPosition, while ignoring the axes defined in settings and defaulting to currentPosition
    /// </summary>
    /// <param name="settings">The axes to ignore</param>
    /// <param name="currrentPosition">The Vector3 to default to</param>
    /// <param name="newPosition">The new Vector3 to edit</param>
    /// <returns></returns>
    public static Vector3 Calculate(IgnoreAxis settings, Vector3 currrentPosition, Vector3 newPosition)
    {        
        if (settings.HasFlag(IgnoreAxis.X))
        {
            newPosition.x = currrentPosition.x;
        }
        if (settings.HasFlag(IgnoreAxis.Y))
        {
            newPosition.y = currrentPosition.y;
        }
        if (settings.HasFlag(IgnoreAxis.Z))
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

public static class EnumUtils
{
    /// <summary>
    /// Check to see if a flags enumeration has a specific flag set.
    /// </summary>
    /// <param name="variable">Flags enumeration to check</param>
    /// <param name="value">Flag to check for</param>
    /// <returns></returns>
    public static bool HasFlag(this Enum variable, Enum value)
    {
        if (variable == null)
            return false;

        if (value == null)
            throw new ArgumentNullException("value");

        // Not as good as the .NET 4 version of this function, but should be good enough
        if (!Enum.IsDefined(variable.GetType(), value))
        {
            throw new ArgumentException(string.Format(
                "Enumeration type mismatch.  The flag is of type '{0}', was expecting '{1}'.",
                value.GetType(), variable.GetType()));
        }

        ulong num = Convert.ToUInt64(value);
        return ((Convert.ToUInt64(variable) & num) == num);

    }
}