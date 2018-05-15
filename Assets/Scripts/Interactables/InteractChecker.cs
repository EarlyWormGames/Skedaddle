using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputNew;
using UnityEngine.SceneManagement;

/// <summary>
/// any interactions with objects the player can have is handled here
/// </summary>
public class InteractChecker : Singleton<InteractChecker>
{
    private class KeyListener
    {
        public bool wasPressed;
        public bool firstPress;
        public bool consumed;
        public List<IInteractable> listeners;

        public KeyListener(List<IInteractable> list)
        {
            listeners = list;
        }
    }

    private Dictionary<InputControl, KeyListener> Listeners = new Dictionary<InputControl, KeyListener>();

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
            if(pair.Key.isEnabled && pair.Key.rawValue != 0)
            {
                if (pair.Value.consumed)
                {
                    pair.Value.firstPress = false;
                    continue;
                }

                pair.Value.wasPressed = true;
                pair.Value.firstPress = true;

                float distance = Mathf.Infinity;
                IInteractable current = null;

                //Loop through all of the interactables that listen to this key
                foreach(var interactable in pair.Value.listeners)
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
                {
                    current.Interact(Animal.CurrentAnimal);
                    pair.Value.consumed = true;
                }
            }
            else
            {
                pair.Value.wasPressed = false;
                pair.Value.firstPress = false;
                pair.Value.consumed = false;
            }
        }
    }

    ///<summary>Add a listener to a list of strings, which relate to <see cref="GameManager.controlsList"/></summary>
    public static void RegisterKeyListener(IInteractable interactable, List<string> actionSlots)
    {
        if (GameManager.Instance == null || GameManager.Instance.input == null)
            return;

        foreach (var slot in actionSlots)
        {
            if (Instance.Listeners.ContainsKey(GameManager.Instance.controlsList[slot]))
                Instance.Listeners[GameManager.Instance.controlsList[slot]].listeners.Add(interactable);
            else
            {
                Instance.Listeners.Add(GameManager.Instance.controlsList[slot], new KeyListener(new List<IInteractable>(new[] { interactable })));
            }
        }
    }
    ///<summary>Remove a listener from a list of strings, which relate to <see cref="GameManager.controlsList"/></summary>
    public static void UnregisterKeyListener(IInteractable interactable, List<string> actionSlots)
    {
        if (GameManager.Instance == null || GameManager.Instance.input == null)
            return;

        foreach (var slot in actionSlots)
        {
            if (!Instance.Listeners.ContainsKey(GameManager.Instance.controlsList[slot]))
                continue;

            Instance.Listeners[GameManager.Instance.controlsList[slot]].listeners.Remove(interactable);
        }
    }

    public bool WasKeyConsumed(string name)
    {
        if (!Instance.Listeners.ContainsKey(GameManager.Instance.controlsList[name]))
            return false;

        var item = Instance.Listeners[GameManager.Instance.controlsList[name]];
        return item.consumed;
    }
}
