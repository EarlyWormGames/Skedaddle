using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class CentreOfMass : MonoBehaviour
{
    public Vector3 Centre;
    private Rigidbody rig;

    // Update is called once per frame
    void Update()
    {
        if (rig == null)
            rig = GetComponent<Rigidbody>();

        rig.centerOfMass = Centre;
    }

    private void OnDrawGizmos()
    {
        Matrix4x4 rotate = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
        Gizmos.matrix = rotate;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(Centre, Centre + new Vector3(0.5f, 0, 0));

        Gizmos.color = Color.green;
        Gizmos.DrawLine(Centre, Centre + new Vector3(0, 0.5f, 0));

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(Centre, Centre + new Vector3(0, 0, 0.5f));
    }
}
