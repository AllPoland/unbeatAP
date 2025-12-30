using System.Collections.Generic;
using System.Linq;

namespace UNBEATAP.Helpers;

public static class CharacterList
{
    public static readonly string[] CharNames = [
        "beat",
        "beat (hoodie)",
        "beat (guitar)",
        "beat (up)",
        "beat (nothing)",
        "clef",
        "quaver",
        "quaver (acoustic)",
        "quaver (cqc)",
        "treble",
        "rest"
    ];

    private static List<string> characters = new List<string>();


    public static bool TryAddCharacter(string character)
    {
        if(characters.Contains(character))
        {
            Plugin.Logger.LogWarning($"Character '{character}' has already been added!");
            return false;
        }

        if(!CharNames.Contains(character.ToLower()))
        {
            Plugin.Logger.LogWarning($"Character '{character}' does not exist!");
            return false;
        }

        characters.Add(character);
        return true;
    }


    public static List<string> GetCharacters()
    {
        return characters;
    }
}