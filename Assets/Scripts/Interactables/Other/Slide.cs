using UnityEngine;
using System.Collections;

public class Slide : AnimalTrigger
{
    public override void AnimalEnter(Animal a_animal)
    {
        Animator a = a_animal.GetComponentInChildren<Animator>();
        a.SetBool("OnSlide", true);
    }

    public override void AnimalExit(Animal a_animal)
    {
        Animator a = a_animal.GetComponentInChildren<Animator>();
        a.SetBool("OnSlide", false);
    }
}
