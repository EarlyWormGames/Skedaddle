using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class AnimalHeadTrigger : MonoBehaviour
{
    [HideInNormalInspector]
    public Animal parent;
    private Collider col;

    void Awake()
    {
        parent = GetComponentInParent<Animal>();
        col = GetComponent<Collider>();
    }

    private void Update()
    {
        //col.enabled = parent.m_oCurrentObject == null;
    }
}