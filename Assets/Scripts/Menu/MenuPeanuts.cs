using UnityEngine;
using System.Collections;

/// <summary>
/// Peanut chest in the menu
/// check the save file for the peanuts collected to activeate them in the menu
/// </summary>
public class MenuPeanuts : MonoBehaviour {

    public Vector2 m_v2Level;

	void Start () {
        gameObject.SetActive(SaveManager.CheckPeanut(Mathf.RoundToInt(m_v2Level.x), Mathf.RoundToInt(m_v2Level.y)));
	}
	
}
