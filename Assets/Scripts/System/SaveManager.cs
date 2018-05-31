using UnityEngine;
using System.Collections;

/// <summary>
/// helper class for the save management
/// </summary>
public class SaveManager : Singleton<SaveManager>
{
    private int m_iSaveIndex = 0;
    // Use this for initialization
    protected override void OnAwake()
    {

    }

    //=======================================================================
    //LEVEL
    public static void UnlockLevel(int a_area, int a_level)
    {
    }

    public static bool CheckLevel(int a_area, int a_level)
    {
        return true;
    }
    //=======================================================================


    //=======================================================================
    //PEANUT
    public static void UnlockPeanut(int a_area, int a_level)
    {
    }

    public static bool CheckPeanut(int a_area, int a_level)
    {
        return false;
    }

    public static int PeanutCount()
    {
        return 0;
    }
    //=======================================================================


    //=======================================================================
    //ANIMAL
    public static void UnlockAnimal(ANIMAL_NAME a_name)
    {
    }

    public static bool CheckAnimal(ANIMAL_NAME a_name)
    {
        return true;
    }
    //=======================================================================
}
