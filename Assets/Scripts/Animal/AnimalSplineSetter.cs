using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputNew;

public class AnimalSplineSetter : MonoBehaviour
{
    public ANIMAL_NAME RequiredAnimal;
    public SplineMovement Spline;
    public ButtonAction RequiredKey;
    public bool StopOnExit;
    public LayerMask RequiredLayer;

    [Tooltip("Will it only trigger if the animal isn't using a spline?")]
    public bool OnlyIfNotSplining = false;

    private List<Animal> animalsIn = new List<Animal>();

    private void OnTriggerEnter(Collider other)
    {
        //Set the default value for the layer if it isn't set
        if (RequiredLayer == 0)
        {
            RequiredLayer = (1 << LayerMask.NameToLayer("Animal")) | (1 << LayerMask.NameToLayer("AnimalTrigger"));
        }

        //Checks that the object is on the correct layer
        if (!RequiredLayer.Contains(other.gameObject.layer))
            return;

        //We also kinda need an animal
        Animal trig = other.GetComponentInParent<Animal>();
        if (trig == null)
            return;

        //Sometimes we need a specific animal
        if (trig.m_eName != RequiredAnimal && RequiredAnimal != ANIMAL_NAME.NONE)
            return;

        //Ignore if they're on this spline, or if on something else and we can't override
        if (trig.m_aMovement.FollowSpline == this || (OnlyIfNotSplining && trig.m_aMovement.FollowSpline != null))
            return;

        //Sometimes we move on a specific key
        if (RequiredKey.control == null)
        {
            //Ignore if the animal is pushing an object and isn't allowed to enter a spline
            if (trig.currentAttached != null && trig.currentAttached.GetType().IsAssignableFrom(typeof(PPObject)))
            {
                if (!(trig.currentAttached as PPObject).CanEnterMoveSpline)
                    return;
            }

            trig.GetComponent<AnimalMovement>().SetSpline(Spline);
        }
        else
            animalsIn.Add(trig);
    }

    private void OnTriggerExit(Collider other)
    {
        if (animalsIn == null)
            return;

        Animal trig = other.GetComponentInParent<Animal>();
        if (trig == null)
            return;

        if (!animalsIn.Contains(trig))
            return;

        //Stop splining on leaving the trigger if appropriate
        if (StopOnExit && trig.m_aMovement.FollowSpline == this)
            trig.m_aMovement.StopSpline();

        animalsIn.Remove(trig);
    }

    private void Start()
    {
        //Bind the keypress
        if (RequiredKey.action != null)
            RequiredKey.Bind(GameManager.Instance.input.handle);
    }

    private void Update()
    {
        if (animalsIn != null)
        {
            foreach (var animal in animalsIn)
            {
                if (animal.m_aMovement.FollowSpline != Spline)
                {
                    if (RequiredKey.control.isHeld)
                    {
                        //Ignore if the animal is pushing an object and isn't allowed to enter a spline
                        if (animal.currentAttached != null && animal.currentAttached.GetType().IsAssignableFrom(typeof(PPObject)))
                        {
                            if (!(animal.currentAttached as PPObject).CanEnterMoveSpline)
                                continue;
                        }

                        animal.m_aMovement.SetSpline(Spline);
                    }
                }
            }
        }
    }
}
