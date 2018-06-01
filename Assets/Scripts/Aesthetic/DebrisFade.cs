using UnityEngine;
using System.Collections;

/// <summary>
/// debris fade out.
/// </summary>
public class DebrisFade : MonoBehaviour
{
    public float m_fSpeed;
    public Material m_matFadeMaterial;

    private float m_fTimer;
    private Material m_realFadeMat;
    private bool done;

    // Use this for initialization
    void Start()
    {
        m_fTimer = -2;
    }

    // Update is called once per frame
    void Update()
    {
        if (GetComponent<Collider>().enabled)
        {
            m_fTimer += Time.deltaTime;
            if (m_fTimer > 0)
            {
                if (!done)
                {
                    GetComponent<MeshRenderer>().material = Instantiate(m_matFadeMaterial);
                    m_realFadeMat = GetComponent<MeshRenderer>().material;
                    done = true;
                }

                FadeOut();
            }
        }

    }

    void FadeOut()
    {
        Color newColor = m_realFadeMat.color;
        newColor.a = Mathf.Lerp(1, 0, m_fTimer * m_fSpeed);
        m_realFadeMat.SetColor("_Color", newColor);

        if (m_fTimer * m_fSpeed > 1)
        {
            Destroy(gameObject);
        }
    }
}
