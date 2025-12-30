using Arcade.Unlockables;
using HarmonyLib;
using TMPro;
using UnityEngine.UI;

namespace UNBEATAP.Patches;

[HarmonyPatch(typeof(ArcadeCharacterToggle))]
public class ArcadeCharacterTogglePatch
{
    [HarmonyPatch(nameof(ArcadeCharacterToggle.Setup))]
    [HarmonyPrefix]
    static bool SetupPrefix(ArcadeCharacterToggle __instance, CharacterIndex.Character character, bool isPrimary, ToggleGroup toggleGroup)
    {
        if(!Plugin.Client.Connected)
        {
            return true;
        }

        Traverse traverse = new Traverse(__instance);

        TextMeshProUGUI[] characterName = traverse.Field("characterName").GetValue<TextMeshProUGUI[]>();
        for (int i = 0; i < characterName.Length; i++)
        {
            characterName[i].text = character.name;
        }

        Toggle _toggle = __instance.GetComponent<Toggle>();
        _toggle.onValueChanged.AddListener((state) => traverse.Method("OnToggleChanged").GetValue(state));
        _toggle.group = toggleGroup;
        bool select = (isPrimary ? Plugin.Client.primaryCharacter : Plugin.Client.secondaryCharacter) == character.name;
        _toggle.SetIsOnWithoutNotify(select);

        traverse.Field("_isPrimary").SetValue(isPrimary);
        traverse.Field("_toggle").SetValue(_toggle);
        traverse.Field("_character").SetValue(character);
        return false;
    }


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