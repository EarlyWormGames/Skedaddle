using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CombineMeshes
{
    [MenuItem("Tools/Combine Meshes")]
    public static void Combine()
    {
        var parent = new GameObject();
        parent.name = "Combined Mesh";

        Undo.RegisterCreatedObjectUndo(parent, "create parent mesh");

        Vector3 pos = parent.transform.position;
        parent.transform.position = Vector3.zero;

        MeshFilter filter = parent.AddComponent<MeshFilter>();
        MeshRenderer renderer = parent.AddComponent<MeshRenderer>();
        Mesh mesh = new Mesh();
        Material[] mats = new Material[0];

        var objs = Selection.gameObjects;

        List<CombineInstance> combine = new List<CombineInstance>();
        for (int i = 0; i < objs.Length; ++i)
        {
            var childFilter = objs[i].GetComponent<MeshFilter>();
            if (childFilter == null)
                continue;

            if (childFilter.sharedMesh != null)
            {
                mats = objs[i].GetComponent<MeshRenderer>().sharedMaterials;

                CombineInstance inst = new CombineInstance();
                inst.mesh = childFilter.sharedMesh;
                inst.transform = childFilter.transform.localToWorldMatrix;
                combine.Add(inst);

                Undo.RecordObject(objs[i], "disable child mesh");
                objs[i].SetActive(false);
            }
        }

        mesh.CombineMeshes(combine.ToArray());

        if (filter != null)
            filter.mesh = mesh;

        if (renderer != null)
        {
            renderer.sharedMaterials = mats;
        }

        parent.transform.position = pos;
    }
}