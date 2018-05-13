using System.Collections.Generic;
using UnityEngine;

public abstract class AnimalTrigger : MonoBehaviour
{
    public ANIMAL_SIZE RequiredSize;
    public ANIMAL_NAME RequiredAnimal;
    public ANIMAL_SIZE MaximumSize;
    public bool OnlyHeadTrigger;

    protected List<Animal> AnimalsIn = new List<Animal>();

    ///<summary>Check if this object is able to interact with this <see cref="Animal"/></summary>
    public bool AllowsAnimal(Animal animal)
    {
        if (RequiredAnimal != ANIMAL_NAME.NONE)
        {
            if (animal.m_eName != RequiredAnimal)
                return false;
        }
        else if (RequiredSize != ANIMAL_SIZE.NONE)
        {
            if (animal.m_eSize < RequiredSize)
                return false;
        }
        else if (MaximumSize != ANIMAL_SIZE.NONE)
        {
            if (animal.m_eSize > MaximumSize)
                return false;
        }
        return true;
    }

    private void OnTriggerEnter(Collider other)
    {
        //Search for an animal head trigger
        AnimalHeadTrigger animtrig = other.GetComponent<AnimalHeadTrigger>();
        Animal anim = null;
        if (animtrig == null)
        {
            //Or search for an animal if not found
            if (!OnlyHeadTrigger)
                anim = other.GetComponentInParent<Animal>(2);
            if (anim == null)
            {
                ObjectEnter(other);
                return;
            }
        }
        else
            anim = animtrig.parent;

        if (anim == null)
        {
            ObjectEnter(other);
            return;
        }

        if (AllowsAnimal(anim))
        {
            AnimalsIn.Add(anim);
            AnimalEnter(anim);
        }
        else
            WrongAnimalEnter(anim);
    }

    private void OnTriggerExit(Collider other)
    {
        //Search for an animal head trigger
        AnimalHeadTrigger animtrig = other.GetComponent<AnimalHeadTrigger>();
        Animal anim = null;
        if (animtrig == null)
        {
            //Or search for an animal if not found
            if (!OnlyHeadTrigger)
                anim = other.GetComponentInParent<Animal>(2);
            if (anim == null)
            {
                ObjectExit(other);
                return;
            }
        }
        else
            anim = animtrig.parent;

        if (anim == null)
        {
            ObjectExit(other);
            return;
        }

        if (AllowsAnimal(anim))
        {
            AnimalsIn.Remove(anim);
            AnimalExit(anim);
        }
        else
            WrongAnimalExit(anim);
    }

    /// <summary>
    /// An <see cref="Animal"/> has entered the trigger
    /// </summary>
    public abstract void AnimalEnter(Animal animal);
    /// <summary>
    /// An <see cref="Animal"/> has exited the trigger
    /// </summary>
    public abstract void AnimalExit(Animal animal);

    /// <summary>
    /// An incorrect <see cref="Animal"/> has entered the trigger
    /// </summary>
    protected virtual void WrongAnimalEnter(Animal other) { }
    /// <summary>
    /// An incorrect <see cref="Animal"/> has exited the trigger
    /// </summary>
    protected virtual void WrongAnimalExit(Animal other) { }

    /// <summary>
    /// An object other than an <see cref="Animal"/> has entered the trigger
    /// </summary>
    protected virtual void ObjectEnter(Collider other) { }
    /// <summary>
    /// An object other than an <see cref="Animal"/> has exited the trigger
    /// </summary>
    protected virtual void ObjectExit(Collider other) { }
}