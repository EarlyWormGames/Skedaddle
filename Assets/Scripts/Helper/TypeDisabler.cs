using UnityEngine;
using System;

public class TypeDisabler : MonoBehaviour
{
    public Type m_tRemove;
    public Collider[] m_acAcceptedColliders;

    void Start()
    {
        m_tRemove = typeof(MovingObject);
    }

    void OnTriggerEnter(Collider a_col)
    {
        if (a_col.GetComponent(m_tRemove) == null)
            return;

        for (int i = 0; i < m_acAcceptedColliders.Length; ++i)
        {
            if (a_col == m_acAcceptedColliders[i])
            {
                (a_col.GetComponent(m_tRemove) as MonoBehaviour).enabled = false;
            }
        }
    }
}
