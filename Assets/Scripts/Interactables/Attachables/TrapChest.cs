using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapChest : AttachableInteract
{
    public Animator m_Animator;
    public float m_RattleTime = 1;
    public float m_WaitKillTime = 0.5f;
    private bool isOpen;
    private float timer;

    protected override void OnStart()
    {
        base.OnStart();

        CanDetach = false;
        BlocksMovement = true;
        BlocksTurn = true;
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

    protected override bool CheckDetach()
    {
        //REEEEEEEEEEEEEEEEEEE
        //No-detacho. Comprende?
        return false;
    }

    protected override void DoInteract(Animal caller)
    {
        if (AttachedAnimal != null)
            return;

        if (!TryDetachOther())
            return;

        Attach(caller);

        m_Animator.SetTrigger("Open");
        isOpen = true;

        StartCoroutine(KillAnimal());
    }

    IEnumerator KillAnimal()
    {
        yield return new WaitForSeconds(m_WaitKillTime);
        AttachedAnimal.Kill(DEATH_TYPE.SQUASH);
    }
}