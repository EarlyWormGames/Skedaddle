using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PPObject : ActionObject
{
    private Rigidbody rig;
    private float mass, drag, angularDrag;
    private RigidbodyConstraints constraints;
    private RigidbodyInterpolation interpolation;
    private CollisionDetectionMode collisionDetectionMode;

    private List<Collider> triggers = new List<Collider>();

    protected override void OnStart()
    {
        base.OnStart();
        rig = GetComponent<Rigidbody>();
        rig.isKinematic = false;
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

    public override void DoAction()
    {
        if (!TryDetach())
            return;

        if (m_aCurrentAnimal != null)
        {
            Detach();
            return;
        }

        base.DoAction();
        m_aCurrentAnimal = Animal.CurrentAnimal;
        m_aCurrentAnimal.m_bPullingObject = true;
        m_aCurrentAnimal.m_oCurrentObject = this;

        transform.SetParent(m_aCurrentAnimal.transform, true);

        foreach (var trigger in triggers)
            trigger.enabled = false;

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
        m_aCurrentAnimal.m_bPullingObject = false;
        m_aCurrentAnimal.m_oCurrentObject = null;

        transform.parent = null;

        rig = gameObject.AddComponent<Rigidbody>();
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