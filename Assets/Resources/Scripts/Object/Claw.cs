using UnityEngine;
using System.Collections;

public class Claw : MonoBehaviour
{
    public float m_fOpenDuration;

    internal Animator m_aAnimator;

    void Start()
    {
        m_aAnimator = GetComponent<Animator>();
        m_aAnimator.SetBool("Grabbing", true);
    }

    public void Open()
    {
        m_aAnimator.SetBool("Open", true);
        StartCoroutine(WaitClose());
    }

    IEnumerator WaitClose()
    {
        yield return new WaitForSeconds(m_fOpenDuration);
        m_aAnimator.SetBool("Closed", true);
    }
}
