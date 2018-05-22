using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// how much of an object is inside a trigger and when to detach it from its 
/// </summary>
public class FallDetacher : MonoBehaviour
{
    public Attachable ToDetach;
    [Tooltip("Should the ActionObject also be disabled when it would be detached?")]
    public bool DisableWhenDetach = true;

    [Tooltip("What percentage of the box to be overlapping to detach")]
    public float DetachPercent = 10;

    public BoxCollider Trigger; 
    public LayerMask Layer;
    public QueryTriggerInteraction TriggerInteraction = QueryTriggerInteraction.Ignore;

    private void FixedUpdate()
    {
        DetachTest(true);
    }

    /// <summary>
    /// Returns the percentage of obj contained by region. Both obj and region are calculated as quadralaterals for performance purposes.
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="region"></param>
    /// <returns></returns>
    private float BoundsPercent(Bounds obj, Bounds region)
    {
        var total = 1f;

        for (var i = 0; i < 3; i++)
        {
            var dist = obj.min[i] > region.center[i] ?
                obj.max[i] - region.max[i] :
                region.min[i] - obj.min[i];

            total *= Mathf.Clamp01(1f - dist / obj.size[i]);
        }

        return total;
    }

    float DetachTest(bool detach = true)
    {
        if (Trigger == null)
            return 100;

        List<Collider> cols = new List<Collider>(Physics.OverlapBox(Trigger.transform.position, Trigger.transform.TransformVector(Trigger.size / 2), Trigger.transform.rotation, Layer, TriggerInteraction));
        for(int i = 0; i < cols.Count; ++i)
        {
            var item = cols[i];
            if (transform.parent != null)
            {
                if (item.transform == transform.parent || item.transform.parent == transform.parent)
                {
                    cols.RemoveAt(i);
                    --i;
                    continue;
                }
            }
            else
            {
                if (item.transform == transform || item.transform.parent == transform)
                {
                    cols.RemoveAt(i);
                    --i;
                    continue;
                }
            }
        }

        Bounds b = new Bounds();
        if (cols.Count > 0)
            b = cols[0].bounds;

        for (int i = 1; i < cols.Count; ++i)
        {
            b.Encapsulate(cols[i].bounds);
        }

        Bounds box = Trigger.bounds;

        if (DisableWhenDetach)
            ToDetach.enabled = true;

        float percent = BoundsPercent(box, b);
        if (percent * 100 < DetachPercent && detach)
        {
            if (ToDetach.AttachedAnimal != null)
                ToDetach.Detach(null);
            if (DisableWhenDetach)
                ToDetach.enabled = false;
        }
        return percent * 100;
    }

    private void OnDrawGizmos()
    {
        if (Trigger == null)
            return;

        Gizmos.color = Color.green;

        float percent = DetachTest(false);
        if (percent < DetachPercent)
        {
            Gizmos.color = Color.red;
        }

        Matrix4x4 rotationMatrix = Matrix4x4.TRS(Trigger.transform.position, Trigger.transform.rotation, Trigger.transform.lossyScale);
        Gizmos.matrix = rotationMatrix;
        Gizmos.DrawWireCube(Trigger.center, Trigger.size);
    }
}
