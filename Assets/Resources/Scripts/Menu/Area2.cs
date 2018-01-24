using UnityEngine;
using System.Collections;

public class Area2 : Area
{

    public string m_sLevelName;

    public override void Select()
    {
        m_IsActive = true;
        Selected = this;
    }

    public override void Deselect()
    {
        m_IsActive = false;
    }

    public override void Click()
    {
        EWApplication.LoadLevel(m_sLevelName);
        m_IsActive = false;
        CurrentArea = this;
    }

    // Update is called once per frame
    protected override void OnUpdate()
    {

    }
}