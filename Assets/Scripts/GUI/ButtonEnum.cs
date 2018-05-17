using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Dead Script
/// </summary>
public class ButtonEnum : MonoBehaviour
{
    //public KeyActionName m_kKey;
    public bool m_bIsSecondary;
    public bool m_bIsGamepad;
    
    void Start()
    {
        Check();
    }

    public void Check()
    {
        //string Keyname = Keybinding.GetKeyName(m_kKey, m_bIsSecondary? KeyType.SECONDARY : KeyType.PRIMARY);
        //
        //if (m_bIsGamepad)
        //{
        //    Keyname = Keybinding.GetKeyName(m_kKey, KeyType.GPAD);
        //
        //    Sprite spr;
        //    if (Keyname != "NULL" && Keyname != null && Keyname != "null")
        //    {
        //        GPadControls gpad = (GPadControls)System.Enum.Parse(typeof(GPadControls), Keyname);
        //        spr = GameObject.Find("ScreenSpace Canvas").GetComponent<Menu>().GetGpadButton(gpad);
        //    }
        //    else
        //    {
        //        spr = null;
        //    }
        //
        //    if (spr != null)
        //    {
        //        GetComponent<Image>().sprite = spr;
        //    }
        //}
        //else
        //{
        //    if (Keyname == "LEFTSHIFT")
        //    {
        //        GetComponentInChildren<Text>().text = "L SHIFT";
        //        return;
        //    }
        //    if (Keyname == "RIGHTSHIFT")
        //    {
        //        GetComponentInChildren<Text>().text = "R SHIFT";
        //        return;
        //    }
        //    else
        //    {
        //        GetComponentInChildren<Text>().text = Keyname;
        //    }
        //
        //}
    }
}
