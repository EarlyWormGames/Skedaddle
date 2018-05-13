using UnityEngine;
using UnityEngine.InputNew;

public interface IInteractable
{
    ///<summary>Check if this interactable uses the key</summary>
    bool CheckInfo(InputControl input, Animal caller);
    ///<summary>Should this interactable ignore the distance check and automatically interact when possible</summary>
    bool IgnoreDistance();
    ///<summary>Get the distance from point. Used to check keypress</summary>
    float GetDistance(Vector3 point);
    /// <summary>Called on a successful key (and possible distance) check occurred</summary>
    void Interact(Animal caller);
}