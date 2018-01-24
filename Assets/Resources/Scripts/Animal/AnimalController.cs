using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.SceneManagement;
using System;
using System.Collections.Generic;
using System.Linq;

public class AnimalController : Singleton<AnimalController>
{
    //==================================
    //          Public Vars
    //==================================

    //==================================
    //          Internal Vars
    //==================================

    //==================================
    //          Private Vars
    //==================================
    private List<Animal> m_lAnimals;
    private int m_iSelectedNumber = 0;

    private InGameGUI m_iAnimalGui;
    private int m_LastSelected;

    private bool m_bControlling = true;


    // Use this for initialization
    void Start()
    {
        m_lAnimals = new List<Animal>();
        Recheck();
        SceneManager.sceneLoaded += SceneLoaded;
    }

    void SceneLoaded(Scene a_scene, LoadSceneMode a_mode)
    {
        Recheck();
    }

    // Update is called once per frame
    void Update()
    {
        if (!m_bControlling)
            return;

        if (m_lAnimals.Count > m_iSelectedNumber)
        {
            EWEyeTracking.FocusZ = m_lAnimals[m_iSelectedNumber].transform.position.z;
        }

        //Debug.Log(Keybinding.GetKeyDown("Action"));

        if (Keybinding.GetKeyDown("PrevAnimal") || Controller.GetButtonDown(ControllerButtons.LeftShoulder))
        {
            ChangeAnimal(m_iSelectedNumber - 1);
        }

        if (Keybinding.GetKeyDown("NextAnimal") || Controller.GetButtonDown(ControllerButtons.RightShoulder))
        {
            ChangeAnimal(m_iSelectedNumber + 1);
        }

        for (int i = 1; i <= 5; ++i)
        {
            if (Keybinding.GetKeyDown("Animal" + i.ToString()))
            {
                ChangeAnimal((ANIMAL_NAME)i);
            }
        }
    }

    public void Deselect()
    {
        m_lAnimals[m_iSelectedNumber].m_bSelected = false;
        m_lAnimals[m_iSelectedNumber].OnSelectChange();

        m_bControlling = false;
    }

    public void Reselect()
    {
        m_lAnimals[m_iSelectedNumber].m_bSelected = true;
        m_lAnimals[m_iSelectedNumber].OnSelectChange();
        m_bControlling = true;
    }

    public void ChangeAnimal(int a_index)
    {
        if (m_lAnimals.Count <= 0)
        {
            return;
        }

        if (a_index >= m_lAnimals.Count)
        {
            a_index = 0;
        }
        else if (a_index < 0)
        {
            a_index = m_lAnimals.Count - 1;
        }

        if (m_lAnimals[a_index].m_bCanBeSelected)
        {
            //De-select the previous and select the new one
            if (m_iSelectedNumber < m_lAnimals.Count && m_iSelectedNumber >= 0)
            {
                if (m_lAnimals[m_iSelectedNumber].m_bAutoClimbing)
                    return;

                m_lAnimals[m_iSelectedNumber].m_bSelected = false;
                m_lAnimals[m_iSelectedNumber].OnSelectChange();

                Analytics.CustomEvent("Animal Swap", new Dictionary<string, object>
                {
                    { "From", m_lAnimals[m_iSelectedNumber].gameObject.name },
                    { "To", m_lAnimals[a_index].gameObject.name }
                });
            }
            

            m_iSelectedNumber = a_index;
            CameraController.Instance.m_aAnimal = m_lAnimals[m_iSelectedNumber];
            m_lAnimals[m_iSelectedNumber].m_bSelected = true;
            m_lAnimals[m_iSelectedNumber].OnSelectChange();
            MusicHandler.SetIndex(m_iSelectedNumber);
            if (m_iAnimalGui != null)
            {
                m_iAnimalGui.ChangeAnimal(m_lAnimals[m_iSelectedNumber].m_eName);
            }
        }
        else
        {
            if (a_index < m_iSelectedNumber)
            {
                ChangeAnimal(a_index - 1);
                return;
            }
            else if (a_index > m_iSelectedNumber)
            {
                ChangeAnimal(a_index + 1);
                return;
            }
        }
    }

    public void ChangeAnimal(ANIMAL_NAME a_animal)
    {
        for (int i = 0; i < m_lAnimals.Count; ++i)
        {
            if (m_lAnimals[i].m_eName == a_animal)
            {
                if (m_lAnimals[i].m_bCanBeSelected)
                {
                    //De-select the previous and select the new one
                    if (m_iSelectedNumber < m_lAnimals.Count && m_iSelectedNumber >= 0 && m_iSelectedNumber != i)
                    {
                        if (m_lAnimals[m_iSelectedNumber].m_bAutoClimbing)
                            return;

                        m_lAnimals[m_iSelectedNumber].m_bSelected = false;
                        m_lAnimals[m_iSelectedNumber].OnSelectChange();

                        Analytics.CustomEvent("Animal Swap", new Dictionary<string, object>
                        {
                            { "From", m_lAnimals[m_iSelectedNumber].gameObject.name },
                            { "To", m_lAnimals[i].gameObject.name }
                        });
                    }

                    m_iSelectedNumber = i;
                    CameraController.Instance.m_aAnimal = m_lAnimals[m_iSelectedNumber];
                    m_lAnimals[m_iSelectedNumber].m_bSelected = true;
                    m_lAnimals[m_iSelectedNumber].OnSelectChange();
                    MusicHandler.SetIndex(m_iSelectedNumber);
                    if (m_iAnimalGui != null)
                    {
                        m_iAnimalGui.ChangeAnimal(m_lAnimals[m_iSelectedNumber].m_eName);
                    }
                }
            }
        }
    }

    public void Recheck()
    {
        Analytics.CustomEvent("Scene Loaded", new Dictionary<string, object>
        {
            { "Level", SceneManager.GetActiveScene().name }
        });

        if (m_lAnimals == null)
            m_lAnimals = new List<Animal>();

        m_lAnimals.Clear();

        List<Animal> animsFound = new List<Animal>(FindObjectsOfType<Animal>());

        m_iAnimalGui = FindObjectOfType<InGameGUI>();

        m_lAnimals = animsFound.OrderBy(x => (int)(x.m_eName)).ToList();

        if (m_iAnimalGui != null)
        {
            m_iAnimalGui.StartLevel(m_lAnimals.Count);
        }

        if (m_iSelectedNumber > m_lAnimals.Count)
        {
            m_iSelectedNumber = 0;
        }

        if (animsFound.Count > 0)
        {
            EndLevel lvl = FindObjectOfType<EndLevel>();
            if (lvl != null)
                lvl.SetRequirement(animsFound.Count);
        }
        ChangeAnimal(m_iSelectedNumber);
    }

    public int GetAnimalCount()
    {
        if (m_lAnimals != null)
        {
            return m_lAnimals.Count;
        }
        return 0;
    }

    public Animal GetAnimal(ANIMAL_NAME a_name)
    {
        for (int i = 0; i < m_lAnimals.Count; ++i)
        {
            if (m_lAnimals[i].m_eName == a_name)
            {
                return m_lAnimals[i];
            }
        }
        return null;
    }

    public bool GetAnimalSelected(ANIMAL_NAME a_name)
    {
        for (int i = 0; i < m_lAnimals.Count; ++i)
        {
            if (m_lAnimals[i].m_eName == a_name)
            {
                return m_lAnimals[i].m_bSelected;
            }
        }
        return false;
    }

    public Animal GetCurrentAnimal()
    {
        if (m_lAnimals.Count < 0)
            return null;

        return m_lAnimals[m_iSelectedNumber];
    }
}
