using UnityEngine;
using System.Collections;

public class DebrisFade : MonoBehaviour {

    public Material m_matFadeMaterial;
    public float m_fSpeed;

    private float m_fTimer;

	// Use this for initialization
	void Start () {
        m_fTimer = -2;
	}
	
	// Update is called once per frame
	void Update () {
        if(GetComponent<Collider>().enabled)
        {
            m_fTimer += Time.deltaTime;
            if (m_fTimer > 0)
            {
                FadeOut();
            }
        }
	
	}

    void FadeOut()
    {
        GetComponent<MeshRenderer>().material = m_matFadeMaterial;

        Color newColor = m_matFadeMaterial.color;
        newColor.a = Mathf.Lerp(1, 0, m_fTimer * m_fSpeed);
        m_matFadeMaterial.SetColor("_Color", newColor);

        if(m_fTimer * m_fSpeed > 1)
        {
            Destroy(gameObject);
        }
    }
}
