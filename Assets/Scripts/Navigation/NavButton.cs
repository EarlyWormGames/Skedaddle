using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// a button that can be navigated to using the GUINavigation
/// </summary>
public class NavButton : GUINavigation
{

    public override void Select()
    {
        base.Select();
        GetComponent<Button>().Select();
    }

    public override void Click()
    {
        base.Click();
        GetComponent<Button>().onClick.Invoke();
    }
}
