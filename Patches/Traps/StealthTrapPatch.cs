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
        if(Stealth.GetStealth() && Plugin.Client.Connected)
        {
            __instance.hiding = true;
        }
    }
}