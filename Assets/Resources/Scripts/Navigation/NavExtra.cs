using UnityEngine;
using System.Collections;

public class NavExtra : GUINavigation
{
    private ExtrasItem m_eItem;

    void Start()
    {
        m_eItem = GetComponent<ExtrasItem>();
    }

    public override void Select()
    {
        base.Select();
    }

    public override void Deselect()
    {
        base.Deselect();
    }

    public override void Click()
    {
        base.Click();
        m_eItem.Show();
    }
}
