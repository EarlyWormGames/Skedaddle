using UnityEngine;
using System;
using System.Collections;

public enum LightPattern
{
    AllBlinking = 0,
    Alternating = 1,
    AllFade = 2,
    AlternFade = 3,
    OneWorm = 4,
    MultiWorm = 5,
    MiddleToSides = 6,
    SidesToMiddle = 7,
    MiddleSideWave = 8,
    TopToBottom = 9,
    BottomToTop = 10,
    Random
}

public class TitleLights : MonoBehaviour {

    public LightPattern m_enPattern;
    public float m_fSpeed;
    public float m_fPatternChangeSpeed;
    public float m_fFadeMaxIntesity;
    public int m_iWormLength;
    public int m_iWormNumber;
    public Light[] m_lTopSignLights;
    public Light[] m_lMiddleSignLights;
    public Light[] m_lBottomSignLights;

    private float m_fTimer;
    private float m_fRandomTimer;
    private LightPattern m_enRealPattern;
    private int m_iCycleNumber;
    private bool m_bReverse;

    void Start ()
    {
	    if(m_enPattern != LightPattern.Random)
        {
            m_enRealPattern = m_enPattern;
        }

	}
	
	void Update ()
    {

        if (m_enRealPattern != LightPattern.AllFade && m_enRealPattern != LightPattern.AlternFade && m_enPattern != LightPattern.Random)
        {
            m_fTimer -= Time.deltaTime;
            if (m_fTimer < 0)
            {
                switch (m_enPattern)
                {
                    case LightPattern.AllBlinking:
                        Light[] lightsOn = new Light[m_lTopSignLights.Length + m_lMiddleSignLights.Length + m_lBottomSignLights.Length];
                        Light[] lightsOff = new Light[0];
                        Array.Copy(m_lTopSignLights, lightsOn, m_lTopSignLights.Length);
                        Array.Copy(m_lMiddleSignLights, 0, lightsOn, m_lTopSignLights.Length, m_lMiddleSignLights.Length);
                        Array.Copy(m_lBottomSignLights, 0, lightsOn, m_lTopSignLights.Length + m_lMiddleSignLights.Length, m_lBottomSignLights.Length);
                        BlinkLights(lightsOn, lightsOff, m_bReverse);
                        m_bReverse = !m_bReverse;
                        break;
                    case LightPattern.Alternating:
                        lightsOn = new Light[m_lTopSignLights.Length + m_lMiddleSignLights.Length + m_lBottomSignLights.Length];
                        lightsOff = new Light[m_lTopSignLights.Length + m_lMiddleSignLights.Length + m_lBottomSignLights.Length];
                        Array.Copy(m_lTopSignLights, lightsOn, m_lTopSignLights.Length);
                        Array.Copy(m_lMiddleSignLights, 0, lightsOn, m_lTopSignLights.Length, 1);
                        Array.Copy(m_lMiddleSignLights, 1, lightsOn, m_lTopSignLights.Length + m_lMiddleSignLights.Length + m_lBottomSignLights.Length - 1, 1);
                        Array.Copy(m_lBottomSignLights, 0, lightsOn, m_lTopSignLights.Length + 1, m_lBottomSignLights.Length);
                        Array.Copy(m_lTopSignLights, lightsOff, m_lTopSignLights.Length);
                        Array.Copy(m_lMiddleSignLights, 0, lightsOff, m_lTopSignLights.Length, 1);
                        Array.Copy(m_lMiddleSignLights, 1, lightsOff, m_lTopSignLights.Length + m_lMiddleSignLights.Length + m_lBottomSignLights.Length - 1, 1);
                        Array.Copy(m_lBottomSignLights, 0, lightsOff, m_lTopSignLights.Length + 1, m_lBottomSignLights.Length);
                        for (int i = 1; i < lightsOn.Length; i = i + 2)
                        {
                            lightsOn[i] = null;
                        }
                        for (int i = 0; i < lightsOff.Length; i = i + 2)
                        {
                            lightsOff[i] = null;
                        }
                        BlinkLights(lightsOn, lightsOff, m_bReverse);
                        m_bReverse = !m_bReverse;
                        break;
                    case LightPattern.BottomToTop:
                        if (m_iCycleNumber == 1)
                        {
                            BlinkLights(m_lBottomSignLights, m_lTopSignLights, false);
                        }
                        else if (m_iCycleNumber == 2)
                        {
                            BlinkLights(m_lMiddleSignLights, m_lBottomSignLights, false);
                        }
                        else if (m_iCycleNumber == 3)
                        {
                            BlinkLights(m_lTopSignLights, m_lMiddleSignLights, false);
                            m_iCycleNumber = 0;
                        }
                        else
                        {
                            m_iCycleNumber = 0;
                        }
                        m_iCycleNumber++;
                        break;
                    case LightPattern.MiddleSideWave:

                        break;
                    case LightPattern.MiddleToSides:

                        break;
                    case LightPattern.MultiWorm:

                        break;
                    case LightPattern.OneWorm:
                        lightsOn = new Light[m_lTopSignLights.Length + m_lMiddleSignLights.Length + m_lBottomSignLights.Length];
                        lightsOff = new Light[m_lTopSignLights.Length + m_lMiddleSignLights.Length + m_lBottomSignLights.Length];
                        Array.Copy(m_lTopSignLights, lightsOn, m_lTopSignLights.Length);
                        Array.Copy(m_lMiddleSignLights, 0, lightsOn, m_lTopSignLights.Length, 1);
                        Array.Copy(m_lMiddleSignLights, 1, lightsOn, m_lTopSignLights.Length + m_lMiddleSignLights.Length + m_lBottomSignLights.Length - 1, 1);
                        Array.Copy(m_lBottomSignLights, 0, lightsOn, m_lTopSignLights.Length + 1, m_lBottomSignLights.Length);
                        Array.Copy(m_lTopSignLights, lightsOff, m_lTopSignLights.Length);
                        Array.Copy(m_lMiddleSignLights, 0, lightsOff, m_lTopSignLights.Length, 1);
                        Array.Copy(m_lMiddleSignLights, 1, lightsOff, m_lTopSignLights.Length + m_lMiddleSignLights.Length + m_lBottomSignLights.Length - 1, 1);
                        Array.Copy(m_lBottomSignLights, 0, lightsOff, m_lTopSignLights.Length + 1, m_lBottomSignLights.Length);
                        for (int i = 0; i < lightsOn.Length; i++)
                        {
                            if (m_iCycleNumber < m_iWormLength)
                            {
                                if (i > lightsOn.Length - m_iWormLength + m_iCycleNumber)
                                {
                                    lightsOff[i] = null;
                                }
                                else if (i <= m_iCycleNumber)
                                {
                                    lightsOff[i] = null;
                                }
                                else
                                {
                                    lightsOn[i] = null;
                                }
                            }
                            else
                            {
                                if(i <= m_iCycleNumber && i > m_iCycleNumber - m_iWormLength)
                                {
                                    lightsOff[i] = null;
                                }
                                else
                                {
                                    lightsOn[i] = null;
                                }
                            }
                        }
                        BlinkLights(lightsOn, lightsOff, false);
                        if(m_iCycleNumber < lightsOn.Length - 1)
                        {
                            m_iCycleNumber++;
                        }
                        else
                        {
                            m_iCycleNumber = 0;
                        }
                        break;
                    case LightPattern.SidesToMiddle:

                        break;
                    case LightPattern.TopToBottom:
                        if (m_iCycleNumber == 1)
                        {
                            BlinkLights(m_lTopSignLights, m_lBottomSignLights, false);
                        }
                        else if (m_iCycleNumber == 2)
                        {
                            BlinkLights(m_lMiddleSignLights, m_lTopSignLights, false);
                        }
                        else if (m_iCycleNumber == 3)
                        {
                            BlinkLights(m_lBottomSignLights, m_lMiddleSignLights, false);
                            m_iCycleNumber = 0;
                        }
                        else
                        {
                            m_iCycleNumber = 0;
                        }
                        m_iCycleNumber++;

                        break;
                }
                m_fTimer = m_fSpeed;
            }
        }
        else if (m_enPattern != LightPattern.Random)
        {
            m_fTimer += Time.deltaTime * m_fSpeed;
            if (m_enRealPattern == LightPattern.AllFade)
            {

            }
            else
            {

            }
        }
            if (m_enPattern == LightPattern.Random)
            {
                m_fRandomTimer -= Time.deltaTime;
                if (m_fRandomTimer < 0)
                {
                    int newPattern = Mathf.RoundToInt(UnityEngine.Random.Range(-0.49f, 10.49f));
                    m_enRealPattern = (LightPattern)newPattern;
                    m_fRandomTimer = m_fPatternChangeSpeed;
                }
            }
        }
	

    void BlinkLights(Light[] lightsOn, Light[] lightsOff, bool reverse)
    {
        for(int i = 0; i < lightsOn.Length; i++)
        {
            if (lightsOn[i] != null)
            {
                lightsOn[i].enabled = reverse ? false : true;
            }
        }
        for (int i = 0; i < lightsOff.Length; i++)
        {
            if (lightsOff[i] != null)
            {
                lightsOff[i].enabled = reverse ? true : false;
            }
        }
    }

    void FadeLights(float t, float intensity, Light[] groupA, Light[] groupB)
    {
        for(int i = 0; i < groupA.Length; i++)
        {
            groupA[i].intensity = intensity * (Mathf.Sin(m_fTimer) + 1f);
        }

        for (int i = 0; i < groupB.Length; i++)
        {
            groupB[i].intensity = intensity * (Mathf.Sin(m_fTimer - 0.5f) + 1f);
        }
    }
}
