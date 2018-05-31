using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// apply shared forces to multiple rigidbodies.
/// </summary>
public class Sharder : MonoBehaviour
{
    public float m_fShardForce;
    private Rigidbody[] m_rShards;

    // Use this for initialization
    void Start()
    {
        m_rShards = GetComponentsInChildren<Rigidbody>();
    }

    public void DestroyShards()
    {
        //This object should have the box collider on it
        foreach (var shard in m_rShards)
        {
            shard.isKinematic = false;
            shard.useGravity = true;
            shard.GetComponent<Transform>().parent = null;
            shard.GetComponent<Collider>().enabled = true;
            shard.AddForce(Vector3.left * m_fShardForce, ForceMode.Impulse);
            shard.AddForce(Vector3.forward * m_fShardForce, ForceMode.Impulse);
            shard.Sleep();
        }
    }
}