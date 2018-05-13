using System;
using UnityEngine.Events;

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

//public class ActionObject : MonoBehaviour
//{
//    [Header("Requirements")]
//    public ANIMAL_SIZE m_aRequiredSize;
//    public ANIMAL_NAME m_aRequiredAnimal;
//    public ANIMAL_SIZE m_aMaximumSize;
//    public bool m_bOnlyHeadTrigger = true;

//    [Header("Links")]
//    public List<Animal> m_lAnimalsIn = new List<Animal>();
//    public Rigidbody m_rBody;
//    public EWGazeObject m_GazeObject;

//    [Header("Sounds")]
//    public NamedEvent[] m_aSoundEvents;

//    [Header("Events")]
//    public UnityEvent OnDoAction;
//    public UnityEvent OnDetach;
//    public UnityEvent OnAnimalEnter;
//    public UnityEvent OnAnimalExit;
//    public UnityEvent OnWrongAnimalEnter;
//    public UnityEvent OnWrongAnimalExit;

//    internal bool m_CanDetach;
//    internal bool m_CanBeDetached = true;
//    internal bool m_bBlocksTurn = false;
//    internal bool m_bBlocksMovement = false;
//    internal Animal m_aCurrentAnimal;

//    protected bool m_bEyetrackSelected = false;
//    protected bool m_bQuickExitFix = false;

//    protected float m_fGazeTimer = 0f;
//    protected bool m_bGazeRunning = false;

//    protected MainMapping input;

//    private static List<ActionObject> m_lCurrentObjects = new List<ActionObject>();


//    // Use this for initialization
//    void Start()
//    {
//        if (m_aRequiredAnimal != ANIMAL_NAME.NONE)
//        {
//            m_aRequiredSize = ANIMAL_SIZE.NONE;
//        }

//        if (m_GazeObject == null)
//            m_GazeObject = GetComponent<EWGazeObject>();

//        input = GameManager.Instance.mainMap;

//        m_lCurrentObjects.Add(this);

//        OnStart();
//    }

//    protected virtual void OnStart() { }

//    // Update is called once per frame
//    void Update()
//    {
//        if (m_aRequiredAnimal != ANIMAL_NAME.NONE)
//        {
//            m_aRequiredSize = ANIMAL_SIZE.NONE;
//        }

//        if (EWEyeTracking.GetFocusedObject() == m_GazeObject && m_GazeObject != null)
//        {
//            //A GREAT EYE IS UPON US
//            m_bEyetrackSelected = true;

//            Highlighter.Selected = m_GazeObject.gameObject;
//        }
//        else
//            m_bEyetrackSelected = false;

//        if (m_bGazeRunning)
//            m_fGazeTimer += Time.deltaTime;

//        if (m_aCurrentAnimal == null)
//        {
//            if (m_lAnimalsIn.Contains(Animal.CurrentAnimal))
//            {
//                if (CheckCorrectAnimal(Animal.CurrentAnimal))
//                {
//                    OnCanTrigger();
//                }
//            }
//        }
//        else if (m_aCurrentAnimal == Animal.CurrentAnimal)
//        {
//            OnCanTrigger();
//        }

//        OnUpdate();
//    }

//    protected virtual void OnCanTrigger()
//    {
//        if (m_aCurrentAnimal != null)
//            return;
//        if (input.interact.wasJustPressed)
//        {
//            DoAction();
//        }
//    }

//    void FixedUpdate()
//    {
//        OnFixedUpdate();
//    }

//    protected virtual void OnFixedUpdate() { }
//    protected virtual void OnUpdate() { }

//    public bool CheckCorrectAnimal(Animal a_animal)
//    {
//        if (m_aRequiredAnimal != ANIMAL_NAME.NONE)
//        {
//            if (a_animal.m_eName != m_aRequiredAnimal)
//                return false;
//        }
//        else if (m_aRequiredSize != ANIMAL_SIZE.NONE)
//        {
//            if (a_animal.m_eSize < m_aRequiredSize)
//                return false;
//        }
//        else if (m_aMaximumSize != ANIMAL_SIZE.NONE)
//        {
//            if (a_animal.m_eSize > m_aMaximumSize)
//                return false;
//        }
//        return true;
//    }

//    public void OnTriggerEnter(Collider a_col)
//    {
//        AnimalTrigger animtrig = a_col.GetComponent<AnimalTrigger>();
//        Animal anim = null;
//        if (animtrig == null)
//        {
//            if (!m_bOnlyHeadTrigger)
//                anim = a_col.GetComponentInParent<Animal>(2);
//            if (anim == null)
//            {
//                ObjectEnter(a_col);
//                return;
//            }
//        }
//        else
//            anim = animtrig.parent;

//        if (CheckCorrectAnimal(anim))
//        {
//            if (!m_lAnimalsIn.Contains(anim))
//            {
//                AnimalEnter(anim);
//                OnAnimalEnter.Invoke();
//            }
//        }
//        else
//        {
//            WrongAnimalEnter(anim);
//            OnWrongAnimalEnter.Invoke();
//        }

//        m_lAnimalsIn.Add(anim);
//    }

//    protected virtual void ObjectEnter(Collider a_col) { }
//    public virtual void AnimalEnter(Animal a_animal) { }
//    protected virtual void WrongAnimalEnter(Animal a_animal) { }

//    public void OnTriggerExit(Collider a_col)
//    {
//        AnimalTrigger animtrig = a_col.GetComponent<AnimalTrigger>();
//        Animal anim = null;
//        if (animtrig == null)
//        {
//            if (!m_bOnlyHeadTrigger)
//                anim = a_col.GetComponentInParent<Animal>(2);
//            if (anim == null)
//            {
//                ObjectExit(a_col);
//                return;
//            }
//        }
//        else
//            anim = animtrig.parent;

//        if (!m_lAnimalsIn.Contains(anim))
//            return;

//        m_lAnimalsIn.Remove(anim);

//        if (anim != null)
//        {
//            if (CheckCorrectAnimal(anim))
//            {
//                if (!m_lAnimalsIn.Contains(anim))
//                {
//                    AnimalExit(anim);
//                    OnAnimalExit.Invoke();
//                }
//            }
//            else
//                OnWrongAnimalExit.Invoke();
//        }
//    }

//    protected virtual void ObjectExit(Collider a_col) { }
//    protected virtual void AnimalExit(Animal a_animal) { }
//    protected virtual void WrongAnimalExit(Animal a_animal) { }
//    public virtual void DoAnimation() { }

//    public virtual void DoAction()
//    {
//        OnDoAction.Invoke();
//    }

//    public virtual void DoActionOn() { }
//    public virtual void DoActionOff() { }

//    public void Detach()
//    {
//        Detach(m_aCurrentAnimal);
//    }

//    public virtual void Detach(Animal anim, bool destroyed = false)
//    {
//        OnDetach.Invoke();
//    }

//    public void SetTimer(bool a_Run)
//    {
//        if (m_bGazeRunning == a_Run)
//            return;

//        m_fGazeTimer = 0f;
//        m_bGazeRunning = a_Run;
//    }

//    protected bool TryDetach(Animal anim = null)
//    {
//        if (anim == null)
//            anim = Animal.CurrentAnimal;

//        if (anim.m_oCurrentObject != null)
//        {
//            if (!m_CanDetach || !anim.m_oCurrentObject.m_CanBeDetached)
//                return false;
//            anim.m_oCurrentObject.Detach(anim);
//        }
//        return true;
//    }

//    private void OnDestroy()
//    {
//        if (m_aCurrentAnimal != null)
//            Detach(m_aCurrentAnimal, true);

//        m_lCurrentObjects.Remove(this);
//    }

//    //=====================================================
//    // STATIC FUNCTIONS

//    /// <summary>
//    /// Removes <paramref name="animals"/> from all <see cref="ActionObject"/>s in the scene
//    /// </summary>
//    /// <param name="animals">The <see cref="Animal"/>s to remove</param>
//    /// <param name="ignore">Optional: ignore these <see cref="ActionObject"/>s</param>
//    public static void RemoveAll(List<Animal> animals, List<ActionObject> ignore = null)
//    {
//        if (ignore == null)
//            ignore = new List<ActionObject>();

//        foreach(var ao in m_lCurrentObjects)
//        {
//            if (ao == null)
//                continue;

//            if (!ignore.Contains(ao))
//            {
//                foreach (var animal in animals)
//                    ao.m_lAnimalsIn.RemoveAll(animal);
//            }
//        }
//    }

//    /// <summary>
//    /// Removes <paramref name="animal"/> from all <see cref="ActionObject"/>s in the scene
//    /// </summary>
//    /// <param name="animal">The <see cref="Animal"/> to remove</param>
//    /// <param name="ignore">Optional: ignore <see cref="ActionObject"/></param>
//    public static void RemoveAll(Animal animal, ActionObject ignore = null)
//    {
//        var ignoreList = new List<ActionObject>();
//        if (ignore != null)
//            ignoreList.Add(ignore);

//        RemoveAll(new List<Animal> { animal }, ignoreList);
//    }

//    /// <summary>
//    /// Removes <paramref name="animal"/> from all <see cref="ActionObject"/>s in the scene
//    /// </summary>
//    /// <param name="animal">The <see cref="Animal"/> to remove</param>
//    /// <param name="ignore">Ignore <see cref="ActionObject"/></param>
//    public static void RemoveAll(Animal animal, List<ActionObject> ignore)
//    {
//        RemoveAll(new List<Animal> { animal }, ignore);
//    }
//    //=====================================================
//}
