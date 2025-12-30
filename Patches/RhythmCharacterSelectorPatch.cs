using HarmonyLib;

namespace UNBEATAP.Patches;

[HarmonyPatch(typeof(RhythmCharacterSelector))]
public class RhythmCharacterSelectorPatch
{
    [HarmonyPatch("Awake")]
    [HarmonyPrefix]
    static bool AwakePrefix(RhythmCharacterSelector __instance)
    {
        if(!Plugin.Client.Connected)
        {
            return true;
        }

        if (JeffBezosController.rhythmGameType == RhythmGameType.ArcadeMode)
        {
            __instance.InstantiatePlayer(Plugin.Client.primaryCharacter);
            __instance.InstantiateAssist(Plugin.Client.secondaryCharacter);
            __instance.InstantiateBG(Plugin.Client.primaryCharacter, Plugin.Client.secondaryCharacter);
        }
        return false;
    }
}