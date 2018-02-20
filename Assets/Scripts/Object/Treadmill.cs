using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Treadmill : MonoBehaviour
{
    public Vector3 force;
    public bool Active = true;
    public Animator m_Animator;
    public float AnimSpeed = 1;
    public float TransitionTime = 2;
    public AnimationCurve TransitionCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private List<Rigidbody> bodiesIn = new List<Rigidbody>();
    private float timer;
    private bool wasActive = true;

    private void OnTriggerEnter(Collider other)
    {
        Rigidbody rig = other.GetComponentInParent<Rigidbody>();

        if (!bodiesIn.Contains(rig))
            bodiesIn.Add(rig);
    }

    private void OnTriggerExit(Collider other)
    {
        Rigidbody rig = other.GetComponentInParent<Rigidbody>();

        if (bodiesIn.Contains(rig))
            bodiesIn.Remove(rig);
    }

    private void Update()
    {
        if ((Active && !wasActive) ||
            (!Active && wasActive))
        {
            wasActive = Active;
            timer = TransitionTime - timer;
        }

        timer = Mathf.Clamp(timer + Time.deltaTime, 0, TransitionTime);

        float t = TransitionCurve.Evaluate(timer / TransitionTime);
        float start = Active ? 0 : AnimSpeed;
        float end = !Active ? 0 : AnimSpeed;

        m_Animator.SetFloat("Speed", Mathf.Lerp(start, end, t));

        Vector3 vStart = Active ? Vector3.zero : force;
        Vector3 vEnd = !Active ? Vector3.zero : force;
        Vector3 pushForce = Vector3.Lerp(vStart, vEnd, t);

        foreach (var item in bodiesIn)
        {
            item.transform.position += (pushForce * Time.deltaTime);
        }
    }

    public void SetActive(bool active)
    {
        Active = active;
    }
}