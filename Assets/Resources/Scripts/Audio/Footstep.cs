using UnityEngine;
using System.Collections;

public class Footstep : MonoBehaviour
{
    public float m_fLifeTime = 1f;

    void Start()
    {
        //GetComponent<AudioObject>().SetParam("Index", Random.Range(0f, 4f));
    }

    // Update is called once per frame
    void Update()
    {
        m_fLifeTime -= Time.deltaTime;
        if (m_fLifeTime <= 0f)
        {
            Destroy(gameObject);
        }
    }
}
