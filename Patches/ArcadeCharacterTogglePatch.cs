using Arcade.Unlockables;
using HarmonyLib;

namespace UNBEATAP.Patches;

[HarmonyPatch(typeof(ArcadeCharacterToggle))]
public class ArcadeCharacterTogglePatch
{
    [HarmonyPatch("OnToggleChanged")]
    [HarmonyPrefix]
    static bool OnToggleChangedPrefix(ArcadeCharacterToggle __instance, bool state)
    {
        if(!Plugin.Client.Connected)
        {
            return true;
        }

        Traverse traverse = new Traverse(__instance);
        bool _isPrimary = traverse.Field("_isPrimary").GetValue<bool>();
        CharacterIndex.Character _character = traverse.Field("_character").GetValue<CharacterIndex.Character>();

        if(_isPrimary)
        {
            // Here is the changed code
            Plugin.Client.SetPrimaryCharacter(_character.name);
        }
        else
        {
            Plugin.Client.SetSecondaryCharacter(_character.name);
        }

        return false;
    }
}