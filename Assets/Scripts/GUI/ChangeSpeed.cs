public class ChangeSpeed : AnimalTrigger
{

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
