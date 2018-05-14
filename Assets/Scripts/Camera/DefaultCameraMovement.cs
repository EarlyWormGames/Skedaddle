using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Refer to cameraMover for documentation
/// </summary>
public abstract class DefaultCameraMovement : CameraMover
{
    protected override void CheckCurrentAnimal()
    {
        for (int i = 0; i < MyAnimals.Count; ++i)
        {
            if (MyAnimals[i] == Animal.CurrentAnimal.m_eName &&
                EnableForAnimals[i])
            {
                currentAnimal = Animal.CurrentAnimal;
                break;
            }
        }
    }

    public override bool SetAnimalEnabled(ANIMAL_NAME a_name, bool a_enabled)
    {
        if (!MyAnimals.Contains(a_name))
            return true;

        EnableForAnimals[MyAnimals.IndexOf(a_name)] = a_enabled;
        return true;
    }
}