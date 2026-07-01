using HarmonyLib;

namespace UNBEATAP.Patches;

[HarmonyPatch(typeof(CrossPlatform.Platforms.PlatformSteam))]
public class PlatformSteamPatch
{
    [HarmonyPatch("UnlockAchievement")]
    [HarmonyPrefix]
    static bool UnlockAchievementPrefix()
    {
        // kill the 10* achievement bug and all that may come after
        if(!Plugin.Client.Connected)
        {
            return true;
        }

        return false;
    }
}

