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
        if (RequiredLayer == 0)
        {
            RequiredLayer = (1 << LayerMask.NameToLayer("Animal")) | (1 << LayerMask.NameToLayer("AnimalTrigger"));
        }

        if (!RequiredLayer.Contains(other.gameObject.layer))
            return;

        Animal trig = other.GetComponentInParent<Animal>();
        if (trig == null)
            return;

        if (trig.m_eName != RequiredAnimal && RequiredAnimal != ANIMAL_NAME.NONE)
            return;

        if (trig.m_aMovement.FollowSpline == this || (OnlyIfNotSplining && trig.m_aMovement.FollowSpline != null))
            return;

        if (RequiredKey.control == null)
            trig.GetComponent<AnimalMovement>().SetSpline(Spline);
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

        if (StopOnExit && trig.m_aMovement.FollowSpline == this)
            trig.m_aMovement.StopSpline();

        animalsIn.Remove(trig);
    }

    private void Start()
    {
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
                        animal.m_aMovement.SetSpline(Spline);
                    }
                }
            }
        }
    }
}
