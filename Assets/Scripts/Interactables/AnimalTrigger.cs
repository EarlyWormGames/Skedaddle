using UnityEngine;

public abstract class AnimalTrigger : MonoBehaviour
{
    public ANIMAL_SIZE RequiredSize;
    public ANIMAL_NAME RequiredAnimal;
    public ANIMAL_SIZE MaximumSize;
    public bool OnlyHeadTrigger;

    ///<summary>Check if this object is able to interact with this Animal</summary>
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
        AnimalHeadTrigger animtrig = other.GetComponent<AnimalHeadTrigger>();
        Animal anim = null;
        if (animtrig == null)
        {
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
            AnimalEnter(anim);
        else
            WrongAnimalEnter(anim);
    }

    private void OnTriggerExit(Collider other)
    {
        AnimalHeadTrigger animtrig = other.GetComponent<AnimalHeadTrigger>();
        Animal anim = null;
        if (animtrig == null)
        {
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
            AnimalExit(anim);
        else
            WrongAnimalExit(anim);
    }

    public abstract void AnimalEnter(Animal animal);
    public abstract void AnimalExit(Animal animal);

    protected virtual void WrongAnimalEnter(Animal other) { }
    protected virtual void WrongAnimalExit(Animal other) { }

    protected virtual void ObjectEnter(Collider other) { }
    protected virtual void ObjectExit(Collider other) { }
}