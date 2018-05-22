using UnityEngine;
using System.Collections;

public class FallObject : AnimalInteractor
{
    public Rigidbody m_rBody;
    protected override bool HeadTriggerOnly { get; set; }

    protected override void DoInteract(Animal caller)
    {
        m_rBody.isKinematic = false;
    }
}
