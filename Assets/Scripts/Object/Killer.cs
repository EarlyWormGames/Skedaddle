using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Killer : ActionObject
{
    [System.Serializable]
    public class KillEvent : UnityEvent<Animal, DEATH_TYPE> { };

    public DEATH_TYPE KillType;

    public KillEvent OnKill;

    protected override void OnCanTrigger()
    {
    }

    public override void AnimalEnter(Animal a_animal)
    {
        base.AnimalEnter(a_animal);
        if (CheckCorrectAnimal(a_animal))
        {
            m_aCurrentAnimal = a_animal;
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