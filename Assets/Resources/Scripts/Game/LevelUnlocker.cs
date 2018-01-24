using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class LevelUnlocker : MonoBehaviour
{
    // Use this for initialization
    void Awake()
    {
        string[] al = SceneManager.GetActiveScene().name.Split('-');
        //SaveManager.UnlockLevel(Convert.ToInt32(al[0]), Convert.ToInt32(al[1]));
    }
}
