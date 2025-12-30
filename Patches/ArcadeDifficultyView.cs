using System;
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
        String songname2 = songName.ToLower();
        String diffname2 = selectedDifficulty.ToLower();
        DifficultyList difficultyList = DifficultyList.GetInstance();
        foreach(String difficulty in difficultyList.GetDifficulties())
        {
            if(difficulty.ToLower() == $"{songname2}/{diffname2}")
            {
                __result = true;
                return true;
            }
        }
        __result = false;
        return false;
    }
}