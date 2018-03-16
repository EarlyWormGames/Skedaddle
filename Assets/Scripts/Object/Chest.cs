using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : ActionObject
{
    [LabelColor(1, 0, 0, true)]
    public ChestManager Manager;
    public Animator m_Animator;
    public float m_RattleTime = 1;

    [HideInNormalInspector]
    public int GUID = 0;

    private bool isOpen;
    private float timer;

    protected override void OnStart()
    {
        base.OnStart();
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();
        if (!isOpen)
        {
            timer += Time.deltaTime;
            if (timer >= m_RattleTime)
            {
                m_Animator.SetTrigger("Rattle");
                timer = 0;
            }
        }
    }

    public override void DoAction()
    {
        if (isOpen)
            return;

        base.DoAction();
        
        m_Animator.SetTrigger("Open");
        isOpen = true;
    }
}