using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TutPanel : Singleton<TutPanel>
{
    public GameObject m_goTextPanel;
    public Transform m_tTop;
    public Transform m_tBottom;
    public float m_fWidthPerChar = 1f;
    public int m_iMaxCharPerline = 10;
    public float m_fHeightPerLine = 1f;
    public float m_fBaseHeight = 30f;
    public float m_fWaitPerWord = 0.5f;

    public AnimationCurve m_aCurve;
    public float m_fShowTime = 2f; //IT'S SHOWTIME MOTHERFUCKER
    public float m_fHideTime = 1f;

    private enum TEXT_DIR
    {
        NONE,
        HIDE,
        SHOW,
        DOWN
    }

    private TEXT_DIR m_eDirection;

    private Vector3 m_v3MaxVel;

    private Text m_tTextObject;
    private RectTransform m_rTextTrans;
    private RectTransform m_rTopTrans;
    private RectTransform m_rBottomTrans;

    private float m_fTimer = -1f;

    // Use this for initialization
    void Start()
    {
        m_bDestroyEntireObject = true;

        if (m_goTextPanel == null)
        {
            return;
        }
        
        m_tTextObject = m_goTextPanel.GetComponentInChildren<Text>();
        m_rTextTrans = m_goTextPanel.GetComponent<RectTransform>();
        m_rTopTrans = m_tTop.gameObject.GetComponent<RectTransform>();
        m_rBottomTrans = m_tBottom.gameObject.GetComponent<RectTransform>();

        m_eDirection = 0;

        EWApplication.Instance.m_olCallbacks += OnExiting;

        //ShowText("This is some info. This box will resize depending on how many characters are in the string. It will also wait on the screen until you tell it to disappear or you can set it to auto hide depending on how many words there are.");
    }

    // Update is called once per frame
    void Update()
    {
        if (m_goTextPanel == null)
        {
            return;
        }

        if (string.Compare(m_tTextObject.text, "new text", true) == 0)
        {
            m_eDirection = TEXT_DIR.HIDE;
            m_fTimer = m_fHideTime;
        }

        if (m_fTimer >= 0)
            m_fTimer += Time.deltaTime;

        if (m_eDirection == TEXT_DIR.SHOW)
        {
            float t = Mathf.Min(1f, m_fTimer / m_fShowTime);
            if (m_fTimer >= m_fShowTime)
            {
                m_eDirection = 0;
                m_fTimer = -1f;
            }

            m_goTextPanel.transform.position = Vector3.Lerp(m_tBottom.position, m_tTop.position, t);
        }
        else if (m_eDirection == TEXT_DIR.HIDE)
        {
            //m_rtPanel.offsetMin = Vector2.SmoothDamp(m_rtPanel.offsetMin, new Vector2(m_rtPanel.offsetMin.x, -m_rtPanel.sizeDelta.y - 10), ref m_v2MinVel, m_fMoveSpeed * Time.deltaTime);
            //m_rtPanel.offsetMax = Vector2.SmoothDamp(m_rtPanel.offsetMax, new Vector2(m_rtPanel.offsetMax.x, -10), ref m_v2MaxVel, m_fMoveSpeed * Time.deltaTime);

            float t = Mathf.Min(1f, m_fTimer / m_fHideTime);
            if (m_fTimer >= m_fHideTime)
            {
                m_fTimer = -1f;
                m_eDirection = TEXT_DIR.DOWN;
            }

            m_goTextPanel.transform.position = Vector3.Lerp(m_tTop.position, m_tBottom.position, t);
        }
    }

    public void ShowText(string a_text)
    {
        if (m_goTextPanel == null)
        {
            return;
        }

        m_tTextObject.text = a_text;

        /* Measure the text to produce a max width and then the text will obviously wrap
        to the next line kinda like this */
        float width = m_fWidthPerChar;
        float height = 0f;
        if (a_text.Length > m_iMaxCharPerline)
        {
            width *= m_iMaxCharPerline;
            height = m_fHeightPerLine * (int)((a_text.Length / m_iMaxCharPerline) + 0.5f);
        }
        else
        {
            width *= a_text.Length;
        }

        m_rTextTrans.sizeDelta = new Vector2(width, height + m_fBaseHeight);

        m_rTopTrans.sizeDelta = new Vector2(width, height + m_fBaseHeight);
        m_rTopTrans.offsetMin = new Vector2(m_rTopTrans.offsetMin.x, 0f);
        m_rTopTrans.offsetMax = new Vector2(m_rTopTrans.offsetMax.x, height + m_fBaseHeight);

        m_rBottomTrans.sizeDelta = new Vector2(width, height + m_fBaseHeight);
        m_rBottomTrans.offsetMin = new Vector2(m_rBottomTrans.offsetMin.x, -(height + m_fBaseHeight));
        m_rBottomTrans.offsetMax = new Vector2(m_rBottomTrans.offsetMax.x, 0f);

        m_eDirection = TEXT_DIR.SHOW;
        m_fTimer = 0f;
    }

    public void Hide()
    {
        if (m_eDirection != TEXT_DIR.DOWN)
            m_eDirection = TEXT_DIR.HIDE;
        m_fTimer = 0f;
    }

    public void OnExiting(string a_levelTo)
    {
        if (m_eDirection != TEXT_DIR.DOWN)
            m_eDirection = TEXT_DIR.HIDE;
        m_fTimer = 0f;
    }
}
