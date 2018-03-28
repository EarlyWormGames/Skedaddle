using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PPObject : ActionObject
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

        defaultParent = transform.parent;
    }

    protected override void OnUpdate()
    {
        if (m_aCurrentAnimal != null)
        {
            transform.position = IgnoreUtils.Calculate(MaintainPosition, startPosition, transform.position);
            transform.eulerAngles = IgnoreUtils.Calculate(MaintainPosition, startRotation, transform.eulerAngles);

            if (input.interact.wasJustPressed && !waitOne && m_aCurrentAnimal.m_bSelected)
                Detach(m_aCurrentAnimal);

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

        if (Animal.CurrentAnimal.m_bTurning)
            return;

        base.DoAction();

        startPosition = transform.position;
        startRotation = transform.eulerAngles;

        m_aCurrentAnimal = Animal.CurrentAnimal;
        m_aCurrentAnimal.m_bPullingObject = true;
        m_aCurrentAnimal.m_oCurrentObject = this;
        m_aCurrentAnimal.OnPushChange();

        transform.SetParent(m_aCurrentAnimal.transform, true);

        foreach (var trigger in TriggersToDisable)
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

    public override void Detach(Animal anim, bool destroy = false)
    {
        base.Detach(anim, destroy);
        m_aCurrentAnimal.m_bPullingObject = false;
        m_aCurrentAnimal.m_oCurrentObject = null;
        m_aCurrentAnimal.OnPushChange();

        if (!destroy)
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

            m_lAnimalsIn.RemoveAll(m_aCurrentAnimal);
            m_aCurrentAnimal = null;
        }
    }

    protected override void AnimalExit(Animal a_animal)
    {
        base.AnimalExit(a_animal);
    }
}