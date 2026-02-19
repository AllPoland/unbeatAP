using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace UNBEATAP.AP;

public class SlotData
{
    public bool UseBreakout;
    public int MaxDifficulty;
    public int MinDifficulty;

    public float SkillRating;
    public bool AllowPfc;
    public float AccCurveBias;
    public float AccCurveLowBias;
    public float AccCurveCutoff;

    public int ItemCount;
    public float TargetRating;

    public string WorldVersion;
    public string[] CompatibleVersions;


    private void TryGetValue(Dictionary<string, object> data, string key, int def, out int output)
    {
        if(!data.TryGetValue(key, out object dataObject))
        {
            Plugin.Logger.LogError($"Failed to find key {key} in slot data!");
            output = def;
            return;
        }

        string repr = dataObject.ToString();
        if(!int.TryParse(repr, out output))
        {
            Plugin.Logger.LogError($"Entry {key} : {repr} in slot data is unexpected type! Expected int.");
            return;
        }
    }


    private void TryGetValue(Dictionary<string, object> data, string key, float def, out float output)
    {
        if(!data.TryGetValue(key, out object dataObject))
        {
            Plugin.Logger.LogError($"Failed to find key {key} in slot data!");
            output = def;
            return;
        }

        string repr = dataObject.ToString();
        if(!float.TryParse(repr, out output))
        {
            Plugin.Logger.LogError($"Entry {key} : {repr} in slot data is unexpected type! Expected float.");
            return;
        }
    }


    private void TryGetValue(Dictionary<string, object> data, string key, bool def, out bool output)
    {
        if(!data.TryGetValue(key, out object dataObject))
        {
            Plugin.Logger.LogError($"Failed to find key {key} in slot data!");
            output = def;
            return;
        }

        string repr = dataObject.ToString();
        if(!int.TryParse(repr, out int intValue))
        {
            Plugin.Logger.LogError($"Entry {key} : {repr} in slot data is unexpected type! Expected int.");
            output = def;
            return;
        }

        output = intValue != 0;
    }


    public SlotData(Dictionary<string, object> data)
    {
        TryGetValue(data, "use_breakout", false, out UseBreakout);
        TryGetValue(data, "max_difficulty", 5, out MaxDifficulty);
        TryGetValue(data, "min_difficulty", 0, out MinDifficulty);

        TryGetValue(data, "skill_rating", 500, out SkillRating);
        TryGetValue(data, "allow_pfc", true, out AllowPfc);
        TryGetValue(data, "acc_curve_bias", 600, out AccCurveBias);
        TryGetValue(data, "acc_curve_low_bias", 200, out AccCurveLowBias);
        TryGetValue(data, "acc_curve_cutoff", 85, out AccCurveCutoff);

        TryGetValue(data, "item_count", 1, out ItemCount);
        TryGetValue(data, "target_rating", Mathf.Infinity, out TargetRating);

        if(data.TryGetValue("version", out object versionData))
        {
            WorldVersion = versionData.ToString();
        }
        if(data.TryGetValue("compatible_versions", out object compatibleVersionsData))
        {
            try
            {
                string compatibleVersionsJson = compatibleVersionsData.ToString();
                CompatibleVersions = JsonConvert.DeserializeObject<string[]>(compatibleVersionsJson);
            }
            catch(Exception e)
            {
                Plugin.Logger.LogError($"Failed to parse compatible versions with error: {e.Message}\n    {e.StackTrace}");
            }
        }

        AccCurveBias /= 100f;
        AccCurveLowBias /= 100f;
        AccCurveCutoff /= 100f;
    }
}