using UnityEngine;
using System.Collections;

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
