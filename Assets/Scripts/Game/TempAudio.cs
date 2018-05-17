using UnityEngine;
using System.Collections;

[System.Serializable]
public struct AudioTrack
{
    public string name;
    public AudioSource source;
    public float volumeTarget;
    public bool fadeOut;
}

/// <summary>
/// im 90% sure this class is gonna get re written
/// </summary>
public class TempAudio : Singleton<TempAudio>
{
    public AudioTrack[] m_aAnimalAudios;
    public AudioTrack[] m_aOtherAudio;
    public float m_fLerpSpeed = 1f;

    public int m_iSelectedIndex;

    // Update is called once per frame
    void Update()
    {
        if (m_aAnimalAudios == null)
            return;

        if (m_aAnimalAudios.Length == 0)
            return;

        for (int i = 0; i < m_aAnimalAudios.Length; ++i)
        {
            if (i == m_iSelectedIndex)
                m_aAnimalAudios[i].volumeTarget = 1f;
            else
                m_aAnimalAudios[i].volumeTarget = 0f;

            m_aAnimalAudios[i].source.volume = Mathf.Lerp(m_aAnimalAudios[i].source.volume, m_aAnimalAudios[i].volumeTarget, Time.deltaTime * m_fLerpSpeed);

            if (m_aAnimalAudios[i].fadeOut && m_aAnimalAudios[i].source.volume < 0.05f)
            {
                m_aAnimalAudios[i].fadeOut = false;
                m_aAnimalAudios[i].source.Stop();
            }
        }

        for (int i = 0; i < m_aOtherAudio.Length; ++i)
        {
            m_aOtherAudio[i].source.volume = Mathf.Lerp(m_aOtherAudio[i].source.volume, m_aOtherAudio[i].volumeTarget, Time.deltaTime * m_fLerpSpeed);

            if (m_aOtherAudio[i].fadeOut && m_aOtherAudio[i].source.volume < 0.05f)
            {
                m_aOtherAudio[i].fadeOut = false;
                m_aOtherAudio[i].source.Stop();
            }
        }
    }

    /// <summary>
    /// play audio at the start of the new level
    /// </summary>
    public void StartLevel()
    {
        if (m_aAnimalAudios == null)
            return;

        if (m_aAnimalAudios.Length == 0)
            return;

        foreach (var audio in m_aAnimalAudios)
        {
            if (!audio.source.isPlaying)
            {
                audio.source.volume = 0f;
                audio.source.Play();
            }
        }

        //Turn off the menu music
        m_aOtherAudio[0].volumeTarget = 0;
        m_aOtherAudio[0].fadeOut = true;
        m_aOtherAudio[1].volumeTarget = 1;
        m_aOtherAudio[2].volumeTarget = 1;

        if (!m_aOtherAudio[1].source.isPlaying)
            m_aOtherAudio[1].source.Play();

        if (!m_aOtherAudio[2].source.isPlaying)
            m_aOtherAudio[2].source.Play();
    }

    /// <summary>
    /// stop music when the level ends
    /// </summary>
    public void StopLevel()
    {
        if (m_aAnimalAudios.Length == 0)
            return;

        m_iSelectedIndex = -1;
        for (int i = 0; i < m_aAnimalAudios.Length; ++i)
        {
            m_aAnimalAudios[i].volumeTarget = 0f;
            m_aAnimalAudios[i].fadeOut = true;
        }

        m_aOtherAudio[1].volumeTarget = 0;
        m_aOtherAudio[2].volumeTarget = 0;

        m_aOtherAudio[1].fadeOut = true;
        m_aOtherAudio[2].fadeOut = true;
    }
}
