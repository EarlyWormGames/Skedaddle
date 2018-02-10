using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PPObject : ActionObject
{
    public Collider Trigger;

    private Rigidbody rig;
    private float mass, drag, angularDrag;
    private RigidbodyConstraints constraints;
    private RigidbodyInterpolation interpolation;
    private CollisionDetectionMode collisionDetectionMode;

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
    }

    public override void DoAction()
    {
        if (Animal.CurrentAnimal.m_oCurrentObject != null && Animal.CurrentAnimal.m_oCurrentObject != this)
            return;

        Collider[] cols;
        if (m_aCurrentAnimal != null)
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
            Trigger.enabled = true;

            //SetCollisions(m_aCurrentAnimal, true);

            m_aCurrentAnimal = null;
            return;
        }

        m_aCurrentAnimal = Animal.CurrentAnimal;
        m_aCurrentAnimal.m_bPullingObject = true;
        m_aCurrentAnimal.m_oCurrentObject = this;

        transform.SetParent(m_aCurrentAnimal.transform, true);
        Trigger.enabled = false;

        Destroy(rig);

        //SetCollisions(m_aCurrentAnimal, false);
    }

    public void SetCollisions(Animal anim, bool allowed)
    {
        var cols = m_aCurrentAnimal.GetComponentsInChildren<Collider>();
        foreach (var col in cols)
            Physics.IgnoreCollision(col, GetComponent<Collider>(), true);
    }
}