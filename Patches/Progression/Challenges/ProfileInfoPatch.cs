using Challenges;
using HarmonyLib;

namespace UNBEATAP.Patches;

[HarmonyPatch(typeof(ProfileInfo))]
public class ProfileInfoPatch
{
    [HarmonyPatch("LoadChabo")]
    [HarmonyPrefix]
    static bool LoadChaboPrefix(ref bool __result, string chabo, out SuperChaboDescriptor.ChaboSaveStructure saveData)
    {
        if(!Plugin.Client.Connected)
        {
            saveData = default(SuperChaboDescriptor.ChaboSaveStructure);
            return true;
        }

        // Disable loading all challenge boards
        // This could probably be replaced with a call to AP datastorage once challenges are added
        saveData = default(SuperChaboDescriptor.ChaboSaveStructure);
        __result = false;
        return false;
    }


    [HarmonyPatch("SaveChabo")]
    [HarmonyPrefix]
    static bool SaveChaboPrefix(string chabo, SuperChaboDescriptor.ChaboSaveStructure save)
    {
        if(!Plugin.Client.Connected)
        {
            return true;
        }

        // Disable saving all challenge boards
        // This could also be replaced with a call to AP datastorage
        return false;
    }
}