using Arcade.UI.YourProfile;
using HarmonyLib;
using TMPro;
using UNBEATAP;

[HarmonyPatch(typeof(CharacterDropdown))]
public class CharacterDropdownPatch
{
    private static string storedPrimaryCharacter;
    private static string storedSecondaryCharacter;


    [HarmonyPatch("OnEnable")]
    [HarmonyPrefix]
    static bool OnEnablePrefix()
    {
        if(!Plugin.Client.Connected)
        {
            return true;
        }

        // This method is really long, so let's just temporarily change the character names in FileStorage
        storedPrimaryCharacter = FileStorage.beatmapOptions.primaryCharacter;
        storedSecondaryCharacter = FileStorage.beatmapOptions.secondaryCharacter;

        FileStorage.beatmapOptions.primaryCharacter = Plugin.Client.primaryCharacter;
        FileStorage.beatmapOptions.secondaryCharacter = Plugin.Client.secondaryCharacter;
        return true;
    }


    [HarmonyPatch("OnEnable")]
    [HarmonyPostfix]
    static void OnEnablePostfix()
    {
        if(!Plugin.Client.Connected)
        {
            return;
        }

        // Now that OnEnable has been bamboozled, clean up the character states
        FileStorage.beatmapOptions.primaryCharacter = storedPrimaryCharacter;
        FileStorage.beatmapOptions.secondaryCharacter = storedSecondaryCharacter;
    }


    [HarmonyPatch("OnDropdownChanged")]
    [HarmonyPrefix]
    static bool OnDropdownChangedPrefix(CharacterDropdown __instance, int option)
    {
        if(!Plugin.Client.Connected)
        {
            return true;
        }

        Traverse traverse = new Traverse(__instance);

        // this.UpdatePreview(option);
        traverse.Method("UpdatePreview", option).GetValue();
        
        TMP_Dropdown dropdown = traverse.Field("dropdown").GetValue<TMP_Dropdown>();
        string charName = dropdown.options[option].text;
        if(traverse.Field("isAssist").GetValue<bool>())
        {
            // Changed lines; set Client character instead of FileStorage
            Plugin.Client.SetSecondaryCharacter(charName);
        }
        else
        {
            Plugin.Client.SetPrimaryCharacter(charName);
        }
        // Removed line; don't call FileStorage.SaveOptions()

        return false;
    }
}