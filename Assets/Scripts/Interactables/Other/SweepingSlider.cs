using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// sweep test for sliding objects
/// 
/// figure out if an object can move without hitting something.
/// </summary>
public class SweepingSlider : SlidingObject
{
    public Rigidbody SweepBody;
    public Transform[] GrowingObjects;
    public float GrowMin = 1;
    public float GrowMax = 2;
    public bool UseSweep { get; set; }

    protected override void OnStart()
    {
        base.OnStart();
        UseSweep = true;
    }

    protected override bool DoSlide(float time)
    {
        if(GrowingObjects.Length != 0)
        {
            foreach(Transform x in GrowingObjects)
            {
                x.localScale = new Vector3(x.localScale.x, Mathf.Lerp(movingForward ? GrowMin : GrowMax, movingForward ? GrowMax : GrowMin, MovingCurve.Evaluate( time / Speed)), x.localScale.z);
            }
        }
        return base.DoSlide(time);
    }

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