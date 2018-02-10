using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PPObject : ActionObject
{
    private Rigidbody rig;

    protected override void OnStart()
    {
        base.OnStart();
        rig = GetComponent<Rigidbody>();
        rig.isKinematic = false;
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
            rig.isKinematic = false;

            cols = m_aCurrentAnimal.GetComponentsInChildren<Collider>();
            foreach (var col in cols)
                Physics.IgnoreCollision(col, GetComponent<Collider>(), false);
            
            m_aCurrentAnimal = null;
            return;
        }

        m_aCurrentAnimal = Animal.CurrentAnimal;
        m_aCurrentAnimal.m_bPullingObject = true;
        m_aCurrentAnimal.m_oCurrentObject = this;

        transform.SetParent(m_aCurrentAnimal.transform, true);
        rig.isKinematic = true;

        cols = m_aCurrentAnimal.GetComponentsInChildren<Collider>();
        foreach (var col in cols)
            Physics.IgnoreCollision(col, GetComponent<Collider>(), true);
    }
}