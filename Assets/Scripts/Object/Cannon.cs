using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannon : ActionObject
{
    public override void DoAction()
    {
        if (Animal.CurrentAnimal.m_oCurrentObject != null)
        {

            return;
        }
    }

    protected override void OnUpdate()
    {

    }

    public void Shoot()
    {

    }
}