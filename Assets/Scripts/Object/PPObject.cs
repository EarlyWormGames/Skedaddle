﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PPObject : ActionObject
{
    public bool MaintainY = false;

    private Rigidbody rig;
    private bool isKinematic;
    private float mass, drag, angularDrag;
    private RigidbodyConstraints constraints;
    private RigidbodyInterpolation interpolation;
    private CollisionDetectionMode collisionDetectionMode;

    private List<Collider> triggers = new List<Collider>();
    private bool waitOne = false;
    private float startY;

    protected override void OnStart()
    {
        base.OnStart();
        m_CanBeDetached = true;
        m_CanDetach = false;
        m_bBlocksMovement = false;
        m_bBlocksTurn = true;

        rig = GetComponent<Rigidbody>();
        isKinematic = rig.isKinematic;
        mass = rig.mass;
        drag = rig.drag;
        angularDrag = rig.angularDrag;
        constraints = rig.constraints;
        interpolation = rig.interpolation;
        collisionDetectionMode = rig.collisionDetectionMode;

        Collider[] cols = GetComponentsInChildren<Collider>();
        foreach (var item in cols)
        {
            if (item.isTrigger)
                triggers.Add(item);
        }
    }

    protected override void OnUpdate()
    {
        if (m_aCurrentAnimal != null)
        {
            if (MaintainY)
            {
                var v3 = transform.position;
                v3.y = startY;
                transform.position = v3;
            }

            if (input.interact.wasJustPressed && !waitOne)
                Detach();
            waitOne = false;
        }
    }

    public override void DoAction()
    {
        if (!TryDetach())
            return;

        if (m_aCurrentAnimal != null)
        {
            return;
        }

        base.DoAction();
        startY = transform.position.y;

        m_aCurrentAnimal = Animal.CurrentAnimal;
        m_aCurrentAnimal.m_bPullingObject = true;
        m_aCurrentAnimal.m_oCurrentObject = this;

        transform.SetParent(m_aCurrentAnimal.transform, true);

        foreach (var trigger in triggers)
            trigger.enabled = false;

        waitOne = true;

        Destroy(rig);
    }

    public void SetCollisions(Animal anim, bool allowed)
    {
        var cols = m_aCurrentAnimal.GetComponentsInChildren<Collider>();
        foreach (var col in cols)
            Physics.IgnoreCollision(col, GetComponent<Collider>(), true);
    }

    public override void Detach()
    {
        base.Detach();
        m_aCurrentAnimal.m_bPullingObject = false;
        m_aCurrentAnimal.m_oCurrentObject = null;

        transform.parent = null;

        rig = gameObject.AddComponent<Rigidbody>();
        rig.isKinematic = isKinematic;
        rig.mass = mass;
        rig.drag = drag;
        rig.angularDrag = angularDrag;
        rig.constraints = constraints;
        rig.interpolation = interpolation;
        rig.collisionDetectionMode = collisionDetectionMode;

        foreach (var trigger in triggers)
            trigger.enabled = true;

        m_lAnimalsIn.Remove(m_aCurrentAnimal);
        m_aCurrentAnimal = null;
    }
}