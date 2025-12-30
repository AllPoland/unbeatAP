using HarmonyLib;
using Rhythm;
using UNBEATAP.Traps;
namespace UNBEATAP.Patches;

public class FadeTrap
{
    [HarmonyPatch(typeof(HitObjectInfo), "IsHiding")]
    [HarmonyPrefix]
    static bool HidingPatch(ref bool __result)
    {
        if(Fading.GetFade() && Plugin.Client.Connected)
        {
            __result = true;
            return false;
        }
        return true;
    }
}