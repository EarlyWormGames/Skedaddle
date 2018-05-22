using UnityEngine;
using System.Collections;

public class SecretCameraMover : AnimalTrigger
{
    public Camera m_cMain;
    public Vector2 m_newXLimits;
    public Vector2 m_newYLimits;
    protected override bool HeadTriggerOnly { get; set; }

    public override void AnimalEnter(Animal a_animal)
    {
        m_cMain.GetComponent<CameraController>().m_v2XLimits = m_newXLimits;
        m_cMain.GetComponent<CameraController>().m_v2YLimits = m_newYLimits;
    }

    public override void AnimalExit(Animal animal)
    {

    }
}
