using UnityEngine;
using System.Collections;

public class SplineWalker : SplineFollower
{
    protected override void OnUpdate()
    {
        base.OnUpdate();

        if (m_bFollow && m_aAnimal != null)
        {
            m_aAnimal.m_bForceWalk = true;
            m_aAnimal.m_bCheckForFall = false;
        }
    }

    protected override void OnFinish()
    {
        base.OnFinish();
        m_aAnimal.m_bForceWalk = false;
    }
}
