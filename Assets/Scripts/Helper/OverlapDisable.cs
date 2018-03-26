﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OverlapDisable : MonoBehaviour
{
    public BoxCollider Trigger;
    public LayerMask Layer;
    public QueryTriggerInteraction TriggerInteraction;

    public UnityEvent OnOverlap, OnEmpty;

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Check())
            OnOverlap.Invoke();
        else
            OnEmpty.Invoke();
    }

    bool Check()
    {
        var colliders = Physics.OverlapBox(Trigger.transform.position, Trigger.transform.TransformVector(Trigger.size / 2), Trigger.transform.rotation, Layer, TriggerInteraction);
        foreach (var item in colliders)
        {
            if (!item.isTrigger)
                return true;
        }
        return false;
    }

    private void OnDrawGizmos()
    {
        if (Trigger == null)
            return;

        if (Check())
            Gizmos.color = Color.red;
        else
            Gizmos.color = Color.green;

        Matrix4x4 rotationMatrix = Matrix4x4.TRS(Trigger.transform.position, Trigger.transform.rotation, Trigger.transform.lossyScale);
        Gizmos.matrix = rotationMatrix;
        Gizmos.DrawWireCube(Trigger.center, Trigger.size);
    }
}