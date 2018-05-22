using UnityEngine;
using System.Collections;

/// <summary>
/// destroy the rigid body on entering the trigger
/// Use:
/// attach to an object with a trigger. destroy rigidbody when the trigger event occurs
/// </summary>
public class RBodyRemover : MonoBehaviour
{
    public Collider[] m_acAcceptedColliders;

    void OnTriggerEnter(Collider a_col)
    {
        if (a_col.GetComponent<Rigidbody>() == null)
            return;

        for (int i = 0; i < m_acAcceptedColliders.Length; ++i)
        {
            if (a_col == m_acAcceptedColliders[i])
            {
                Destroy(a_col.GetComponent<Rigidbody>());
            }
        }
    }
}
