using UnityEngine;
using UnityEditor;
using System.Collections;

[InitializeOnLoad]
public class ExtraHierarchy
{
    static bool m_Down;

    static ExtraHierarchy()
    {
        EditorApplication.hierarchyWindowItemOnGUI += Update;
    }

    static void Update(int id, Rect rect)
    {
        if (Event.current.control && Event.current.keyCode == KeyCode.G)
        {
            if (!m_Down)
            {
                m_Down = true;
                if (Selection.transforms.Length > 1)
                {
                    Transform previousParent = Selection.transforms[0].parent;
                    bool allSiblings = true;
                    foreach (Transform trans in Selection.transforms)
                    {
                        if(trans.parent != previousParent)
                        {
                            allSiblings = false;
                        }
                    }
                    GameObject parent = new GameObject();
                    Undo.RegisterCreatedObjectUndo(parent, "New Group");

                    parent.name = "New Group";
                    Vector3 pos = Vector3.zero;

                    foreach (Transform trans in Selection.transforms)
                    {
                        pos += trans.position;
                    }
                    pos /= Selection.transforms.Length;
                    parent.transform.position = pos;

                    foreach (Transform trans in Selection.transforms)
                    {
                        Undo.SetTransformParent(trans, parent.transform, "New group");
                    }
                    Selection.activeTransform = parent.transform;
                    if (allSiblings) parent.transform.parent = previousParent;
                }
            }
        }
        else
            m_Down = false;
    }
}
