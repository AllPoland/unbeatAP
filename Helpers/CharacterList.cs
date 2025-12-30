using System.Collections.Generic;

namespace UNBEATAP.Helpers;

public static class CharacterList
{
    private static List<string> characters = new List<string>();


    public static bool TryAddCharacter(string character)
    {
        if(characters.Contains(character))
        {
            Plugin.Logger.LogWarning($"Character '{character}' does not exist!");
            return false;
        }

        characters.Add(character.ToLower());
        return true;
    }


    public static List<string> GetCharacters()
    {
        return characters;
    }
}