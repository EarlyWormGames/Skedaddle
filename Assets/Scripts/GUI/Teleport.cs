using UnityEngine;
using System.Collections;

/// <summary>
/// Teleport script for Testing purposes
/// </summary>
public class Teleport : MonoBehaviour
{
    /// <summary>
    /// Teleport Transforms for Hotkeys if needed
    /// </summary>
    [Tooltip ("Teleport Transforms for Hotkeys if needed")]
    public Transform[] teleportPositions;
    /// <summary>
    /// Hotkeys for teleportation if needed
    /// </summary>
    [Tooltip ("Hotkeys for teleportation if needed")]
    public KeyCode[] HotKeys;

    void Update()
    {
        // check if Hotkeys are being pressed and teleport if needed
        for(int i = 0; i < HotKeys.Length; i++)
        {
            if (HotKeys[i] != KeyCode.None)
            {
                if (Input.GetKeyDown(HotKeys[i]))
                {
                    if (teleportPositions[i] != null)
                    {
                        TeleportAnimal(teleportPositions[i]);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Teleport Animal, can be accessed from UI
    /// </summary>
    /// <param name="TeleportLocation"></param>
    public void TeleportAnimal(Transform TeleportLocation)
    {
        Transform teleAnim = AnimalController.Instance.GetCurrentAnimal<Animal>().transform;
        teleAnim.position = TeleportLocation.position;
        Animal animal = teleAnim.GetComponent<Animal>();
        animal.m_aMovement.StopSpline();
        animal.m_bCanWalkLeft = true;
        animal.m_bCanWalkRight = true;
        animal.m_bOnTrampoline = false;
    }
}
