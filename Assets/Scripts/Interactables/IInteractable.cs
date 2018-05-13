using UnityEngine;
using UnityEngine.InputNew;

public interface IInteractable
{
    ///<summary>Check if this interactable uses the key</summary>
    bool CheckInfo(ActionSlot input, Animal caller);
    ///<summary>Should this interactable ignore the distance check and automatically interact when possible</summary>
    bool IgnoreDistance();
    ///<summary>Get the distance from point. Used to check keypress</summary>
    float GetDistance(Vector3 point);

    void Interact(Animal caller);
}