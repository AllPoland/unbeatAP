using Arcade.Progression;
using HarmonyLib;

namespace UNBEATAP.Patches;

[HarmonyPatch(typeof(MainProgressionContainer))]
public class MainProgressionContainerPatch
{
    [HarmonyPatch("QueueUnlock")]
    [HarmonyPrefix]
    static bool QueueUnlockPrefix()
    {
        if(!Plugin.Client.Connected)
        {
            return true;
        }

        // Disable any progression unlock triggers
        return false;
    }


    [HarmonyPatch("SetUnlock")]
    [HarmonyPrefix]
    static bool SetUnlockPrefix()
    {
        if(!Plugin.Client.Connected)
        {
            return true;
        }

        // Disable any progression unlock triggers
        return false;
    }
    
    
    [HarmonyPatch("GetUnlockName")]
    [HarmonyPrefix]
    static bool UnlockNamePatch(ref MainProgressionContainer.Unlock unlock, ref string __result)
    {
        if(unlock.UnlockManager == "Archipelago" && Plugin.Client.Connected)
        {
            // If the unlock manager is named Archipelago, make the name what we want.
            __result = unlock.UnlockName;
            return false;
        }

        return true;
    }
}