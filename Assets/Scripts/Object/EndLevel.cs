using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.InputNew;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class EndLevel : MonoBehaviour
{
    public string m_sLoadScene;

    public LevelDoor m_Door;

    public SpriteRenderer m_sLock;
    public float m_fColorLerpSpeed = 1f;
    public Color m_cInColor;
    public Color m_cFullColor;
    public int m_iAnimalInCount = 0;

    internal int m_iRequiredCount;

    private Color m_cEmptyColor;
    private List<Animal> m_lAnimsIn;
    private InGameGUI m_iAnimalGUI;

    private float m_fLevelTimer;

    private float m_fGazeTimer = 0f;

    private EWGazeObject m_GazeObject;
    private ButtonInputControl ExitKey;

    // Use this for initialization
    void Start()
    {
        m_cEmptyColor = m_sLock.color;
        m_lAnimsIn = new List<Animal>();
        m_iAnimalGUI = FindObjectOfType<InGameGUI>();
        if (m_Door != null)
            m_Door.enabled = false;

        m_GazeObject = GetComponent<EWGazeObject>();

        ExitKey = GameManager.Instance.GetComponent<PlayerInput>().GetActions<MainMapping>().interact;
    }

    // Update is called once per frame
    void Update()
    {
        m_iAnimalInCount = m_lAnimsIn.Count;
        m_fLevelTimer += Time.deltaTime;

        if (EWEyeTracking.GetFocusedObject() == m_GazeObject)
        {
            Highlighter.Selected = gameObject;
        }

        if (m_lAnimsIn.Count == 0)
        {
            m_sLock.color = Color.Lerp(m_sLock.color, m_cEmptyColor, Time.deltaTime * m_fColorLerpSpeed);
        }
        else if (m_lAnimsIn.Count < m_iRequiredCount)
        {
            m_sLock.color = Color.Lerp(m_sLock.color, m_cInColor, Time.deltaTime * m_fColorLerpSpeed);
        }
        else
        {
            m_sLock.color = Color.Lerp(m_sLock.color, m_cFullColor, Time.deltaTime * m_fColorLerpSpeed);

            if (EWEyeTracking.GetFocusedObject() == m_GazeObject && EWEyeTracking.active)
            {
                m_fGazeTimer += Time.deltaTime;
            }
            else
                m_fGazeTimer = 0f;

            if (ExitKey.wasJustPressed || m_fGazeTimer >= EWEyeTracking.holdTime)
            {
                Analytics.CustomEvent("Level Ended", new Dictionary<string, object>
                {
                    { "Level", SceneManager.GetActiveScene().name },
                    { "Time", m_fLevelTimer }
                });
            
                //EXIT LEVEL
                EWApplication.LoadLevel(m_sLoadScene);
                enabled = false;
            
                if (m_Door != null)
                    m_Door.enabled = true;
            }
        }
    }

    void OnTriggerEnter(Collider a_col)
    {
        Animal anim = a_col.GetComponentInParent<Animal>();
        if (anim == null)
        {
            return;
        }

        if (!m_lAnimsIn.Contains(anim))
        {
            m_lAnimsIn.Add(anim);
            m_iAnimalGUI.AnimalEnterExit(anim.m_eName);
            anim.m_aAnimalAnimator.SetBool("InExitBox", true);
        }
    }

    void OnTriggerExit(Collider a_col)
    {
        Animal anim = a_col.GetComponentInParent<Animal>();
        if (anim == null)
        {
            return;
        }

        if (m_lAnimsIn.Contains(anim))
        {
            m_lAnimsIn.Remove(anim);
            m_iAnimalGUI.AnimalLeaveExit(anim.m_eName);
            anim.m_aAnimalAnimator.SetBool("InExitBox", false);
        }
    }

    public void SetRequirement(int a_amount)
    {
        m_iRequiredCount = a_amount;
    }
}
