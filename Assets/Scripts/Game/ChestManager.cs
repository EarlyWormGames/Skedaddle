using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

[CreateAssetMenu(fileName = "Chest Manager", menuName = "Chest Manager")]
public class ChestManager : ScriptableObject
{
    [HideInNormalInspector]
    public List<ExposedReference<Chest>> chests = new List<ExposedReference<Chest>>();
    [SerializeField]
    public int ChestLength { get { return chests.Count; } }

    public bool AddChest(Chest item, out int i)
    {
        i = -1;

        var resolver = FindObjectOfType<Referencer>();
        if (resolver == null)
        {
            resolver = new GameObject("Reference Resolver").AddComponent<Referencer>();

#if UNITY_EDITOR
            UnityEditor.Undo.RegisterCreatedObjectUndo(resolver, "New resolver");
#endif

            Add(item, resolver, new PropertyName("Chest GUID: " + chests.Count.ToString()));
            i = chests.Count - 1;
            return true;
        }

        PropertyName oName;
        if (resolver.HasReference(item, out oName))
        {
            var exref = new ExposedReference<Chest>();
            exref.exposedName = new PropertyName(oName);

            int index = chests.IndexOf(exref);
            if (index < 0)
            {
                Add(item, resolver, exref.exposedName);
                i = chests.Count - 1;
                return true;
            }
            else
            {
                i = index;
                return true;
            }
        }

        Add(item, resolver, new PropertyName("Chest GUID: " + chests.Count.ToString()));
        i = chests.Count - 1;
        return true;
    }

    void Add(Chest item, Referencer resolver, PropertyName customName)
    {
        var exref = new ExposedReference<Chest>();
        exref.exposedName = customName;

#if UNITY_EDITOR
        UnityEditor.Undo.RecordObject(resolver, "Add reference to resolver");
#endif

        resolver.SetReferenceValue(exref.exposedName, item);
        chests.Add(exref);
    }
}