using UnityEngine;
using System.Collections;

public class BreakableObject : MonoBehaviour
{
    //==================================
    //          Public Vars
    //==================================
    public GameObject m_goObject;
    public ParticleSystem m_psExplosion;
    public float m_fShardForce;

    //==================================
    //          Internal Vars
    //==================================

    //==================================
    //          Private Vars
    //================================== 
    private Rigidbody[] m_rShards;

    //Inherited functions

    void Start()
    {
        if (m_goObject != null)
        {
            m_rShards = m_goObject.GetComponentsInChildren<Rigidbody>();
        }
    }

    public void Break()
    {
        //This object should have the box collider on it
        foreach (Rigidbody x in m_rShards)
        {
            x.isKinematic = false;
            x.useGravity = true;
            x.GetComponent<Transform>().parent = null;
            x.GetComponent<Collider>().enabled = true;
            x.AddForce(Vector3.left * m_fShardForce, ForceMode.Impulse);
            x.AddForce(Vector3.forward * m_fShardForce, ForceMode.Impulse);
            x.Sleep();
        }
        m_psExplosion.Play();
        Destroy(gameObject);
    }
}
