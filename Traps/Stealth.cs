namespace UNBEATAP.Traps;

public static class Stealth
{
    private static bool enableStealth;
    

    public static bool GetStealth()
    {
        return enableStealth;
    }


    public static void SetStealth(bool enabled)
    {
        enableStealth = enabled;
    }
}