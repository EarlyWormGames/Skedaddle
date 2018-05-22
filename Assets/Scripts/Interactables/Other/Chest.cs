using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputNew;

public class Chest : AnimalInteractor
{
    [LabelColor(1, 0, 0, true)]
    public ChestManager Manager;
    public Animator m_Animator;
    public float m_RattleTime = 1;

    [HideInNormalInspector]
    public int GUID = 0;
    protected override bool HeadTriggerOnly { get; set; }

    private bool isOpen;
    private float timer;

    protected override void DoInteract(Animal caller)
    {
        if (isOpen)
            return;

        m_Animator.SetTrigger("Open");
        isOpen = true;

        SaveData.AddPeanut();
        SaveData.UnlockChest(GUID);
    }

    void Start()
    {
        if (SaveData.IsChestUnlocked(GUID))
        {
            isOpen = true;
            m_Animator.SetBool("Start_Opened", true);
        }
    }

    void Update()
    {
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
}