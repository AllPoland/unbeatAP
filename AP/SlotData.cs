using System.Collections.Generic;
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
    public float AccCurveCutoff;

    public int ItemCount;
    public float TargetRating;


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

        TryGetValue(data, "skill_rating", 600, out SkillRating);
        TryGetValue(data, "allow_pfc", false, out AllowPfc);
        TryGetValue(data, "acc_curve_bias", 250, out AccCurveBias);
        TryGetValue(data, "acc_curve_cutoff", 80, out AccCurveCutoff);

        TryGetValue(data, "item_count", 1, out ItemCount);
        TryGetValue(data, "target_rating", Mathf.Infinity, out TargetRating);

        SkillRating /= 100f;
        AccCurveBias /= 100f;
        AccCurveCutoff /= 100f;
    }
}