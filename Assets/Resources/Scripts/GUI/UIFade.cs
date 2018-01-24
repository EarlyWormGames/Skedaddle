using UnityEngine;
using UnityEngine.UI;
using System;

public enum UIType
{
    CANVAS,
    IMAGE,
}

public class UIFade : MonoBehaviour
{
    /////////////////////////////////
    // PUBLIC VARS
    /////////////////////////////////
    public UIType   m_uiType;
    public float    m_fFadeInSpeed;
    public float    m_fFadeOutSpeed;
    public float    m_fCurrentAlpha = 0f;

    /////////////////////////////////
    // PRIVATE VARS
    /////////////////////////////////
    private bool    m_bDirection = false;

    void Update()
    {
        switch (m_uiType)
        {
            case UIType.CANVAS:
                {
                    //Check that it's not done
                    if (!m_bDirection)
                    {
                        if (m_fCurrentAlpha <= 0f)
                        {
                            enabled = false;
                            break;
                        }
                    }
                    else
                    {
                        if (m_fCurrentAlpha >= 1f)
                        {
                            enabled = false;
                            break;
                        }
                    }

                    //In/decrease the alpha
                    m_fCurrentAlpha += m_fFadeOutSpeed * Time.deltaTime * (!m_bDirection? -1 : 1);

                    //Set the alpha
                    GetComponent<CanvasGroup>().alpha = m_fCurrentAlpha;
                    break;
                }
            case UIType.IMAGE:
                {
                    //Check that it's not done
                    if (!m_bDirection)
                    {
                        if (m_fCurrentAlpha <= 0f)
                        {
                            enabled = false;
                            break;
                        }
                    }
                    else
                    {
                        if (m_fCurrentAlpha >= 1f)
                        {
                            enabled = false;
                            break;
                        }
                    }

                    //In/decrease the alpha
                    m_fCurrentAlpha += m_fFadeOutSpeed * Time.deltaTime * (!m_bDirection ? -1 : 1);

                    //Create the new color and set the alpha
                    Color col = GetComponent<Image>().color;
                    col.a = m_fCurrentAlpha;

                    GetComponent<Image>().color = col;
                    break;
                }
            default: break;
        }
    }

    public void FadeIn()
    {
        //m_fCurrentAlpha = 0;
        m_bDirection = true;
        enabled = true;

        switch (m_uiType)
        {
            case UIType.CANVAS:
                {
                    GetComponent<CanvasGroup>().alpha = m_fCurrentAlpha;
                    break;
                }
            case UIType.IMAGE:
                {
                    Color col = GetComponent<Image>().color;
                    col.a = m_fCurrentAlpha;

                    GetComponent<Image>().color = col;
                    break;
                }
        }
    }

    public void FadeOut()
    {
        //m_fCurrentAlpha = 1;
        m_bDirection = false;
        enabled = true;

        switch (m_uiType)
        {
            case UIType.CANVAS:
                {
                    GetComponent<CanvasGroup>().alpha = m_fCurrentAlpha;
                    break;
                }
            case UIType.IMAGE:
                {
                    Color col = GetComponent<Image>().color;
                    col.a = m_fCurrentAlpha;

                    GetComponent<Image>().color = col;
                    break;
                }
        }
    }

    public void Stop()
    {
        enabled = false;
    }
}
