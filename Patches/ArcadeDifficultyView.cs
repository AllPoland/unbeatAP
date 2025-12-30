using Arcade.Progression;
using HarmonyLib;
using UNBEATAP.Helpers;

namespace UNBEATAP.Patches;

public class ArcadeDifficultyView
{
    [HarmonyPatch(typeof(DifficultyUnlocksManager), "GetSongState")]
    [HarmonyPrefix]
    static bool GetDifficultyUnlockPatch(ref bool __result, string songName, string selectedDifficulty)
    {
        if(!Plugin.Client.Connected)
        {
            return true;
        }

        string fullName = $"{songName.ToLower()}{DifficultyList.NameSeparator}{selectedDifficulty.ToLower()}";
        if(DifficultyList.GetDifficulties().Contains(fullName))
        {
            __result = true;
            return true;
        }

        __result = false;
        return false;
    }
}