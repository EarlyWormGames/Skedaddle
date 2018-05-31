using UnityEngine;
using UnityEngine.Events;
using System.Collections;


/// <summary>
/// Call an event when a breakable object "Breaks"
/// </summary>
public class BreakableObject : MonoBehaviour
{
    public UnityEvent OnBreak;

    public void Break()
    {
        OnBreak.Invoke();
        Destroy(gameObject);
    }
}
