using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SweepingSlider : SlidingObject
{
    public Rigidbody SweepBody;
    public bool UseSweep { get; set; }

    protected override bool DoMove(Vector3 position)
    {
        if (SweepBody == null || !UseSweep)
        {
            transform.position = position;
            return true;
        }
        else
        {
            Vector3 dir = position - transform.position;
            RaycastHit hit;
            if (SweepBody.SweepTest(dir.normalized, out hit, dir.magnitude))
                return false;

            transform.position = position;
            return true;
        }
    }
}