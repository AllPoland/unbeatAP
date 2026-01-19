using Rhythm;
using UNBEATAP.AP;

namespace UNBEATAP.Helpers;

public static class DeathHelper
{
    public static bool HandleDeathSilent { get; private set; }

    public static bool HandledDeath = false;


    public static bool KillPlayer()
    {
        if(!RhythmController.Instance)
        {
            // Can't kill the player while not playing
            return false;
        }

        Plugin.Logger.LogInfo($"Received death link, killing player.");
        HandleDeathSilent = true;
        RhythmController.Instance.song.health = 0f;
        return true;
    }


    public static void OnPlayerDeath(DeathLinkReason reason)
    {
        if(!Plugin.Client.deathLink)
        {
            return;
        }

        if(HandleDeathSilent)
        {
            HandleDeathSilent = false;
            return;
        }

        if(HandledDeath)
        {
            return;
        }

        DeathLinkController.SendDeathLink(reason);
    }


    public static void Reset()
    {
        HandleDeathSilent = false;
        HandledDeath = false;
    }
}