using UNBEATAP.Helpers;

namespace UNBEATAP.AP;

public static class CharacterController
{
    public const string CharPrefix = "Character: ";


    public static void AddCharacter(string itemName)
    {
        string charName = itemName.Replace(CharPrefix, "");

        CharacterList.AddCharacter(charName);
        Plugin.Logger.LogInfo($"Collected Character: {charName}");
    }
}