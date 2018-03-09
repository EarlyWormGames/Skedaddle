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

    public void StartDig()
    {
        GetComponentInParent<Animal>().m_oCurrentObject.DoAction();
    }

    public void FinishDig()
    {
        var dig = (Dig)GetComponentInParent<Animal>().m_oCurrentObject;
        dig.Finish();
    }

    public void BuildBridge()
    {
        var bridge = (BridgeMaker)GetComponentInParent<Animal>().m_oCurrentObject;
        if (bridge == null)
            return;

        bridge.BuildBridge();
    }
}
