namespace UNBEATAP.Traps;

public static class Critical
{
    private static bool enableCritical;


    public static bool GetCritical()
    {
        return enableCritical;
    }


    public static void SetCritical(bool enabled)
    {
        enableCritical = enabled;
    }
}