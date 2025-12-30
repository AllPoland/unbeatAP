using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace UNBEATAP.Helpers;

public static class DifficultyList
{
    public const string NameSeparator = "|";

    private static Dictionary<string, string[]> diffDict;
    private static Dictionary<string, string> songDict;

    private static List<string> currentDifficulties = new List<string>();


    public static List<string> GetDifficulties()
    {
        return currentDifficulties;
    }


    private static void AddDifficulty(string song, string difficulty)
    {
        currentDifficulties.Add($"{song}{NameSeparator}{difficulty}");
    }


    private static string GetInternalName(string songName)
    {
        if(songDict == null)
        {
            return null;
        }

        return songDict[songName].ToLower();
    }


    public static bool TryAddSongItem(string songName, int diffIndex)
    {
        string internalName = GetInternalName(songName);
        
        if(!diffDict.TryGetValue(internalName, out string[] difficulties))
        {
            Plugin.Logger.LogWarning($"Failed to get difficulties for song: {internalName}");
            return false;
        }

        if(difficulties.Length <= diffIndex)
        {
            int extraDiffs = diffIndex - difficulties.Length + 1;
            Plugin.Logger.LogWarning($"Received {extraDiffs} extra difficult(y)(ies) for song: {internalName}");
            return false;
        }

        string newDifficulty = difficulties[diffIndex];
        AddDifficulty(internalName, newDifficulty);
        return true;
    }


    private static async Task LoadDifficulties()
    {
        const string diffFile = "difficulties.json";

        string diffPath = Path.Combine(Plugin.AssetsFolder, diffFile);
        string json = await File.ReadAllTextAsync(diffPath);
        diffDict = JsonConvert.DeserializeObject<Dictionary<string, string[]>>(json);
    }


    private static async Task LoadSongs()
    {
        const string songFile = "songs.json";

        string songPath = Path.Combine(Plugin.AssetsFolder, songFile);
        string json = await File.ReadAllTextAsync(songPath);
        songDict = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
    }
    

    public static async Task Init()
    {
        try
        {
            await LoadDifficulties();
            await LoadSongs();
        }
        catch(Exception e)
        {
            Plugin.Logger.LogError($"Failed to load assets with error: {e.Message}, {e.StackTrace}");
        }
    }
}