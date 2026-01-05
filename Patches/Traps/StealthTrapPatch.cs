using HarmonyLib;
using Rhythm;
using UNBEATAP.Traps;
namespace UNBEATAP.Patches;

public class StealthTrapPatch
{
    [HarmonyPatch(typeof(BaseNote), "Init")]
    [HarmonyPostfix]
    static void HidingPatch(ref BaseNote __instance)
    {
        if(!Plugin.Client.Connected)
        {
            return;
        }

        if(Stealth.GetStealth())
        {
            __instance.hiding = true;
        }
    }
}