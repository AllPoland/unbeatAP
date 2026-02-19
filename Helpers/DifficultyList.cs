using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace UNBEATAP.Helpers;

public static class DifficultyList
{
    public const string NameSeparator = "/";

    public static readonly string[] DiffNameFromRank =
    [
        "beginner",
        "easy",
        "normal",
        "hard",
        "unbeatable",
        "star"
    ];

    public static readonly Dictionary<string, string> LowerToFullDiffName = new Dictionary<string, string>
    {
        {"beginner", "Beginner"},
        {"easy", "Easy"},
        {"normal", "Normal"},
        {"hard", "Hard"},
        {"unbeatable", "UNBEATABLE"},
        {"star", "Star"}
    };

    private static Dictionary<string, string[]> diffDict;
    private static Dictionary<string, string> songDict;

    private static List<string> currentDifficulties = new List<string>();
    private static List<string> visibleDifficulties = new List<string>();


    public static List<string> GetDifficulties()
    {
        return currentDifficulties;
    }


    public static List<string> GetVisibleDifficulties()
    {
        return visibleDifficulties;
    }


    private static void SortVisibleDifficulties()
    {
        // We need to do this because the difficulty switcher assumes they're sorted properly
        List<string> sortedDifficulties = new List<string>();

        // The dictionary has them listed in order, so we can just check each one in order and get a sorted list
        foreach(KeyValuePair<string, string> pair in LowerToFullDiffName)
        {
            if(visibleDifficulties.Contains(pair.Value))
            {
                sortedDifficulties.Add(pair.Value);
            }
        }

        visibleDifficulties = sortedDifficulties;
    }


    private static void AddDifficulty(string song, string difficulty)
    {
        currentDifficulties.Add($"{song}{NameSeparator}{difficulty}");

        string newDiffName = LowerToFullDiffName[difficulty.ToLower()];
        if(!visibleDifficulties.Contains(newDiffName))
        {
            visibleDifficulties.Add(newDiffName);
            SortVisibleDifficulties();
        }
    }


    private static string GetInternalName(string songName)
    {
        if(songDict == null)
        {
            return null;
        }

        if(!songDict.TryGetValue(songName, out string internalName))
        {
            Plugin.Logger.LogError($"Unable to get internal name for song: {songName}");
            return null;
        }

        return internalName.ToLower();
    }


    public static bool TryAddSongItem(string songName, int diffIndex, int minDiff, int maxDiff)
    {
        string internalName = GetInternalName(songName);
        if(string.IsNullOrEmpty(internalName))
        {
            return false;
        }
        
        if(!diffDict.TryGetValue(internalName, out string[] difficulties))
        {
            Plugin.Logger.LogWarning($"Failed to get difficulties for song: {internalName}");
            return false;
        }

        // Gather all of the difficulties the song has within our minimum and maximum ranges
        List<string> songDifficulties = new List<string>();
        for(int rank = minDiff; rank <= maxDiff && rank <= DiffNameFromRank.Length; rank++)
        {
            string diffName = DiffNameFromRank[rank];
            if(difficulties.Contains(diffName))
            {
                songDifficulties.Add(diffName);
            }
        }

        if(songDifficulties.Count == 0)
        {
            Plugin.Logger.LogWarning($"Received item for song with no included difficulties: {songName}");
            return false;
        }

        if(diffIndex < 0)
        {
            Plugin.Logger.LogError($"Target difficulty index is less than zero! ({internalName})");
            return false;
        }

        if(diffIndex >= songDifficulties.Count)
        {
            Plugin.Logger.LogWarning($"Received extra difficulty for song: {internalName}");
            return false;
        }

        // Get the [diffIndex]th difficulty from the ones that are included in the rando
        string targetDiff = songDifficulties[diffIndex];
        AddDifficulty(internalName, targetDiff);
        return true;
    }


    public static void Clear()
    {
        currentDifficulties.Clear();
        visibleDifficulties.Clear();
    }


    private static void LoadDifficulties()
    {
        const string diffFile = "difficulties.json";

        string diffPath = Path.Combine(Plugin.AssetsFolder, diffFile);
        string json = File.ReadAllText(diffPath);
        diffDict = JsonConvert.DeserializeObject<Dictionary<string, string[]>>(json);
    }


    private static void LoadSongs()
    {
        const string songFile = "songs.json";

        string songPath = Path.Combine(Plugin.AssetsFolder, songFile);
        string json = File.ReadAllText(songPath);
        songDict = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
    }
    

    public static void Init()
    {
        try
        {
            LoadDifficulties();
            LoadSongs();
        }
        catch(Exception e)
        {
            Plugin.Logger.LogError($"Failed to load assets with error: {e.Message}, {e.StackTrace}");
        }
    }
}