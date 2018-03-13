using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapChest : ActionObject
{
    public Animator m_Animator;
    public float m_RattleTime = 1;
    public float m_WaitKillTime = 0.5f;
    private bool isOpen;
    private float timer;

    protected override void OnStart()
    {
        base.OnStart();

        m_CanBeDetached = false;
        m_CanDetach = false;
        m_bBlocksMovement = true;
        m_bBlocksTurn = true;
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
        if (m_aCurrentAnimal != null)
            return;

        if (!TryDetach())
            return;      
        base.DoAction();

        m_aCurrentAnimal = Animal.CurrentAnimal;
        m_aCurrentAnimal.m_oCurrentObject = this;

        m_Animator.SetTrigger("Open");
        isOpen = true;

        StartCoroutine(KillAnimal());
    }

    IEnumerator KillAnimal()
    {
        yield return new WaitForSeconds(m_WaitKillTime);
        m_aCurrentAnimal.Kill(DEATH_TYPE.SQUASH);
    }
}