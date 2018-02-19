using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Killer : ActionObject
{
    [System.Serializable]
    public class KillEvent : UnityEvent<Animal, DEATH_TYPE> { };

    public DEATH_TYPE KillType;
    public bool OnTrigger = true;

    public KillEvent OnKill;

    protected override void OnCanTrigger()
    {
        if (!OnTrigger)
        {
            if (input.interact.wasJustPressed)
            {
                if (!Animal.CurrentAnimal.Alive)
                    return;

                m_aCurrentAnimal = Animal.CurrentAnimal;
                DoAction();
            }
        }
    }

    public override void AnimalEnter(Animal a_animal)
    {
        base.AnimalEnter(a_animal);
        if (OnTrigger)
        {
            if (!a_animal.Alive)
                return;

            m_aCurrentAnimal = a_animal;
            DoAction();
        }
    }

    public override void DoAction()
    {
        base.DoAction();

        m_aCurrentAnimal.Kill(KillType);
        OnKill.Invoke(m_aCurrentAnimal, KillType);
        m_aCurrentAnimal = null;
    }
}