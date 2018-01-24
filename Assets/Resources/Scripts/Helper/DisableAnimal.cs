using UnityEngine;
using System.Collections;

public class DisableAnimal : MonoBehaviour
{
    public Animal[] m_aDisables;

    // Update is called once per frame
    void Awake()
    {
        for (int i = 0; i < m_aDisables.Length; ++i)
        {
            if (m_aDisables[i] != null)
            {
                m_aDisables[i].m_bCanBeSelected = false;
            }
        }
        Destroy(this);
    }
}
