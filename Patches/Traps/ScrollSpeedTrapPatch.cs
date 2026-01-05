using HarmonyLib;
using Rhythm;
using UNBEATAP.Traps;

namespace UNBEATAP.Patches;

public class ScrollSpeedTrapPatch
{
    [HarmonyPatch(typeof(RhythmController), "noteTravelTime", MethodType.Getter)]
    [HarmonyPostfix]
    static void ScrollSpeedPatch(ref float __result)
    {
        if(ScrollSpeed.GetZoom() && Plugin.Client.Connected)
        {
            __result *= 0.5f;
        }

        if(ScrollSpeed.GetCrawl() && Plugin.Client.Connected)
        {
            __result *= 1.5f;
        }
    }
}