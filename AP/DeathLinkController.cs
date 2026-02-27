using System;
using System.Collections.Generic;
using Archipelago.MultiClient.Net.BounceFeatures.DeathLink;
using UNBEATAP.Helpers;

namespace UNBEATAP.AP;

public static class DeathLinkController
{
    public static readonly Dictionary<DeathLinkReason, string> DeathLinkMessage = new Dictionary<DeathLinkReason, string>
    {
        {DeathLinkReason.Fail, "{player} couldn't keep up."},
        {DeathLinkReason.Quit, "{player} lacked confidence."},
        {DeathLinkReason.Restart, "{player} wanted to run it back."}
    };


    public static void TryPerformDeathLink(DeathLink deathLink)
    {
        DeathHelper.KillPlayer();
    }


    public static void SendDeathLink(DeathLinkReason reason)
    {
        if(!Plugin.Client.Connected || !Plugin.Client.deathLink || Plugin.Client.DeathLinkService == null)
        {
            Plugin.Logger.LogInfo("DeathLink is disabled");
            return;
        }

        if((Plugin.Client.deathLinkBehavior & reason) == 0)
        {
            Plugin.Logger.LogInfo($"Death link masked by options: {reason}");
            return;
        }

        DeathLinkService service = Plugin.Client.DeathLinkService;
        string message = DeathLinkMessage[reason].Replace("{player}", Plugin.Client.slot);
        DeathLink deathLink = new DeathLink(Plugin.Client.slot, message);

        Plugin.Logger.LogInfo($"Sending death link with message: {message}");
        service.SendDeathLink(deathLink);
    }
}


[Flags]
public enum DeathLinkReason
{
    Fail = 1,
    Quit = 2,
    Restart = 4
}