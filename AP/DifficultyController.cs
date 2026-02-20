using System.Collections.Generic;
using Arcade.UI.SongSelect;
using UNBEATAP.Helpers;

namespace UNBEATAP.AP;

public static class DifficultyController
{
    public const string SongNamePrefix = "Progressive Song: ";

    private static Dictionary<string, int> SongItemCounts = new Dictionary<string, int>();


    public static void AddProgressiveSong(string itemName)
    {
        string songName = itemName.Replace(SongNamePrefix, "");

        int unlockedDiffIndex;
        if(!SongItemCounts.TryGetValue(songName, out unlockedDiffIndex))
        {
            // This song hasn't been collected yet, so start with the first difficulty
            unlockedDiffIndex = 0;
        }

        // Since count is 1-indexed and diff arrays are zero indexed, unlockedDiffIndex is actually accurate right now
        SlotData slotData = Plugin.Client.SlotData;
        if(!DifficultyList.TryAddSongItem(songName, unlockedDiffIndex, slotData.MinDifficulty, slotData.MaxDifficulty))
        {
            // The difficulty unlock was invalid
            return;
        }

        SongItemCounts[songName] = unlockedDiffIndex + 1;
        Plugin.Logger.LogInfo($"Successfully collected Progressive Song #{unlockedDiffIndex + 1}: {songName}");
        
        // Skip everything else if not finished connecting
        if(!Plugin.Client.Connected) return;
        NotificationHelper.QueueNotification($"Progressive Song #{unlockedDiffIndex + 1}: {songName}");
    }


    public static void Clear()
    {
        SongItemCounts.Clear();
    }
}