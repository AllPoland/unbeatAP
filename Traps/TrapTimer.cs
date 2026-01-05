namespace UNBEATAP.Traps;

public class TrapTimer
{
    public float Timer;

    public float Duration;


    public TrapTimer()
    {
        Duration = 10f;
        Timer = 0f;
    }


    public TrapTimer(float duration)
    {
        Duration = duration;
        Timer = 0f;
    }


    public void Activate()
    {
        Timer = Duration;
    }


    public void Deactivate()
    {
        Timer = 0f;
    }


    public void Update(float deltaTime)
    {
        if(Timer > 0f)
        {
            Timer -= deltaTime;
        }
    }


    public bool GetActive()
    {
        return Timer > 0f;
    }
}