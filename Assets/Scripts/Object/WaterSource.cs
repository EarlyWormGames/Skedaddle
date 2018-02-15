using UnityEngine;
using System.Collections;

public class WaterSource : ActionObject
{
    //==================================
    //          Public Vars
    //==================================
    public float m_fFillSpeed = 1f;
    //==================================
    //          Internal Vars
    //==================================

    //==================================
    //          Private Vars
    //================================== 
    private WaterHolder m_wHolder;

    //Inherited functions

    //protected override void OnStart() { }
    protected override void OnCanTrigger()
    {
        if (m_wHolder != null)
        {
            m_wHolder.Fill(m_fFillSpeed * Time.deltaTime);
        }
    }

    protected override void ObjectEnter(Collider a_col)
    {
        WaterHolder hol = a_col.GetComponent<WaterHolder>();
        if (hol != null)
            m_wHolder = hol;
    }

    protected override void ObjectExit(Collider a_col)
    {
        WaterHolder hol = a_col.GetComponent<WaterHolder>();
        if (hol == m_wHolder)
            m_wHolder = null;
    }

    //public override void DoAction() { }
    //public override void DoActionOn() { }
    //public override void DoActionOff() { }
}
