using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// menu windows manager
/// </summary>
public class TestingWindowManager : MonoBehaviour {

    public GameObject[] panels;
    public Toggle[] menus;

    private bool hasChanged = false;

    void LateUpdate()
    {
        if (hasChanged)
        {
            hasChanged = false;
        }
    }

    public void SwitchTo(bool isOn)
    {
        if (!hasChanged)
        {
            hasChanged = true;
            int i = Array.IndexOf(menus, EventSystem.current.currentSelectedGameObject.GetComponent<Toggle>());
            for(int x = 0; x < panels.Length; x++)
            {
                if(x == i)
                {
                    if (isOn)
                    {
                        panels[x].SetActive(true);
                    }
                    else
                    {
                        panels[x].SetActive(false);
                    }
                }
                else
                {
                    panels[x].SetActive(false);
                    menus[x].isOn = false;
                }
            }
            
        }
    }
}
