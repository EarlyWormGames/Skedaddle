﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PPObject : AttachableInteract
{
    [EnumFlag] public IgnoreAxis MaintainPosition = IgnoreAxis.Y | IgnoreAxis.Z;
    [EnumFlag] public IgnoreAxis MaintainRotation = IgnoreAxis.Everything;
    public List<Collider> TriggersToDisable = new List<Collider>();

    private Rigidbody rig;
    private bool isKinematic;
    private float mass, drag, angularDrag;
    private RigidbodyConstraints constraints;
    private RigidbodyInterpolation interpolation;
    private CollisionDetectionMode collisionDetectionMode;

    private bool waitOne = false;
    private Vector3 startPosition;
    private Vector3 startRotation;
    private Transform defaultParent;

    protected override void OnStart()
    {
        base.OnStart();
        
        BlocksMovement = false;
        BlocksTurn = true;

        rig = GetComponent<Rigidbody>();
        isKinematic = rig.isKinematic;
        mass = rig.mass;
        drag = rig.drag;
        angularDrag = rig.angularDrag;
        constraints = rig.constraints;
        interpolation = rig.interpolation;
        collisionDetectionMode = rig.collisionDetectionMode;

        defaultParent = transform.parent;
    }

    protected override void OnUpdate()
    {
        if (AttachedAnimal != null)
        {
            transform.position = IgnoreUtils.Calculate(MaintainPosition, startPosition, transform.position);
            transform.eulerAngles = IgnoreUtils.Calculate(MaintainPosition, startRotation, transform.eulerAngles);

            if (GameManager.mainMap.interact.wasJustPressed && !waitOne && AttachedAnimal.m_bSelected)
                Detach(this);

            waitOne = false;
        }
    }

    protected override void DoInteract(Animal caller)
    {
        if (caller.m_bTurning)
            return;

        if (!TryDetachOther())
            return;

        Attach(caller);

        startPosition = transform.position;
        startRotation = transform.eulerAngles;
        
        AttachedAnimal.m_bPullingObject = true;
        AttachedAnimal.OnPushChange();

        transform.SetParent(AttachedAnimal.m_tObjectHolder.transform, true);

        foreach (var trigger in TriggersToDisable)
            trigger.enabled = false;

        waitOne = true;

        Destroy(rig);
    }

    public void SetCollisions(Animal anim, bool allowed)
    {
        var cols = AttachedAnimal.GetComponentsInChildren<Collider>();
        foreach (var col in cols)
            Physics.IgnoreCollision(col, GetComponent<Collider>(), true);
    }

    protected override void OnDetach(Animal anim)
    {
        AttachedAnimal.m_bPullingObject = false;
        AttachedAnimal.OnPushChange();

        if (!BeingDestroyed)
        {
            if (defaultParent != null)
                transform.SetParent(defaultParent, true);
            else
                transform.parent = defaultParent;

            rig = gameObject.AddComponent<Rigidbody>();
            rig.isKinematic = isKinematic;
            rig.mass = mass;
            rig.drag = drag;
            rig.angularDrag = angularDrag;
            rig.constraints = constraints;
            rig.interpolation = interpolation;
            rig.collisionDetectionMode = collisionDetectionMode;

            foreach (var trigger in TriggersToDisable)
                trigger.enabled = true;

            AnimalsIn.RemoveAll(AttachedAnimal);
        }
    }
}