using UnityEngine;
using System;

public class AnimationExtras : MonoBehaviour
{
    internal LeverPlug m_lpLever;
    internal SplineFollower m_sfDigPath;
    internal BridgeMaker m_bmTongueBridge;

    public void LeftFoot()
    {

    }

    public void RightFoot()
    {

    }

    public void DoAnimation()
    {
        if (m_lpLever != null)
            m_lpLever.DoAnimation();
    }

    public void DoAction()
    {
        if (m_lpLever != null)
            m_lpLever.DoAction();
    }

    public void BuildBridge()
    {
        if(m_bmTongueBridge != null)
        m_bmTongueBridge.BuildBridge();
    }

    public void StopAnimation()
    {
        if (m_lpLever != null)
            m_lpLever.StopAnimation();
    }

    public void StartDig()
    {
        m_sfDigPath.StartFollow();
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
