using UnityEngine;
using System.Collections;

public class WaterHolder : ActionObject
{
    //==================================
    //          Public Vars
    //==================================
    public float m_fMaxFillAmount = 10f;
    public float m_fCurrentFill = 0f;

    public float m_fEmptySpeed = 1f;

    //==================================
    //          Internal Vars
    //==================================
    internal bool m_bIsTrunk = false;

    //==================================
    //          Private Vars
    //================================== 
    private WaterHolder m_wTrunk;

    //Inherited functions

    //protected override void OnStart() { }
    protected override void OnUpdate()
    {
        if (m_wTrunk != null && AnimalController.Instance.GetCurrentAnimal().m_eName == ANIMAL_NAME.ELEPHANT)
        {
            if (Keybinding.GetKeyDown("Action") || Controller.GetButtonDown(ControllerButtons.A))
            {
                Fill(m_wTrunk.Empty());
            }
        }
    }
    //protected override void AnimalEnter(Animal a_animal) { }
    //protected override void AnimalExit(Animal a_animal) { }
    //public override void DoAction() { }
    //public override void DoActionOn() { }
    //public override void DoActionOff() { }

    protected override void ObjectEnter(Collider a_col)
    {
        WaterHolder hol = a_col.GetComponent<WaterHolder>();
        if (hol != null)
        {
            if (hol.m_bIsTrunk)
                m_wTrunk = hol;
        }
    }

    protected override void ObjectExit(Collider a_col)
    {
        WaterHolder hol = a_col.GetComponent<WaterHolder>();
        if (hol != null)
        {
            if (hol == m_wTrunk)
                m_wTrunk = null;
        }
    }

    public void Fill(float a_speed)
    {
        m_fCurrentFill += a_speed;

        if (m_fCurrentFill > m_fMaxFillAmount)
        {
            m_fCurrentFill = m_fMaxFillAmount;
        }
    }

    public float Empty()
    {
        float prev = m_fCurrentFill;
        m_fCurrentFill -= m_fEmptySpeed * Time.deltaTime;

        if (m_fCurrentFill < 0f)
        {
            m_fCurrentFill = 0f;
        }
        return prev - m_fCurrentFill;
    }
}
