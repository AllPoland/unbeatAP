using System.Collections.Generic;
using System.Linq;
using Arcade.Unlockables;

namespace UNBEATAP.Helpers;

public static class SceneList
{
    public static readonly string[] SceneNames =
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

    private static List<string> scenes = new List<string>();
    private static List<RhythmSceneIndex.RhythmScene> allscenes = RhythmSceneIndex.defaultIndex.GetAllRhythmScenes();

    public static bool TryAddScene(string scene)
    {
        if(scenes.Contains(scene))
        {
            Plugin.Logger.LogWarning($"Scene '{scene}' has already been added!");
            return false;
        }

        if(!SceneNames.Contains(scene.ToLower()))
        {
            Plugin.Logger.LogWarning($"Scene '{scene}' does not exist!");
            return false;
        }

        scenes.Add(scene);
        return true;
    }


    public static List<string> GetScenes()
    {
        // If there are no scenes after connection, unlock every scene
        // Older apworld versions don't have scenes, and the user shouldn't be punished for that.
        if(scenes.Count == 0)
        {
            scenes = SceneNames.ToList();
        }
        return scenes;
    }


    public static void Clear()
    {
        scenes.Clear();
    }


    public static string GetExternalName(string name)
    {
        foreach(RhythmSceneIndex.RhythmScene scene in allscenes)
        {
            if(scene.scene.EndsWith(name))
            {
                return scene.name;
            }
        }
        return null;
    }


    public static string GetInternalName(string name)
    {
        foreach(RhythmSceneIndex.RhythmScene scene in allscenes)
        {
            string intname = scene.scene.Split("/").Last();
            if(name == scene.name)
            {
                return intname;
            }
        }
        return null;
    }
}