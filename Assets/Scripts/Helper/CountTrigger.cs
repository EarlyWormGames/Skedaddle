using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CountTrigger : MonoBehaviour
{
    public int RequiredCount = 1;
    [Tooltip("If disabled, will trigger every time Add() is called even if condition already met last Add() call")]
    public bool TriggerOnlyOnceMet = true;
    public UnityEvent OnCountMet, OnCountUnMet;

    private int CurrentCount;
    private bool triggered;

    public void Add()
    {
        ++CurrentCount;

        if (CurrentCount >= RequiredCount && !(triggered && TriggerOnlyOnceMet))
        {
            triggered = true;
            OnCountMet.Invoke();
        }
    }

    public void Remove()
    {
        --CurrentCount;

        if (CurrentCount < RequiredCount)
        {
            triggered = false;
            OnCountUnMet.Invoke();
        }
    }
}