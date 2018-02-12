using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannon : ActionObject
{
    public BezierSplineFollower SplineLeft, SplineRight;

    private Loris loris;
    private bool facingLeft = false;

    public override void DoAction()
    {
        if (!TryDetach())
            return;

        m_aCurrentAnimal = Animal.CurrentAnimal;
        loris = (Loris)m_aCurrentAnimal;
        loris.m_bInCannon = true;
        loris.m_rBody.isKinematic = true;
    }

    protected override void OnUpdate()
    {

    }

    public void Shoot()
    {

    }
}