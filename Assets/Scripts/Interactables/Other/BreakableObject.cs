using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class BreakableObject : MonoBehaviour
{
    public UnityEvent OnBreak;

    public void Break()
    {
        OnBreak.Invoke();
        Destroy(gameObject);
    }
}
