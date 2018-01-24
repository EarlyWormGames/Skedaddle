using UnityEngine;
using System.Collections;

public class SaveManager : Singleton<SaveManager>
{
    private SaveFile m_sFile;

    private int m_iSaveIndex = 0;
    // Use this for initialization
    protected override void OnAwake()
    {
        DontDestroy();
        OptionsFile.LoadFile("Saves");
        bool def = false;

        m_iSaveIndex = OptionsFile.GetInt("Saves", "Index", out def);
        m_sFile = Saves.Load(m_iSaveIndex.ToString());
        UnlockLevel(1, 0);
        UnlockAnimal(ANIMAL_NAME.LORIS);

        OptionsFile.SetInt("Saves", "Index", m_iSaveIndex);
        OptionsFile.SaveFile("Saves");
    }

    //=======================================================================
    //LEVEL
    public static void UnlockLevel(int a_area, int a_level)
    {
        int currentArea = Instance.m_sFile.GetInt("Area");
        if (currentArea <= a_area)
        {
            int currentLevel = Instance.m_sFile.GetInt("Level");
            if (currentLevel <= a_level)
            {
                Instance.m_sFile.SetInt("Area", a_area);
                Instance.m_sFile.SetInt("Level", a_level);
                Saves.Save(Instance.m_sFile);
            }
        }
    }

    public static bool CheckLevel(int a_area, int a_level)
    {
        if (Instance.m_sFile.GetInt("Area") >= a_area)
        {
            if (Instance.m_sFile.GetInt("Level") >= a_level)
                return true;
        }
        return false;
    }
    //=======================================================================


    //=======================================================================
    //PEANUT
    public static void UnlockPeanut(int a_area, int a_level)
    {
        if (!Instance.m_sFile.GetBool("Peanut-" + a_area.ToString() + "-" + a_level.ToString()))
        {
            Instance.m_sFile.SetInt("Peanuts", Instance.m_sFile.GetInt("Peanuts") + 1);
            Instance.m_sFile.SetBool("Peanut-" + a_area.ToString() + "-" + a_level.ToString(), true);
            Saves.Save(Instance.m_sFile);
        }
    }

    public static bool CheckPeanut(int a_area, int a_level)
    {
        return Instance.m_sFile.GetBool("Peanut-" + a_area.ToString() + "-" + a_level.ToString());
    }

    public static int PeanutCount()
    {
        return Instance.m_sFile.GetInt("Peanuts");
    }
    //=======================================================================


    //=======================================================================
    //ANIMAL
    public static void UnlockAnimal(ANIMAL_NAME a_name)
    {
        Instance.m_sFile.SetBool("AnimalUnlock-" + a_name.ToString(), true);
        Saves.Save(Instance.m_sFile);
    }

    public static bool CheckAnimal(ANIMAL_NAME a_name)
    {
        return Instance.m_sFile.GetBool("AnimalUnlock-" + a_name.ToString());
    }
    //=======================================================================
}
