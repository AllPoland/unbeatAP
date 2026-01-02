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
}