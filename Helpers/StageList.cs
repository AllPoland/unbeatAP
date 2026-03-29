using System.Collections.Generic;
using System.Linq;
using Arcade.Unlockables;

namespace UNBEATAP.Helpers;

public static class StageList
{
    public static readonly string[] StageNames =
    [
        "train_station.",
        "stadium_(past).",
        "stadium_(present).",
        "dreamscape.",
        "prison_yard.",
        "train.",
        "lighthouse_show.",
        "city_hideout.",
        "recording_studio.",
        "overpass.",
        "city_center.",
        "underground_show.",
        "alleyway_show.",
        "warehouse_show.",
        "harm_hq_lobby.",
        "zero_moment_array.",
        "zm_test_chamber.",
        "graveyard.",
        "nsr.",
        "greenscreen."
    ];

    private static List<string> stages = new List<string>();
    private static List<RhythmSceneIndex.RhythmScene> allStages = RhythmSceneIndex.defaultIndex.GetAllRhythmScenes();

    public static bool TryAddStage(string stage)
    {
        if(stages.Contains(stage))
        {
            Plugin.Logger.LogWarning($"Scene '{stage}' has already been added!");
            return false;
        }

        if(!StageNames.Contains(stage.ToLower()))
        {
            Plugin.Logger.LogWarning($"Scene '{stage}' does not exist!");
            return false;
        }

        stages.Add(stage);
        return true;
    }


    public static List<string> GetStages()
    {
        // If there are no stages after connection, unlock every stage
        // Older apworld versions don't have stages, and the user shouldn't be punished for that.
        if(stages.Count == 0)
        {
            return StageNames.ToList();
        }
        return stages;
    }


    public static void Clear()
    {
        stages.Clear();
    }

    public static void Init()
    {
        // This is empty, but if I don't run something in this class at startup the game crashes
    }

    public static string GetExternalName(string name)
    {
        foreach(RhythmSceneIndex.RhythmScene stage in allStages)
        {
            if(stage.scene.EndsWith(name))
            {
                return stage.name;
            }
        }
        return null;
    }


    public static string GetInternalName(string name)
    {
        foreach(RhythmSceneIndex.RhythmScene stage in allStages)
        {
            string intname = stage.scene.Split("/").Last();
            if(name == stage.name)
            {
                return intname;
            }
        }
        return null;
    }
}