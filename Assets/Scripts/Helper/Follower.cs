using UnityEngine;
using System.Collections;

/// <summary>
/// follow a given object
/// </summary>
public class Follower : MonoBehaviour
{
    public Transform m_tObject;

    // Update is called once per frame
    void Update()
    {
        transform.position = m_tObject.position;
      
    }
}
