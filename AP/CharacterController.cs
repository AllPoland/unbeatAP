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
            client.SetPrimaryCharacter("Beat");
            client.SetSecondaryCharacter("Quaver");
            return;
        }

        string primary = Plugin.Client.primaryCharacter;
        string secondary = Plugin.Client.secondaryCharacter;
        if(!characters.Contains(primary))
        {
            client.SetPrimaryCharacter(characters[0]);
        }
        if(!characters.Contains(secondary))
        {
            client.SetSecondaryCharacter(characters[0]);
        }
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