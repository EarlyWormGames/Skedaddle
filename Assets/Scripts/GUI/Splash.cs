using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Splash Screne
/// </summary>
public class Splash : MonoBehaviour
{
    public static bool GameLoaded = false;

    public Image Logo;
    public Image Background;
    public AnimationCurve FadeCurve;
    public float FadeInTime = 1f;
    public float WaitTime = 1f;
    public float FadeOutTime = 1f;

    public Area StartArea;

    private float m_fFadeTimer;
    private int m_iDirection = 0;

    // Use this for initialization
    void Start()
    {
        Background.gameObject.SetActive(true);
        if (GameLoaded)
        {
            Color col = Background.color;
            col.a = 0;
            Background.color = col;

            col = Logo.color;
            col.a = 0;
            Logo.color = col;

            StartArea.Select();
            Destroy(gameObject);
            return;
        }
        else
        {
            Color col = Background.color;
            col.a = 1;
            Background.color = col;

            col = Logo.color;
            col.a = 0;
            Logo.color = col;
        }
    }

	// Update is called once per frame
	void Update ()
    {
        m_fFadeTimer += Time.deltaTime;
        float t = 0;
        switch (m_iDirection)
        {
            case 0:
                {
                    MouseCheck();

                    t = m_fFadeTimer / FadeInTime;
                    t = FadeCurve.Evaluate(t);

                    Color col = Logo.color;
                    col.a = t;
                    Logo.color = col;

                    if (t >= 1f)
                    {
                        m_fFadeTimer = 0f;
                        ++m_iDirection;
                    }
                    break;
                }
            case 1:
                {
                    MouseCheck();

                    t = m_fFadeTimer / WaitTime;
                    if (t >= 1f)
                    {
                        m_fFadeTimer = 0f;
                        ++m_iDirection;
                    }
                    break;
                }
            case 2:
                {
                    t = m_fFadeTimer / FadeOutTime;
                    t = FadeCurve.Evaluate(t);

                    Color col = Logo.color;
                    col.a = 1 - t;
                    Logo.color = col;

                    col = Background.color;
                    col.a = 1 - t;
                    Background.color = col;

                    if (t >= 1f)
                    {
                        m_fFadeTimer = 0f;
                        ++m_iDirection;
                    }
                    break;
                }
            case 3:
                {
                    GameLoaded = true;
                    StartArea.Select();
                    Destroy(gameObject);
                    break;
                }
        }
    }

    void MouseCheck()
    {
        if (Input.anyKeyDown)
        {
            m_fFadeTimer = 0f;
            m_iDirection = 2;
        }
    }
}
