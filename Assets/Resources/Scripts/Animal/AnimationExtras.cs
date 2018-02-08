using UnityEngine;
using System;

public class AnimationExtras : MonoBehaviour
{
    public void LeftFoot()
    {

    }

    public void RightFoot()
    {

    }

    public void TurnEnd()
    {
        GetComponentInParent<Animal>().FinishTurning();
    }

    public void IKPoodleStepCorection()
    {
        GetComponentInParent<Poodle>();
    }
}
