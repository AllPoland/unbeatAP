using System;
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
        CharacterList characterList = CharacterList.GetInstance();
        foreach(String unlock in characterList.GetCharacters())
        {
            if(character.name.ToLower() == unlock)
            {
                __result = true;
                return true;
            }
        }
        __result = false;
        return false;
    }
}
