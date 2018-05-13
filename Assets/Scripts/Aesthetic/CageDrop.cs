using UnityEngine;
using System.Collections;
using UnityEngine.InputNew;

public class CageDrop : MonoBehaviour, IInteractable
{
    public HingeJoint[] m_hjRope;
    public GameObject m_goBalancer;

    public Collider m_Floor;
    public Collider[] m_MyColliders = new Collider[0];
    public Collider[] m_CanCollide = new Collider[0];

    private Animator m_aAnim;
    private CageJitter m_cjEffects;
    private Rigidbody m_rBody;

    private bool m_bDropped = false;

    void Start()
    {
        m_aAnim = GetComponent<Animator>();
        m_aAnim.enabled = false;
        m_cjEffects = GetComponent<CageJitter>();
        m_rBody = GetComponent<Rigidbody>();
    }

    IEnumerator StopRig()
    {
        yield return new WaitForSeconds(2);

        m_rBody.isKinematic = true;
        m_rBody.useGravity = false;
    }

    void OnCollisionEnter(Collision a_col)
    {
    }

    public bool CheckInfo(InputControl input, Animal caller)
    {
        return true;
    }

    public bool IgnoreDistance()
    {
        return true;
    }

    public float GetDistance(Vector3 point)
    {
        return 0;
    }

    public void Interact(Animal caller)
    {
        if (!m_bDropped)
        {
            m_bDropped = true;

            m_aAnim.enabled = true;
            m_aAnim.SetBool("Release", true);
            if (GameTimer.InstanceExists())
                GameTimer.StartTimer();
        }
    }
}
