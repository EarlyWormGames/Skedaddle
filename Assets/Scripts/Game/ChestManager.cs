using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

[CreateAssetMenu(fileName = "Chest Manager", menuName = "Chest Manager")]
public class ChestManager : ScriptableObject
{
    [HideInInspector]
    public List<ExposedReference<Chest>> chests = new List<ExposedReference<Chest>>();

    public bool AddChest(Chest item)
    {
        var resolver = FindObjectOfType<Referencer>();
        if (resolver == null)
        {
            resolver = new GameObject("Reference Resolver").AddComponent<Referencer>();
            Add(item, resolver);
            return true;
        }

        if (resolver.HasReference(item))
        {
            if (item.GUID > chests.Count)
            {
                Add(item, resolver);
                return true;
            }
            bool valid = false;
            var val = resolver.GetReferenceValue(chests[item.GUID - 1].exposedName, out valid);
            if (val == null)
            {
                Add(item, resolver);
                return true;
            }
            else if (val != item)
            {
                Add(item, resolver);
                return true;
            }
            return false;
        }

        Add(item, resolver);
        return true;
    }

    void Add(Chest item, Referencer resolver)
    {
        var exref = new ExposedReference<Chest>();
        resolver.SetReferenceValue(exref.exposedName, item);
        chests.Add(exref);
    }
}