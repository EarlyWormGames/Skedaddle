using UnityEngine;
using System.Collections;

public class SlidingDoor : ActionObject
{
    //PUBLIC VARS
    public bool m_bOpened;
    public float m_fOpenTime = 3.0f;
    public bool m_bHoldDoorOpen = false;


    //PRIVATE VARS
    private float m_fOpenTimeMax;
    private Animator m_aSlidingDoorAni;
    private float m_fSlidingDoorBlend = 0.0f;

    // Use this for initialization
    protected override void OnStart ()
    {
        m_aSlidingDoorAni = GetComponent<Animator>();
        //m_bOpened = false;
        m_fSlidingDoorBlend = 0.0f;
        m_fOpenTimeMax = m_fOpenTime;
    }
	
	// Update is called once per frame
	protected override void OnUpdate ()
    {
        if (m_aSlidingDoorAni != null)
        {
            if (m_bOpened)
            {
                m_aSlidingDoorAni.SetFloat("DoorState", m_fSlidingDoorBlend);
                m_fSlidingDoorBlend = m_fSlidingDoorBlend + (1.0f * Time.deltaTime);

                m_fOpenTime = m_fOpenTime - (1.0f * Time.deltaTime);
                if (m_fOpenTime < 0.0f)
                {
                    m_fOpenTime = m_fOpenTimeMax;
                    m_bOpened = false;
                }
            }
            else
            {
                m_aSlidingDoorAni.SetFloat("DoorState", m_fSlidingDoorBlend);
                m_fSlidingDoorBlend = m_fSlidingDoorBlend - (1.0f * Time.deltaTime);

            }

            if (m_bHoldDoorOpen)
            {
                m_aSlidingDoorAni.SetFloat("DoorState", m_fSlidingDoorBlend);
                m_fSlidingDoorBlend = m_fSlidingDoorBlend + (1.0f * Time.deltaTime);
            }
            else
            {
                m_aSlidingDoorAni.SetFloat("DoorState", m_fSlidingDoorBlend);
                m_fSlidingDoorBlend = m_fSlidingDoorBlend - (1.0f * Time.deltaTime);
            }

            if (m_fSlidingDoorBlend < 0.0f)
            {
                m_fSlidingDoorBlend = 0.0f;
            }

            if (m_fSlidingDoorBlend > 1.0f)
            {
                m_fSlidingDoorBlend = 1.0f;
            }
        }
    }
}
