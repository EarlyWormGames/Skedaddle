using UnityEngine;
using System.Collections;

public class VideoPlayer : MonoBehaviour
{
    public bool m_bPlayOnAwake = false;

    private MovieTexture m_mTex;
    private AudioSource m_aSource;
    private int m_iWait;

    // Use this for initialization
    void Start()
    {
        m_mTex = GetComponent<Renderer>().material.mainTexture as MovieTexture;
        m_aSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        ++m_iWait;
        if (m_bPlayOnAwake && m_iWait > 2)
        {
            m_mTex.Play();
            m_aSource.Play();
            m_bPlayOnAwake = false;
        }
    }

    void OnDestroy()
    {
        if (m_mTex == null)
            return;

        m_mTex.Stop();
        m_aSource.Stop();
    }

    public void Play()
    {
        if (m_mTex == null)
            Start();

        m_mTex.Play();
        m_aSource.Play();
    }

    public void Stop()
    {
        if (m_mTex == null)
            Start();

        m_mTex.Stop();
        m_aSource.Stop();
    }
}
