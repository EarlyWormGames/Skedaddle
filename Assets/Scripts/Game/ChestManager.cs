using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

[Serializable]
public class ChestReference
{
    [SerializeField]
    public ExposedReference<Chest> reference;
}

[CreateAssetMenu(fileName = "Chest Manager", menuName = "Chest Manager")]
public class ChestManager : ScriptableObject
{
    [HideInInspector]
    public List<ChestReference> chests = new List<ChestReference>();
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

            Add(item, resolver, new PropertyName("Chest GUID: " + DateTime.Now.Ticks));
            i = chests.Count - 1;
            return true;
        }

        PropertyName oName;
        if (resolver.HasReference(item, out oName))
        {
            int index = IndexOf(oName);
            if (index < 0)
            {
                Add(item, resolver, oName);
                i = chests.Count - 1;
                return true;
            }
            else
            {
                i = index;
                return true;
            }
        }

        Add(item, resolver, new PropertyName("Chest GUID: " + DateTime.Now.Ticks));
        i = chests.Count - 1;
        return true;
    }

    public void Add(Chest item, Referencer resolver, PropertyName customName)
    {
        var exref = new ExposedReference<Chest>();
        exref.exposedName = customName;

#if UNITY_EDITOR
        UnityEditor.Undo.RecordObject(resolver, "Add reference to resolver");
        UnityEditor.EditorUtility.SetDirty(this);
#endif

        resolver.SetReferenceValue(exref.exposedName, item);
        chests.Add(new ChestReference() { reference = exref });

#if UNITY_EDITOR
        UnityEditor.AssetDatabase.SaveAssets();
#endif
    }

    int IndexOf(PropertyName name)
    {
        int index = 0;
        foreach (var item in chests)
        {
            if (item.reference.exposedName == name)
                return index;
            ++index;
        }
        return -1;
    }
}