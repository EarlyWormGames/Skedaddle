using UnityEngine;
using System.Collections;

public class Zebra : Animal
{
    protected override void OnAwake()
    {
        m_eName = ANIMAL_NAME.ZEBRA;
        m_eSize = ANIMAL_SIZE.L;
    }

    protected override void OnUpdate()
    {

    }
}
