using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public static class CustomTools
{
    [MenuItem("Tools/Convert To PPObject")]
    public static void MakeObjectPP()
    {
        var go = Selection.activeGameObject;
        if (go == null)
            return;

        var pp = go.GetComponent<PPObject>();
        if (pp)
            return;

        pp = go.AddComponent<PPObject>();

        var rig = go.GetComponent<Rigidbody>();
        if (!rig)
            rig = go.AddComponent<Rigidbody>();
        rig.mass = 10;

        var box = go.AddComponent<BoxCollider>();
        box.isTrigger = true;
        pp.TriggersToDisable.Add(box);

        go.layer = LayerMask.NameToLayer("Trigger");
    }
}
