namespace UNBEATAP.Traps;

public static class Stealth
{
    public static TrapTimer Timer = new TrapTimer();
    

    public static bool GetStealth()
    {
        return Timer.GetActive();
    }
}