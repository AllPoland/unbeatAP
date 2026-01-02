using Challenges;
using HarmonyLib;

namespace UNBEATAP.Patches;

[HarmonyPatch(typeof(BaseChallengeDescriptor))]
public class BaseChallengeDescriptorPatch
{
    [HarmonyPatch("NotifyChallengeUnlocked")]
    [HarmonyPrefix]
    static bool NotifyChallengeUnlockedPrefix(BaseChallengeDescriptor __instance)
    {
        if(!Plugin.Client.Connected)
        {
            return true;
        }

        // Here we avoid triggering OnChallengeCompleted so no popup
        return false;
    }


    [HarmonyPatch("CheckAchievement")]
    [HarmonyPrefix]
    static bool CheckAchievementPrefix()
    {
        if(!Plugin.Client.Connected)
        {
            return true;
        }

        // We just want to stop achievements being sent
        return false;
    }
}