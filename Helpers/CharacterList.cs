using System;
using System.Collections.Generic;

namespace UNBEATAP.Helpers;

public class CharacterList
{
    private static CharacterList _instance = new CharacterList();
    public static CharacterList GetInstance()
    {
        return _instance;
    }
    private List<String> characters = new List<String>();
    public void AddCharacter(String character)
    {
        characters.Add(character.ToLower());
    }

    public List<String> GetCharacters()
    {
        return characters;
    }
}