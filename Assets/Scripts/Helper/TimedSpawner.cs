using UnityEngine;
using System.Collections;

/// <summary>
/// create an object after a certian time.
/// </summary>
public class TimedSpawner : MonoBehaviour
{
    public float m_fCountTime = 1f;
    public GameObject m_gPrefab;
    public Transform m_tSpawnPoint;

    private bool m_bPlaying;
    private float m_fTimer = 0f;

    // Update is called once per frame
    void Update()
    {
        if (m_bPlaying)
        {
            if (m_fTimer > 0f)
            {
                m_fTimer -= Time.deltaTime;
            }
            else
            {
                GameObject obj = Instantiate(m_gPrefab);
                obj.transform.position = m_tSpawnPoint == null ? transform.position : m_tSpawnPoint.position;
                StopTimer();
            }
        }
    }

    public void StartTimer()
    {
        m_bPlaying = true;
        m_fTimer = m_fCountTime;
    }

    public void StopTimer()
    {
        m_bPlaying = false;
        m_fTimer = m_fCountTime;
    }

    public void PauseTimer()
    {
        m_bPlaying = false;
    }
}
