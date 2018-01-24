using UnityEngine;
using System.Collections.Generic;

public class KeybindObject : MonoBehaviour
{
    public List<KeyCode> m_Keys;

    // Update is called once per frame
    void Update()
    {
        if (m_Keys == null)
            m_Keys = new List<KeyCode>();
        m_Keys.Clear();
    }
}
