using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public enum TAG_COLOR
{
    UNSELECTED,
    SELECTED,
    EXITOUT,
    EXITIN,
}

public enum TAG_POSITION
{
    HIDDEN,
    UNSELECTED,
    SELECTED
}

[System.Serializable]
public class AnimalTag
{
    public Image m_iIcon;
    public ANIMAL_NAME m_aName;
    public EWGazeObject m_GazeObject;

    internal TAG_COLOR m_tCurrentColour;
    internal TAG_POSITION m_tCurrentTarget;
    internal float m_fMoveVel;
    internal float[] m_afColorVels;

    internal float m_GazeTimer;
}

public class InGameGUI : MonoBehaviour
{
    public AnimalTag[]  m_aaAnimalTags;
    public Color[]      m_acColors = new Color[4];

    public Transform[]  m_atTargetPoints;
    public float        m_fMoveSpeed = 15f;
    public float        m_fColourSpeed = 15f;

    private int         m_iCurrentAnimal;
    private int         m_iExitCount = 0;
    private bool        m_bEndLevel = false;

    void Start()
    {
        EWApplication.Instance.m_olCallbacks += EndLevel;
    }

    void Update()
    {
        if (AnimalController.Instance.GetAnimalCount() > 0)
        {
            if (m_iCurrentAnimal >= 0)
            {
                if (m_aaAnimalTags[m_iCurrentAnimal].m_tCurrentColour == TAG_COLOR.UNSELECTED)
                {
                    m_aaAnimalTags[m_iCurrentAnimal].m_tCurrentColour = TAG_COLOR.SELECTED;
                }
            }

            for (int i = 0; i < m_aaAnimalTags.Length; i++)
            {
                //Set the active state to their respective availableness
                Animal anim = AnimalController.Instance.GetAnimal((ANIMAL_NAME)(i + 1));
                if (anim != null)
                {
                    m_aaAnimalTags[i].m_iIcon.enabled = anim.m_bCanBeSelected;

                    bool changeCol = true;

                    if (m_aaAnimalTags[i].m_tCurrentColour == TAG_COLOR.EXITIN || m_aaAnimalTags[i].m_tCurrentColour == TAG_COLOR.EXITOUT)
                    {
                        changeCol = false;
                    }

                    if (i == m_iCurrentAnimal)
                    {
                        if (!m_bEndLevel)
                            m_aaAnimalTags[i].m_tCurrentTarget = TAG_POSITION.SELECTED;
                        else
                            m_aaAnimalTags[i].m_tCurrentTarget = TAG_POSITION.HIDDEN;

                        if (changeCol)
                            m_aaAnimalTags[i].m_tCurrentColour = TAG_COLOR.SELECTED;
                    }
                    else
                    {
                        if (!m_bEndLevel)
                        {
                            if (anim.m_bCanBeSelected)
                            {
                                m_aaAnimalTags[i].m_tCurrentTarget = TAG_POSITION.UNSELECTED;
                            }
                            else
                            {
                                m_aaAnimalTags[i].m_tCurrentTarget = TAG_POSITION.HIDDEN;
                            }
                        }
                        else
                            m_aaAnimalTags[i].m_tCurrentTarget = TAG_POSITION.HIDDEN;

                        if (changeCol)
                            m_aaAnimalTags[i].m_tCurrentColour = TAG_COLOR.UNSELECTED;
                    }
                }
                else
                {
                    m_aaAnimalTags[i].m_iIcon.enabled = false;
                    m_aaAnimalTags[i].m_tCurrentTarget = TAG_POSITION.HIDDEN;
                }


                //Lerp the color
                m_aaAnimalTags[i].m_iIcon.color = SmoothColor(m_aaAnimalTags[i].m_iIcon.color, m_acColors[(int)m_aaAnimalTags[i].m_tCurrentColour], m_aaAnimalTags[i].m_afColorVels, m_fColourSpeed);

                //Move to the target point
                Vector3 pos = m_aaAnimalTags[i].m_iIcon.transform.position;
                pos.y = Mathf.SmoothDamp(m_aaAnimalTags[i].m_iIcon.transform.position.y, m_atTargetPoints[(int)m_aaAnimalTags[i].m_tCurrentTarget].position.y, ref m_aaAnimalTags[i].m_fMoveVel, m_fMoveSpeed * Time.deltaTime);
                m_aaAnimalTags[i].m_iIcon.transform.position = pos;

                if (EWEyeTracking.GetFocusedUI() == m_aaAnimalTags[i].m_GazeObject)
                    m_aaAnimalTags[i].m_GazeTimer += Time.deltaTime;
                else
                    m_aaAnimalTags[i].m_GazeTimer = 0f;

                if (m_aaAnimalTags[i].m_GazeTimer >= EWEyeTracking.shortHoldTime)
                {
                    m_aaAnimalTags[i].m_GazeTimer = 0f;
                    AnimalController.Instance.ChangeAnimal(m_aaAnimalTags[i].m_aName);
                }
            }
        }
    }

    Color SmoothColor(Color a_col, Color a_target, float[] a_moveVels, float a_speed)
    {
        Color col = new Color();
        col.r = Mathf.SmoothDamp(a_col.r, a_target.r, ref a_moveVels[0], a_speed * Time.deltaTime);
        col.g = Mathf.SmoothDamp(a_col.g, a_target.g, ref a_moveVels[1], a_speed * Time.deltaTime);
        col.b = Mathf.SmoothDamp(a_col.b, a_target.b, ref a_moveVels[2], a_speed * Time.deltaTime);
        col.a = Mathf.SmoothDamp(a_col.a, a_target.a, ref a_moveVels[3], a_speed * Time.deltaTime);

        return col;
    }

    public void ChangeAnimal(ANIMAL_NAME a_name)
    {
        int index = GetAnimal(a_name);

        if (index >= m_aaAnimalTags.Length || index < 0)
            return;

        m_iCurrentAnimal = index;

        for (int i = 0; i < m_aaAnimalTags.Length; ++i)
        {
            m_aaAnimalTags[i].m_afColorVels = new float[4];
        }
    }

    public void StartLevel(int a_TotalAnimals)
    {
        //for (int i = 0; i < m_aaAnimalTags.Length; i++)
        //{
        //    Animal anim = AnimalController.Instance.GetAnimal((ANIMAL_NAME)(i + 1));
        //    if (anim != null)
        //    {
        //        if(anim.m_bCanBeSelected)
        //            m_aiTargets[i] = 1;
        //        else
        //            m_aiTargets[i] = 0;
        //    }
        //    else
        //    {
        //        m_aiTargets[i] = 0;
        //    }
        //}
    }

    public void EndLevel(string a_level)
    {
        m_bEndLevel = true;

        EWApplication.Instance.m_olCallbacks -= EndLevel;
    }

    public void AnimalEnterExit(ANIMAL_NAME a_name)
    {
        int index = GetAnimal(a_name);

        if (index >= m_aaAnimalTags.Length || index < 0)
            return;

        m_aaAnimalTags[index].m_tCurrentColour = TAG_COLOR.EXITIN;
        ++m_iExitCount;
        for (int i = 0; i < m_aaAnimalTags.Length; ++i)
        {
            if (m_aaAnimalTags[i].m_tCurrentColour != TAG_COLOR.EXITIN)
            {
                m_aaAnimalTags[i].m_tCurrentColour = TAG_COLOR.EXITOUT;
            }
        }
        //++m_iAnimalsInExit;
        //m_atColors[a_animalIndex] = TAG_COLOR.EXITIN;
        //for (int i = 0; i < m_atColors.Length; i++)
        //{
        //    if (m_atColors[i] != TAG_COLOR.EXITIN)
        //    {
        //        m_atColors[i] = TAG_COLOR.EXITOUT;
        //    }
        //}
    }

    public void AnimalLeaveExit(ANIMAL_NAME a_name)
    {
        int index = GetAnimal(a_name);

        if (index >= m_aaAnimalTags.Length || index < 0)
            return;

        --m_iExitCount;
        if (m_iExitCount > 0)
        {
            m_aaAnimalTags[index].m_tCurrentColour = TAG_COLOR.EXITOUT;
        }

        for (int i = 0; i < m_aaAnimalTags.Length; ++i)
        {
            if (m_iExitCount > 0)
            {
                if (m_aaAnimalTags[i].m_tCurrentColour != TAG_COLOR.EXITIN)
                {
                    m_aaAnimalTags[i].m_tCurrentColour = TAG_COLOR.EXITOUT;
                }
            }
            else
            {
                m_aaAnimalTags[i].m_tCurrentColour = TAG_COLOR.UNSELECTED;
            }
        }

        //--m_iAnimalsInExit;
        //if (m_iAnimalsInExit <= 0)
        //{
        //    for (int i = 0; i < m_atColors.Length; i++)
        //    {
        //        m_atColors[i] = TAG_COLOR.UNSELECTED;
        //    }
        //}
        //else
        //{
        //    m_atColors[a_animalIndex] = TAG_COLOR.EXITOUT;
        //}
    }

    public int GetAnimal(ANIMAL_NAME a_name)
    {
        for (int i = 0; i < m_aaAnimalTags.Length; ++i)
        {
            if (m_aaAnimalTags[i].m_aName == a_name)
            {
                return i;
            }
        }
        return -1;
    }
}
