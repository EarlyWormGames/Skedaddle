using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AnimationSetter : MonoBehaviour
{
    private Animator controller;
    private string nextEvent = "";

    // Use this for initialization
    void Start()
    {
        controller = GetComponent<Animator>();
    }

    public void SetBoolOn(string name)
    {
        controller.SetBool(name, true);
    }

    public void SetBoolOff(string name)
    {
        controller.SetBool(name, false);
    }

    public void SetNextParameter(string name)
    {
        nextEvent = name;
    }

    public void SetFloat(float value)
    {
        controller.SetFloat(nextEvent, value);
    }
}