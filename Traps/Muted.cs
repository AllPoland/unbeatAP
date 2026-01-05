using FMODUnity;

namespace UNBEATAP.Traps;

public static class Muted
{
    public static TrapTimer Timer = new TrapTimer();

    public static bool IsMuted { get; private set; }


    public static void Mute()
    {
        RuntimeManager.GetBus("bus:/music").setMute(true);
        IsMuted = true;
    }


    public static void UnMute()
    {
        RuntimeManager.GetBus("bus:/music").setMute(false);
        IsMuted = false;
    }


    public static void Deactivate()
    {
        Timer.Deactivate();
        if(IsMuted)
        {
            UnMute();
        }
    }
}