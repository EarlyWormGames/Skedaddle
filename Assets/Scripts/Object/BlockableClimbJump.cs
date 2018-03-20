using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockableClimbJump : ClimbJump
{
    public Vector3 BoxPosition;
    public Vector3 BoxRotation;
    public Vector3 BoxScale;

    protected override bool TryClimb(Animal animal)
    {
        return true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.matrix = Matrix4x4.TRS(transform.TransformPoint(BoxPosition), transform.rotation * Quaternion.Euler(BoxRotation), transform.TransformVector(BoxScale));
        //Gizmos.col
        Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
    }
}