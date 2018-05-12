using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CameraMover : MonoBehaviour
{
    public static Vector3 CurrentPoint;
    public static Vector3 LookAtPoint;
    
    [Tooltip("Will the mover be active on level start?")]
    public List<ANIMAL_NAME> DefaultAnimals;
    [Tooltip("The Animals this mover can be used for")]
    public List<ANIMAL_NAME> MyAnimals = new List<ANIMAL_NAME>();

    internal bool[] EnableForAnimals;
    protected Animal currentAnimal;

    // Use this for initialization
    void Start()
    {
        OnStart();

        if (DefaultAnimals.Count > 0)
        {
            foreach (var item in DefaultAnimals)
            {
                if (CameraSplineManager.instance.EnableSpline(item, this))
                {
                    Debug.LogWarning("Multiple default splines are enabled for " + item.ToString());
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        OnUpdate();
        CheckCurrentAnimal();
        if (currentAnimal == null)
            return;

        if (CameraSplineManager.instance.OverrideSpline != this && CameraSplineManager.instance.OverrideSpline != null)
            return;

        CalcPosition();
    }

    protected abstract void OnStart();
    protected abstract void OnUpdate();

    /// <summary>
    /// Check which <see cref="Animal"/> to use in the next calculation. This should set <see cref="currentAnimal"/>
    /// </summary>
    protected abstract void CheckCurrentAnimal();

    /// <summary>
    /// Calculate the camera's next destination
    /// </summary>
    protected abstract void CalcPosition();

    /// <summary>
    /// Sets the current move-to and look-at points for the camera. Please type check <paramref name="data"/> before using it
    /// </summary>
    /// <param name="data">The optional data to send through</param>
    public abstract void SetCurrentPoint(object data);

    /// <summary>
    /// Set the animal to use to disuse this mover
    /// </summary>
    /// <param name="a_name">The <see cref="Animal"/> to set</param>
    /// <param name="a_enabled"></param>
    /// <returns>If your code purposefully blocks the enable/disable, return false</returns>
    public abstract bool SetAnimalEnabled(ANIMAL_NAME a_name, bool a_enabled);    
}