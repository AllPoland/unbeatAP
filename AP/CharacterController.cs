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
            if(characters.Count >= 2)
            {
                // Choosing a different character as the secondary if possible is kinda cool
                client.SetSecondaryCharacter(characters[1]);
            }
            else client.SetSecondaryCharacter(characters[0]);
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

        Plugin.Logger.LogInfo($"Successfully collected Character: {charName}");
        ForceEquipUnlockedCharacter();
        // Don't queue notification if not finished connecting
        if(!Plugin.Client.Connected) return;
        NotificationHelper.QueueNotification($"Character: {charName}");
    }
}