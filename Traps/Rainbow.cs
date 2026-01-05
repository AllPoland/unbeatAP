namespace UNBEATAP.Traps;

public static class Rainbow
{
    private static bool enableRainbow;


    public static bool GetRainbow()
    {
        return enableRainbow;
    }


    public static void SetRainbow(bool enabled)
    {
        enableRainbow = enabled;
    }
}