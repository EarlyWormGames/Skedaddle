using UnityEngine;
using UnityEngine.Events;
using System;

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
    public Animal m_aCurrentAnimal;
    public Rigidbody m_rBody;
    public EWGazeObject m_GazeObject;

    [Header("Sounds")]
    public NamedEvent[] m_aSoundEvents;

    [Header("Events")]
    public UnityEvent OnAnimalEnter;
    public UnityEvent OnAnimalExit;


    protected bool m_bUseDefaultAction = true;
    protected bool m_bEyetrackSelected = false;
    protected bool m_bQuickExitFix = false;

    protected float m_fGazeTimer = 0f;
    protected bool m_bGazeRunning = false;

    // Use this for initialization
    void Start()
    {
        if (m_aRequiredAnimal != ANIMAL_NAME.NONE)
        {
            m_aRequiredSize = ANIMAL_SIZE.NONE;
        }

        if (m_GazeObject == null)
            m_GazeObject = GetComponent<EWGazeObject>();

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

        OnUpdate();
    }

    void FixedUpdate()
    {
        OnFixedUpdate();
    }

    protected virtual void OnFixedUpdate() { }
    protected virtual void OnUpdate()
    {
        if (!m_bUseDefaultAction)
            return;

        if (m_aCurrentAnimal == null)
            return;
        else if ((m_aCurrentAnimal.m_oCurrentObject != this && m_aCurrentAnimal.m_oCurrentObject != null) || !m_aCurrentAnimal.m_bSelected)
            return;

        if (Keybinding.GetKeyDown("Action") || Controller.GetButtonDown(ControllerButtons.A))
        {
            DoAction();
        }
        DoAnimation();
    }

    void OnTriggerEnter(Collider a_col)
    {
        AnimalTrigger animtrig = a_col.GetComponent<AnimalTrigger>();
        if (animtrig == null)
        {
            ObjectEnter(a_col);
            return;
        }

        Animal anim = animtrig.parent;

        if (m_aRequiredAnimal != ANIMAL_NAME.NONE)
        {
            if (anim.m_eName != m_aRequiredAnimal)
            {
                WrongAnimalEnter(anim);
                return;
            }
        }
        else if (m_aRequiredSize != ANIMAL_SIZE.NONE)
        {
            if (anim.m_eSize < m_aRequiredSize)
            {
                WrongAnimalEnter(anim);
                return;
            }
        }
        else if(m_aMaximumSize != ANIMAL_SIZE.NONE)
        {
            if (anim.m_eSize > m_aMaximumSize)
            {
                WrongAnimalEnter(anim);
                return;
            }
        }

        m_aCurrentAnimal = anim;
        AnimalEnter(anim);

        OnAnimalEnter.Invoke();
    }

    protected virtual void ObjectEnter(Collider a_col) { }
    public virtual void AnimalEnter(Animal a_animal) { }
    protected virtual void WrongAnimalEnter(Animal a_animal) { }

    void OnTriggerExit(Collider a_col)
    {
        AnimalTrigger animtrig = a_col.GetComponent<AnimalTrigger>();
        if (animtrig == null)
        {
            ObjectExit(a_col);
            return;
        }

        Animal anim = animtrig.parent;

        if (m_aCurrentAnimal != null)
        {
            if (anim == m_aCurrentAnimal)
            {
                if (m_bQuickExitFix)
                {
                    m_bQuickExitFix = false;
                    return;
                }

                m_aCurrentAnimal = null;
            }
            else
            {
                WrongAnimalExit(anim);
                return;
            }
        }
        else
        {
            WrongAnimalExit(anim);
            return;
        }

        if (anim != null)
        {
            AnimalExit(anim);
            OnAnimalExit.Invoke();
        }
    }

    protected virtual void ObjectExit(Collider a_col) { }
    protected virtual void AnimalExit(Animal a_animal) { }
    protected virtual void WrongAnimalExit(Animal a_animal) { }
    public virtual void DoAnimation() { }

    public virtual void DoAction() { }
    public virtual void DoActionOn() { }
    public virtual void DoActionOff() { }

    public virtual void Detach() { }

    public void SetTimer(bool a_Run)
    {
        if (m_bGazeRunning == a_Run)
            return;

        m_fGazeTimer = 0f;
        m_bGazeRunning = a_Run;
    }
}
