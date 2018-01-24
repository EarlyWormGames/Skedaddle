using UnityEngine;
using UnityEngine.UI;
using System.Collections;

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
