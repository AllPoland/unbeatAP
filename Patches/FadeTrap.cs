using HarmonyLib;
using Rhythm;
using UNBEATAP.Traps;
namespace UNBEATAP.Patches;

public class FadeTrap
{
    [HarmonyPatch(typeof(BaseNote), "Init")]
    [HarmonyPostfix]
    static void HidingPatch(ref BaseNote __instance)
    {
        if(Fading.GetFade() && Plugin.Client.Connected)
        {
            __instance.hiding = true;
        }
    }
}