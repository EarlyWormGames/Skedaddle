using UnityEngine;
using System.Collections;

/// <summary>
/// quick animation handling for the loris on the slide
/// Loris interaction with the slide. 
/// </summary>
public class Slide : AnimalTrigger
{
    protected override bool HeadTriggerOnly { get; set; }
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
