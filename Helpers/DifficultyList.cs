using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;

namespace UNBEATAP.Helpers;

public class DifficultyList
{
    private List<String> diffs = new List<String>();
    private static DifficultyList _instance = new DifficultyList();
    public static DifficultyList GetInstance()
    {
        return _instance;
    }
    private List<String> difficulties = new List<String>();
    public void AddDifficulty(String song, String difficulty)
    {
        difficulties.Add($"{song}/{difficulty}");
    }

    public List<String> GetDifficulties()
    {
        return difficulties;
    }
    
    public List<String> ValidDifficulties(string songName)
    {
        diffs.Clear();
        // This expects the mod to be in UNBEATABLE/BepInEx/plugins/unbeatAP, and the Assets folder with difficulties.json to be next to it.
        var name = File.ReadAllText("BepInEx/plugins/unbeatAP/Assets/difficulties.json");
        var data = JObject.Parse(name);
        foreach(String diff in data[songName])
        {
            diffs.Add(diff);
        }
        return diffs;
    }
}