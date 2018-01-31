using UnityEngine;
using System.Collections;

public class Teleport : MonoBehaviour
{
    public void TeleportAnimal(Transform TeleportLocation)
    {
        Transform teleAnim = AnimalController.Instance.GetCurrentAnimal().transform;
        teleAnim.position = TeleportLocation.position;
    }
}
