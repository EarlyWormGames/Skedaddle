using UnityEngine;
using System.Collections;

public class MenuPeanuts : MonoBehaviour {

    public Vector2 m_v2Level;

	void Start () {
        gameObject.SetActive(SaveManager.CheckPeanut(Mathf.RoundToInt(m_v2Level.x), Mathf.RoundToInt(m_v2Level.y)));
	}
	
}
