using HarmonyLib;
using Rhythm;
using UNBEATAP.Helpers;

namespace UNBEATAP.Patches;

[HarmonyPatch(typeof(BeatmapIndex))]
public class BeatmapIndexPatch
{
    [HarmonyPatch(nameof(BeatmapIndex.VisibleDifficulties), MethodType.Getter)]
    [HarmonyPrefix]
    static bool GetVisibleDifficultiesPrefix(ref string[] __result)
    {
        if(!Plugin.Client.Connected)
        {
            return true;
        }

        __result = DifficultyList.GetVisibleDifficulties().ToArray();
        return false;
    }
}