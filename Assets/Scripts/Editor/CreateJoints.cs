using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CreateJoints
{
    [MenuItem("Tools/Hinge/Hinge Joints on Children")]
    public static void HingesOnChildren()
    {
        if (Selection.activeGameObject == null)
            return;

        GameObject parent = Selection.activeGameObject;
        List<GameObject> children = new List<GameObject>();
        foreach (Transform child in parent.transform)
        {
            children.Add(child.gameObject);
        }

        AddHinges(children.ToArray());
    }

    [MenuItem("Tools/Hinge/Hinge Joints on Children Reverse")]
    public static void HingesOnChildrenReverse()
    {
        if (Selection.activeGameObject == null)
            return;

        GameObject parent = Selection.activeGameObject;
        List<GameObject> children = new List<GameObject>();
        foreach (Transform child in parent.transform)
        {
            children.Add(child.gameObject);
        }

        children.Reverse();
        AddHinges(children.ToArray());
    }

    [MenuItem("Tools/Hinge/Rope Joints on Children Reverse")]
    public static void RopeOnChildrenReverse()
    {
        if (Selection.activeGameObject == null)
            return;

        GameObject parent = Selection.activeGameObject;
        List<GameObject> children = new List<GameObject>();
        foreach (Transform child in parent.transform)
        {
            children.Add(child.gameObject);
        }

        children.Reverse();
        AddRopeConfigurable(children.ToArray());
    }

    [MenuItem("Tools/Hinge/Character Joints on Children Reverse")]
    public static void CharacterOnChildReverse()
    {
        if (Selection.activeGameObject == null)
            return;

        GameObject parent = Selection.activeGameObject;
        List<GameObject> children = new List<GameObject>();
        foreach (Transform child in parent.transform)
        {
            children.Add(child.gameObject);
        }

        children.Reverse();
        AddCharacterJoints(children.ToArray());
    }

    static void AddHinges(GameObject[] objects)
    {
        Rigidbody previous = null;
        foreach (var child in objects)
        {
            Undo.RegisterCompleteObjectUndo(child.gameObject, "Hinges");

            var body = child.GetComponent<Rigidbody>();
            if (body == null)
                body = child.AddComponent<Rigidbody>();
            var hinge = child.GetComponent<HingeJoint>();
            if (hinge == null)
                hinge = child.AddComponent<HingeJoint>();

            if (previous != null)
            {
                hinge.connectedBody = previous;
            }

            previous = body;
        }
    }

    static void AddRopeConfigurable(GameObject[] objects)
    {
        Rigidbody previous = null;
        foreach (var child in objects)
        {
            Undo.RegisterCompleteObjectUndo(child.gameObject, "Hinges");

            var body = child.GetComponent<Rigidbody>();
            if (body == null)
                body = child.AddComponent<Rigidbody>();
            var hinge = child.GetComponent<ConfigurableJoint>();
            if (hinge == null)
                hinge = child.AddComponent<ConfigurableJoint>();

            if (previous != null)
            {
                hinge.connectedBody = previous;
            }
            hinge.xMotion = ConfigurableJointMotion.Limited;
            hinge.yMotion = ConfigurableJointMotion.Limited;
            hinge.zMotion = ConfigurableJointMotion.Limited;

            previous = body;
        }
    }

    static void AddCharacterJoints(GameObject[] objects)
    {
        Rigidbody previous = null;
        foreach (var child in objects)
        {
            Undo.RegisterCompleteObjectUndo(child.gameObject, "Hinges");

            var body = child.GetComponent<Rigidbody>();
            if (body == null)
                body = child.AddComponent<Rigidbody>();
            var hinge = child.GetComponent<CharacterJoint>();
            if (hinge == null)
                hinge = child.AddComponent<CharacterJoint>();

            if (previous != null)
            {
                hinge.connectedBody = previous;
            }
            hinge.axis = Vector3.one;
            hinge.swingAxis = Vector3.one;

            previous = body;
        }
    }
}