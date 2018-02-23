using UnityEngine;
using UnityEngine.Events;
using System;
using System.Collections.Generic;
using UnityEngine.InputNew;

[Serializable]
public class NamedEvent
{
    public string m_EventKey;
    public UnityEvent m_eFunction;

    public static void TriggerEvent(string key, NamedEvent[] events)
    {
        if (events == null)
            return;

        foreach (NamedEvent ev in events)
        {
            if (ev.m_EventKey == key)
            {
                if (ev.m_eFunction != null)
                {
                    ev.m_eFunction.Invoke();
                }
            }
        }
    }
}

public class ActionObject : MonoBehaviour
{
    [Header("Requirements")]
    public ANIMAL_SIZE m_aRequiredSize;
    public ANIMAL_NAME m_aRequiredAnimal;
    public ANIMAL_SIZE m_aMaximumSize;

    [Header("Links")]
    public List<Animal> m_lAnimalsIn = new List<Animal>();
    public Rigidbody m_rBody;
    public EWGazeObject m_GazeObject;

    [Header("Sounds")]
    public NamedEvent[] m_aSoundEvents;

    [Header("Events")]
    public UnityEvent OnDoAction;
    public UnityEvent OnDetach;
    public UnityEvent OnAnimalEnter;
    public UnityEvent OnAnimalExit;
    public UnityEvent OnWrongAnimalEnter;
    public UnityEvent OnWrongAnimalExit;

    internal bool m_CanDetach;
    internal bool m_CanBeDetached = true;
    internal bool m_bBlocksTurn = false;
    internal bool m_bBlocksMovement = false;

    protected bool m_bEyetrackSelected = false;
    protected bool m_bQuickExitFix = false;

    protected float m_fGazeTimer = 0f;
    protected bool m_bGazeRunning = false;

    protected Animal m_aCurrentAnimal;
    protected MainMapping input;


    // Use this for initialization
    void Start()
    {
        if (m_aRequiredAnimal != ANIMAL_NAME.NONE)
        {
            m_aRequiredSize = ANIMAL_SIZE.NONE;
        }

        if (m_GazeObject == null)
            m_GazeObject = GetComponent<EWGazeObject>();

        input = GameManager.Instance.mainMap;

        OnStart();
    }

    protected virtual void OnStart() { }

    // Update is called once per frame
    void Update()
    {
        if (m_aRequiredAnimal != ANIMAL_NAME.NONE)
        {
            m_aRequiredSize = ANIMAL_SIZE.NONE;
        }

        if (EWEyeTracking.GetFocusedObject() == m_GazeObject && m_GazeObject != null)
        {
            //A GREAT EYE IS UPON US
            m_bEyetrackSelected = true;

            Highlighter.Selected = m_GazeObject.gameObject;
        }
        else
            m_bEyetrackSelected = false;

        if (m_bGazeRunning)
            m_fGazeTimer += Time.deltaTime;

        if (m_aCurrentAnimal == null)
        {
            if (m_lAnimalsIn.Contains(Animal.CurrentAnimal))
            {
                if (CheckCorrectAnimal(Animal.CurrentAnimal))
                {
                    OnCanTrigger();
                }
            }
        }
        else if (m_aCurrentAnimal == Animal.CurrentAnimal)
        {
            OnCanTrigger();
        }

        OnUpdate();
    }

    protected virtual void OnCanTrigger()
    {
        if (m_aCurrentAnimal != null)
            return;
        if (input.interact.wasJustPressed)
        {
            DoAction();
        }
    }

    void FixedUpdate()
    {
        OnFixedUpdate();
    }

    protected virtual void OnFixedUpdate() { }
    protected virtual void OnUpdate() { }

    public bool CheckCorrectAnimal(Animal a_animal)
    {
        if (m_aRequiredAnimal != ANIMAL_NAME.NONE)
        {
            if (a_animal.m_eName != m_aRequiredAnimal)
                return false;
        }
        else if (m_aRequiredSize != ANIMAL_SIZE.NONE)
        {
            if (a_animal.m_eSize < m_aRequiredSize)
                return false;
        }
        else if (m_aMaximumSize != ANIMAL_SIZE.NONE)
        {
            if (a_animal.m_eSize > m_aMaximumSize)
                return false;
        }
        return true;
    }

    void OnTriggerEnter(Collider a_col)
    {
        AnimalTrigger animtrig = a_col.GetComponent<AnimalTrigger>();
        Animal anim;
        if (animtrig == null)
        {
            anim = a_col.GetComponentInParent<Animal>();
            if (anim == null)
            {
                ObjectEnter(a_col);
                return;
            }
        }
        else
            anim = animtrig.parent;

        if (m_lAnimalsIn.Contains(anim))
            return;

        if (CheckCorrectAnimal(anim))
        {
            AnimalEnter(anim);
            OnAnimalEnter.Invoke();
        }
        else
        {
            WrongAnimalEnter(anim);
            OnWrongAnimalEnter.Invoke();
        }

        m_lAnimalsIn.Add(anim);
    }

    protected virtual void ObjectEnter(Collider a_col) { }
    public virtual void AnimalEnter(Animal a_animal) { }
    protected virtual void WrongAnimalEnter(Animal a_animal) { }

    void OnTriggerExit(Collider a_col)
    {
        AnimalTrigger animtrig = a_col.GetComponent<AnimalTrigger>();
        Animal anim;
        if (animtrig == null)
        {
            anim = a_col.GetComponentInParent<Animal>();
            if (anim == null)
            {
                ObjectExit(a_col);
                return;
            }
        }
        else
            anim = animtrig.parent;

        if (!m_lAnimalsIn.Contains(anim))
            return;

        m_lAnimalsIn.Remove(anim);

        if (anim != null)
        {
            AnimalExit(anim);
            if (CheckCorrectAnimal(anim))
                OnAnimalExit.Invoke();
            else
                OnWrongAnimalExit.Invoke();
        }
    }

    protected virtual void ObjectExit(Collider a_col) { }
    protected virtual void AnimalExit(Animal a_animal) { }
    protected virtual void WrongAnimalExit(Animal a_animal) { }
    public virtual void DoAnimation() { }

    public virtual void DoAction()
    {
        OnDoAction.Invoke();
    }

    public virtual void DoActionOn() { }
    public virtual void DoActionOff() { }

    public virtual void Detach()
    {
        OnDetach.Invoke();
    }

    public void SetTimer(bool a_Run)
    {
        if (m_bGazeRunning == a_Run)
            return;

        m_fGazeTimer = 0f;
        m_bGazeRunning = a_Run;
    }

    protected bool TryDetach()
    {
        if (Animal.CurrentAnimal.m_oCurrentObject != null)
        {
            if (!m_CanDetach || !Animal.CurrentAnimal.m_oCurrentObject.m_CanBeDetached)
                return false;
            Animal.CurrentAnimal.m_oCurrentObject.Detach();
        }
        return true;
    }
}
