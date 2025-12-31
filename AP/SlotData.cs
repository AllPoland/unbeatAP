using System.Collections.Generic;

namespace UNBEATAP.AP;

public class SlotData
{
    public float SkillRating;
    public bool UseBreakout;
    public int MaxDifficulty;
    public int MinDifficulty;
    public int ItemCount;
    public float TargetRating;


    private void TryGetValue(Dictionary<string, object> data, string key, ref int output)
    {
        if(!data.TryGetValue(key, out object dataObject))
        {
            Plugin.Logger.LogError($"Failed to find key {key} in slot data!");
            return;
        }

        string repr = dataObject.ToString();
        if(!int.TryParse(repr, out output))
        {
            Plugin.Logger.LogError($"Entry {key} : {repr} in slot data is unexpected type! Expected int.");
            return;
        }
    }


    private void TryGetValue(Dictionary<string, object> data, string key, ref float output)
    {
        if(!data.TryGetValue(key, out object dataObject))
        {
            Plugin.Logger.LogError($"Failed to find key {key} in slot data!");
            return;
        }

        string repr = dataObject.ToString();
        if(!float.TryParse(repr, out output))
        {
            Plugin.Logger.LogError($"Entry {key} : {repr} in slot data is unexpected type! Expected float.");
            return;
        }
    }


    private void TryGetValue(Dictionary<string, object> data, string key, ref bool output)
    {
        if(!data.TryGetValue(key, out object dataObject))
        {
            Plugin.Logger.LogError($"Failed to find key {key} in slot data!");
            return;
        }

        string repr = dataObject.ToString();
        if(!int.TryParse(repr, out int intValue))
        {
            Plugin.Logger.LogError($"Entry {key} : {repr} in slot data is unexpected type! Expected int.");
            return;
        }

        output = intValue != 0;
    }


    public SlotData(Dictionary<string, object> data)
    {
        TryGetValue(data, "skill_rating", ref SkillRating);
        TryGetValue(data, "use_breakout", ref UseBreakout);
        TryGetValue(data, "max_difficulty", ref MaxDifficulty);
        TryGetValue(data, "min_difficulty", ref MinDifficulty);
        TryGetValue(data, "item_count", ref ItemCount);
        TryGetValue(data, "target_rating", ref TargetRating);
    }
}