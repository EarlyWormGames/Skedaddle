using UnityEngine;
using System.Collections;

public class Extras : MonoBehaviour
{
    public ExtrasItem[] m_aeItems;

    private ExtrasItem m_eSelected;

    // Use this for initialization
    void Start()
    {
        int nuts = SaveManager.PeanutCount();

        foreach (ExtrasItem item in m_aeItems)
        {
            item.gameObject.SetActive(item.m_requireNuts <= nuts);
            item.m_gNav = item.GetComponent<GUINavigation>();
            item.m_eParent = this;
        }

        if (m_aeItems.Length > 0)
            m_aeItems[0].m_gNav.enabled = m_aeItems[0].gameObject.activeInHierarchy;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_eSelected == null)
        {
            //if (Keybinding.GetKeyDown("Pause") || Controller.GetButtonDown(ControllerButtons.B))
            //    EWApplication.LoadLevel("Menu");
        }
    }

    public void Select(ExtrasItem a_item)
    {
        m_eSelected = a_item;
    }
}
