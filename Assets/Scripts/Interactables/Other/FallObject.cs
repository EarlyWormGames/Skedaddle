using UnityEngine;
using System.Collections;

/// <summary>
/// call the interaction when an animal interacts with this object
/// </summary>
public class FallObject : AnimalInteractor
{
    public Rigidbody m_rBody;
    protected override bool HeadTriggerOnly { get; set; }

    protected override void DoInteract(Animal caller)
    {
        m_rBody.isKinematic = false;
    }
}
