﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputNew;
using UnityEngine.SceneManagement;

public class InteractChecker : Singleton<InteractChecker>
{
    private Dictionary<ActionSlot, List<IInteractable>> Listeners = new Dictionary<ActionSlot, List<IInteractable>>();

    // Use this for initialization
    void Start()
    {
        SceneManager.sceneUnloaded += SceneManager_sceneUnloaded;
    }

    private void SceneManager_sceneUnloaded(Scene arg0)
    {
        Listeners.Clear();
    }

    // Update is called once per frame
    void Update()
    {
        //Loop through all listener keys
        foreach(var pair in Listeners)
        {
            if(CheckKey(pair.Key))
            {
                float distance = Mathf.Infinity;
                IInteractable current = null;

                //Loop through all of the interactables that listen to this key
                foreach(var interactable in pair.Value)
                {
                    if (interactable.Equals(null))
                        continue;

                    //Check if the interactable accepts the input at this moment
                    if(interactable.CheckInfo(pair.Key, Animal.CurrentAnimal))
                    {
                        if(interactable.IgnoreDistance())
                        {
                            //Just call interact and ignore the distance check
                            interactable.Interact(Animal.CurrentAnimal);
                            continue;
                        }
                        else
                        {
                            //Default to the camera position if no Animal is selected
                            var point = Animal.CurrentAnimal != null ? Animal.CurrentAnimal.transform.position : Camera.main.transform.position;
                            float dist = interactable.GetDistance(point);
                            if(dist < distance)
                            {
                                //Use the closest interactable
                                distance = dist;
                                current = interactable;
                            }
                        }
                    }
                }

                //Call interact on the interactable
                if (current != null)
                    current.Interact(Animal.CurrentAnimal);
            }
        }
    }

    /// <summary>
    /// Checks what type the <paramref name="slot"/> is and return if it has been pressed
    /// </summary>
    bool CheckKey(ActionSlot slot)
    {
        if (slot.GetType().IsAssignableFrom(typeof(ButtonAction)))
        {
            ButtonAction button = (ButtonAction)slot;
            return button.control.wasJustPressed && button.control.isEnabled;
        }
        else if(slot.GetType().IsAssignableFrom(typeof(AxisAction)))
        {
            AxisAction axis = (AxisAction)slot;
            return axis.control.value != 0 && axis.control.isEnabled;
        }
        return false;
    }

    ///<summary>Add a listener to a list of <see cref="ButtonAction"/></summary>
    public static void RegisterKeyListener(IInteractable interactable, List<ButtonAction> actionSlots)
    {
        foreach(var slot in actionSlots)
        {
            if (Instance.Listeners.ContainsKey(slot))
                Instance.Listeners[slot].Add(interactable);
            else
            {
                slot.Bind(GameManager.Instance.input.handle);
                Instance.Listeners.Add(slot, new List<IInteractable>(new[] { interactable }));
            }
        }
    }
    ///<summary>Remove a listener from a list of <see cref="ButtonAction"/></summary>
    public static void UnregisterKeyListener(IInteractable interactable, List<ButtonAction> actionSlots)
    {
        foreach(var slot in actionSlots)
        {
            if (!Instance.Listeners.ContainsKey(slot))
                continue;

            Instance.Listeners[slot].Remove(interactable);
        }
    }

    ///<summary>Add a listener to a list of <see cref="AxisAction"/></summary>
    public static void RegisterKeyListener(IInteractable interactable, List<AxisAction> actionSlots)
    {
        foreach (var slot in actionSlots)
        {
            if (Instance.Listeners.ContainsKey(slot))
                Instance.Listeners[slot].Add(interactable);
            else
            {
                slot.Bind(GameManager.Instance.input.handle);
                Instance.Listeners.Add(slot, new List<IInteractable>(new[] { interactable }));
            }
        }
    }
    ///<summary>Remove a listener from a list of <see cref="AxisAction"/></summary>
    public static void UnregisterKeyListener(IInteractable interactable, List<AxisAction> actionSlots)
    {
        foreach (var slot in actionSlots)
        {
            if (!Instance.Listeners.ContainsKey(slot))
                continue;

            Instance.Listeners[slot].Remove(interactable);
        }
    }
}
