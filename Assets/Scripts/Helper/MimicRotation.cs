using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MimicRotation : MonoBehaviour
{
    public Transform m_ToMimic;
    public Vector3 m_CopyAxes = new Vector3(0, 0, 1);

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (m_ToMimic == null)
            return;

        Vector3 rot = transform.rotation.eulerAngles;
        if (m_CopyAxes.x > 0)
        {
            rot.x = m_ToMimic.rotation.eulerAngles.x;
        }
        if (m_CopyAxes.x > 0)
        {
            rot.y = m_ToMimic.rotation.eulerAngles.y;
        }
        if (m_CopyAxes.x > 0)
        {
            rot.z = m_ToMimic.rotation.eulerAngles.z;
        }
        transform.rotation = Quaternion.Euler(rot);
    }
}
