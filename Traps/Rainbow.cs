namespace UNBEATAP.Traps;

public static class Rainbow
{
    public static TrapTimer Timer = new TrapTimer();


    public static bool GetRainbow()
    {
        return Timer.GetActive();
    }
}