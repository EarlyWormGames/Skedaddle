using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Chest Manager", menuName = "Chest Manager")]
public class ChestManager : ScriptableObject
{
    [HideInInspector]
    public List<Chest> chests = new List<Chest>();
    [HideInInspector]
    public List<int> unlockedChests = new List<int>();

    public void LoadData()
    {

    }

    public void UnlockChest()
    {

    }
}