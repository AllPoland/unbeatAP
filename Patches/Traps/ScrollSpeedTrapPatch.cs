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
        if(!Plugin.Client.Connected)
        {
            return;
        }

        if(ScrollSpeed.GetZoom())
        {
            __result *= 0.75f;
        }

        if(ScrollSpeed.GetCrawl())
        {
            __result *= 1.75f;
        }
    }
}