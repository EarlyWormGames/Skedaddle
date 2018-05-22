

/// <summary>
/// Change the speed of an animal if they enter the trigger
/// </summary>
public class ChangeSpeed : AnimalTrigger
{
    protected override bool HeadTriggerOnly { get; set; }
    public float SpeedChange;
    private static float InitialSpeed;

    public override void AnimalEnter(Animal a_animal)
    {
        if (InitialSpeed == 0) InitialSpeed = a_animal.m_fTopSpeed;
        a_animal.m_fTopSpeed = SpeedChange;
    }

    public override void AnimalExit(Animal animal)
    {

    }
}
