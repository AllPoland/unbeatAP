using HarmonyLib;
using Rhythm;
using UNBEATAP.Traps;

namespace UNBEATAP.Patches;

public class CriticalTrapPatch
{
    private static bool initialCritEnabled;
    [HarmonyPatch(typeof(FileStorageController), "SaveSettings")]
    [HarmonyPrefix]
    static bool DontSaveCritStatus()
    {
        // Make sure it doesn't save critical being enabled if user didn't manually enable it
        if(initialCritEnabled && Plugin.Client.Connected)
        {
            return false;
        }
        return true;
    }


    [HarmonyPatch(typeof(StorableBeatmapOptions), "useCriticalMode", MethodType.Getter)]
    [HarmonyPrefix]
    static bool UseCriticalModePatch(ref bool __result)
    {
        // Enables critical when trap is triggered, does not change if user manually enabled critical
        if(Critical.GetCritical() && Plugin.Client.Connected)
        {
            __result = true;
            return false;
        }
        return true;
    }


    [HarmonyPatch(typeof(LevelManager), "LoadArcadeLevel")]
    [HarmonyPrefix]
    static void EnableCriticalAtLoad()
    {
        // Enable critical at map load, if you don't then enabling critical mid-song will mess things up
        if(Plugin.Client.Connected)
        {
            initialCritEnabled = Critical.GetCritical();
            Critical.SetCritical(true);
        }
    }


    [HarmonyPatch(typeof(LevelManager), "RestartLevel")]
    [HarmonyPrefix]
    static void EnableCriticalOnRestart()
    {
        // Same as EnableCriticalOnLoad, but handles map restart edge case
        if(Plugin.Client.Connected)
        {
            Critical.SetCritical(true);
        }
    }


    [HarmonyPatch(typeof(RhythmController), "InitializeAndPlay")]
    [HarmonyPostfix]
    static void DisableCriticalAtLoad()
    {
        // Disables critical at load, unless user manually enabled or trap was already enabled
        if(!initialCritEnabled && Plugin.Client.Connected)
        {
            Critical.SetCritical(false);
        }
    }
}