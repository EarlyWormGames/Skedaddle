using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockableClimbJump : ClimbJump
{
    public Vector3 BoxPosition;
    public Vector3 BoxRotation;
    public Vector3 BoxScale;
    public LayerMask Layer;

    protected override bool TryClimb(Animal animal)
    {
        return !BoxCheck();
    }

    /// <summary>
    /// Check if the box with the supplied settings is overlapping anything
    /// </summary>
    bool BoxCheck()
    {
        Vector3 pos = transform.TransformPoint(BoxPosition);
        Quaternion rot = transform.rotation * Quaternion.Euler(BoxRotation);
        Vector3 scale = transform.TransformVector(BoxScale).Abs();

        //Do a physics overlap check
        var cols = Physics.OverlapBox(pos, scale, rot, Layer, QueryTriggerInteraction.Ignore);
        if (cols.Length > 0)
        {
            foreach (var col in cols)
            {
                //Ignore climbjump scripts
                if (col.attachedRigidbody != null)
                {
                    if (col.attachedRigidbody.GetComponent<ClimbJump>())
                        continue;
                }
                else if (col.GetComponent<ClimbJump>())
                    continue;
                //We hit something
                return true;
            }
        }
        //Nothing was hit, good to go!
        return false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.matrix = Matrix4x4.TRS(transform.TransformPoint(BoxPosition), transform.rotation * Quaternion.Euler(BoxRotation), transform.TransformVector(BoxScale) * 2);
        if (BoxCheck())
            Gizmos.color = Color.red;
        else
            Gizmos.color = Color.green;
        Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
    }
}