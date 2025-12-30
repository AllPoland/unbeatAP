using Arcade.Progression;
using Arcade.Unlockables;
using HarmonyLib;
using UNBEATAP.Helpers;

namespace UNBEATAP.Patches;

public class ArcadeCharacterView
{
    [HarmonyPatch(typeof(CharacterUnlocksManager), "GetCharacterState")]
    [HarmonyPrefix]
    static bool GetCharacterStatePatch(ref bool __result, CharacterIndex.Character character)
    {
        if(!Plugin.Client.Connected)
        {
            return true;
        }

        if(CharacterList.GetCharacters().Contains(character.name))
        {
            __result = true;
            return true;
        }

        __result = false;
        return false;
    }
}
