using UnityEngine;
using System.Collections;

/// <summary>
/// Teleport the animal.
/// </summary>
public class Teleport : MonoBehaviour
{
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
