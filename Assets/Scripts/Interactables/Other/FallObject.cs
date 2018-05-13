using UnityEngine;
using System.Collections;

public class FallObject : MonoInteracter
{
    public Rigidbody m_rBody;

    protected override void DoInteract(Animal caller)
    {
        m_rBody.isKinematic = false;
    }
}
