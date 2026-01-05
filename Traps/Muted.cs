using FMODUnity;

namespace UNBEATAP.Traps;

public static class Muted
{
    public static void Mute()
    {
        RuntimeManager.GetBus("bus:/music").setMute(true);
    }


    public static void Unmute()
    {
        RuntimeManager.GetBus("bus:/music").setMute(false);
    }


    public static bool IsMuted()
    {
        RuntimeManager.GetBus("bus:/music").getMute(out bool muted);
        return muted;
    }
}