using UnityEngine;
using System.Collections;

/// <summary>
/// the exit/entry door of each level
/// </summary>
public class LevelDoor : MonoBehaviour
{
    public MovingObject m_Door;
    public float m_WaitTime = 1f;

    private float m_Timer = -1f;
    private bool m_DoOnce = true;

    // Update is called once per frame
    void Update()
    {
        if (m_DoOnce)
        {
            m_Door.Move(true);
            m_DoOnce = false;
        }

        if (m_Timer >= 0f)
        {
            m_Timer += Time.deltaTime;
            if (m_Timer >= m_WaitTime)
            {
                m_Timer = -1f;
                m_Door.Move(false);
                enabled = false;
            }
        }
    }

    public void SlideEnd()
    {
        m_Timer = 0f;
    }

    private void OnEnable()
    {
        if (m_Door != null)
            m_Door.ForwardEnd.AddListener(SlideEnd);
    }

    private void OnDisable()
    {
        if (m_Door != null)
            m_Door.ForwardEnd.RemoveListener(SlideEnd);
    }
}