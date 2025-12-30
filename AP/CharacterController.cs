using System.Collections.Generic;
using UNBEATAP.Helpers;

namespace UNBEATAP.AP;

public static class CharacterController
{
    public const string CharPrefix = "Character: ";


    public static void ForceEquipUnlockedCharacter()
    {
        Client client = Plugin.Client;
        List<string> characters = CharacterList.GetCharacters();

        if(characters.Count <= 0)
        {
            // No characters unlocked for some reason, just use defaults
            client.SetPrimaryCharacter("beat");
            client.SetSecondaryCharacter("quaver");
            return;
        }

        string primary = FileStorage.beatmapOptions.primaryCharacter;
        string secondary = FileStorage.beatmapOptions.secondaryCharacter;
        if(!characters.Contains(primary))
        {
            primary = characters[0];
        }
        if(!characters.Contains(secondary))
        {
            secondary = characters[0];
        }

        client.SetPrimaryCharacter(primary);
        client.SetSecondaryCharacter(secondary);
    }


    public static void AddCharacter(string itemName)
    {
        string charName = itemName.Replace(CharPrefix, "");

        if(!CharacterList.TryAddCharacter(charName))
        {
            // The character unlock was invalid
            return;
        }

        Plugin.Logger.LogInfo($"Collected Character: {charName}");
        ForceEquipUnlockedCharacter();
    }
}