namespace UNBEATAP.Traps;

public static class Critical
{
    public static TrapTimer Timer = new TrapTimer();


    public static bool GetCritical()
    {
        return Timer.GetActive();
    }
}