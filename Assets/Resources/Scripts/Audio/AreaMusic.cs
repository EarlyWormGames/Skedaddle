using UnityEngine;
using System.Collections;
using FMODUnity;

public class AreaMusic : MonoBehaviour
{
    public StudioEventEmitter m_sMainMusic;

    public float m_fLerpSpeed = 1f;

    internal bool isPlaying
    {
        get
        {
            return m_bPlaying;
        }
    }

    private float m_fCurrentVal = 0f;
    private int m_iTargetIndex = 0;
    private bool m_bPlaying = false;

    // Update is called once per frame
    void Update()
    {
        if (m_sMainMusic != null)
        {
            m_fCurrentVal = Mathf.Lerp(m_fCurrentVal, m_iTargetIndex, Time.deltaTime * m_fLerpSpeed);
            m_sMainMusic.SetParameter("Index", m_fCurrentVal);

            if (!m_bPlaying)
            {
                if (m_sMainMusic.IsPlaying() && m_sMainMusic.volume <= 0f)
                {
                    m_sMainMusic.Stop();
                }
                else
                {
                    m_sMainMusic.volume -= Time.deltaTime * 8f;
                }
            }
            else
            {
                if (!m_sMainMusic.IsPlaying())
                    m_sMainMusic.Play();

                if (m_sMainMusic.volume < 10f)
                    m_sMainMusic.volume += Time.deltaTime * 8f;
                else
                    m_sMainMusic.volume = 10f;
            }
        }
    }

    public void SetIndex(int a_index)
    {
        m_iTargetIndex = a_index;
    }

    public void Play()
    {
        if (!m_sMainMusic.IsPlaying())
        {
            m_sMainMusic.Play();
            m_sMainMusic.volume = 0f;
        }
        m_bPlaying = true;
    }

    public void Stop()
    {
        m_bPlaying = false;
    }

    public void SetParam(string a_param, float a_val)
    {
        m_sMainMusic.SetParameter(a_param, a_val);
    }

    public void TriggerCue()
    {
        m_sMainMusic.TriggerCue();
    }
}
