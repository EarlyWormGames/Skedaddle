using System.Collections.Generic;

public abstract class Attachable : AnimalTrigger
{
    protected static List<Attachable> CurrentAttachables = new List<Attachable>();

    ///<summary>The currently attached <see cref="Animal"/></summary>
    public Animal AttachedAnimal { get; protected set; }
    ///<summary>Does this object prevent the <see cref="AttachedAnimal"/> from turning?</summary>
    public bool BlocksTurn { get; protected set; }
    ///<summary>Does this object prevent the <see cref="AttachedAnimal"/> from moving?</summary>
    public bool BlocksMovement { get; protected set; }
    ///<summary>Can this object force others to detach?</summary>
    public bool CanDetach { get; protected set; }

    ///<summary>A collection of <see cref="Animal"/>s that are currently inside this object's trigger(s)</summary>
    protected bool BeingDestroyed;

    sealed public override void AnimalEnter(Animal animal)
    {
        OnAnimalEnter(animal);
    }

    sealed public override void AnimalExit(Animal animal)
    {
        OnAnimalExit(animal);
    }

    public bool TryDetachOther(Animal animal = null)
    {
        if (animal == null)
            animal = Animal.CurrentAnimal;

        if (animal.currentAttached != null)
        {
            if (CanDetach)
                return animal.currentAttached.Detach(this);
            else
                return false;
        }
        return true;
    }

    #region Static Methods
    //=====================================================
    /// <summary>
    /// Removes <paramref name="animals"/> from all <see cref="ActionObject"/>s in the scene
    /// </summary>
    /// <param name="animals">The <see cref="Animal"/>s to remove</param>
    /// <param name="ignore">Optional: ignore these <see cref="ActionObject"/>s</param>
    public static void RemoveAll(List<Animal> animals, List<Attachable> ignore = null)
    {
        if (ignore == null)
            ignore = new List<Attachable>();

        foreach (var ao in CurrentAttachables)
        {
            if (ao == null)
                continue;

            if (!ignore.Contains(ao))
            {
                foreach (var animal in animals)
                    ao.AnimalsIn.RemoveAll(animal);
            }
        }
    }

    /// <summary>
    /// Removes <paramref name="animal"/> from all <see cref="ActionObject"/>s in the scene
    /// </summary>
    /// <param name="animal">The <see cref="Animal"/> to remove</param>
    /// <param name="ignore">Optional: ignore <see cref="ActionObject"/></param>
    public static void RemoveAll(Animal animal, Attachable ignore = null)
    {
        var ignoreList = new List<Attachable>();
        if (ignore != null)
            ignoreList.Add(ignore);

        RemoveAll(new List<Animal> { animal }, ignoreList);
    }

    /// <summary>
    /// Removes <paramref name="animal"/> from all <see cref="ActionObject"/>s in the scene
    /// </summary>
    /// <param name="animal">The <see cref="Animal"/> to remove</param>
    /// <param name="ignore">Ignore <see cref="ActionObject"/></param>
    public static void RemoveAll(Animal animal, List<Attachable> ignore)
    {
        RemoveAll(new List<Animal> { animal }, ignore);
    }
    //=====================================================
    #endregion

    #region Public Methods
    //=====================================================
    ///<summary>Called when interact key pressed and was selected for use</summary>
    public void Attach(Animal attachTo)
    {
        AttachedAnimal = attachTo;
        AttachedAnimal.AttachInteractable(this);
        OnAttach();
    }

    ///<summary>Called to detach the interactabe from an <see cref="Animal"/></summary>
    public bool Detach(Attachable caller, Animal animal = null)
    {
        if (!CheckDetach() && caller != this)
            return false;

        AttachedAnimal.DetachInteractable();
        if (animal == null)
            animal = AttachedAnimal;
        OnDetach(animal);
        AttachedAnimal = null;

        return true;
    }
    //=====================================================
    #endregion

    #region Virtual Methods
    //=====================================================
    ///<summary>Called when attaching to an <see cref="Animal"/></summary>
    protected virtual void OnAttach() { }
    ///<summary>Called when detaching from an <see cref="Animal"/></summary>
    protected virtual void OnDetach(Animal animal) { }
    ///<summary>Check if this object is attachable</summary>
    public virtual bool CheckAttach() { return true; }
    ///<summary>Check if this object is detachable</summary>
    protected virtual bool CheckDetach() { return true; }

    protected virtual void OnAnimalEnter(Animal animal) { }
    protected virtual void OnAnimalExit(Animal animal) { }
    protected virtual void OnAwake() { }
    protected virtual void OnStart() { }
    protected virtual void OnUpdate() { }
    protected virtual void OnDestroying() { }
    //=====================================================
    #endregion

    #region Unity Callbacks
    //=====================================================
    private void Awake()
    {
        CurrentAttachables.Add(this);
        OnAwake();
    }

    private void Start()
    {
        OnStart();
    }

    private void Update()
    {
        OnUpdate();
    }

    private void OnDestroy()
    {
        BeingDestroyed = true;

        if (AttachedAnimal != null)
            Detach(this);

        CurrentAttachables.Remove(this);
        OnDestroying();
    }
    //=====================================================
    #endregion
}