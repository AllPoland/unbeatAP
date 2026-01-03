using System;
using HarmonyLib;

namespace UNBEATAP.Patches;

[HarmonyPatch(typeof(SongRatingDisplay))]
public class SongRatingDisplayPatch
{
    [HarmonyPatch("Fill")]
    [HarmonyPrefix]
    static bool FillPrefix(ref Tuple<string, float> rating)
    {
        if(!Plugin.Client.Connected)
        {
            return true;
        }

        // Rating is divided by 25 for display, but we want to undo that
        // because our rating is stored as the actual contribution
        rating = new Tuple<string, float>(rating.Item1, rating.Item2 * 25f);
        return true;
    }
}