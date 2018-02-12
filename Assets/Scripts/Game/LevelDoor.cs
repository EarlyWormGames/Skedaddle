using UnityEngine;
using System.Collections;

public class LevelDoor : MonoBehaviour
{
    //public MovingObject m_Door;
    public float m_WaitTime = 1f;

    private float m_Timer = -1f;
    private bool m_DoOnce = true;

    // Update is called once per frame
    void Update()
    {
        if (m_DoOnce)
        {
            //m_Door.DoActionOn();
            //m_Door.m_OnMoveEnd.AddListener(SlideEnd);
            m_DoOnce = false;
        }

        if (m_Timer >= 0f)
        {
            m_Timer += Time.deltaTime;
            if (m_Timer >= m_WaitTime)
            {
                m_Timer = -1f;
                //m_Door.DoActionOff();
                enabled = false;
            }
        }
    }

    void SlideEnd()
    {
        m_Timer = 0f;
    }
}