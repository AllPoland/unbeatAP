using System.Collections.Generic;
using UNBEATAP.Helpers;
using UNBEATAP.Objects;

namespace UNBEATAP.AP;

public static class DifficultyController
{
    public const string SongNamePrefix = "Progressive Song: ";

    private static Dictionary<string, int> SongItemCounts = new Dictionary<string, int>();


    public static void AddProgressiveSong(string itemName, bool sentBySelf)
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

        ArchipelagoManager.Instance.SetSongListDirty();

        NotificationPopupMode popupMode = NotificationPopupMode.Received;
        if(sentBySelf)
        {
            // If we sent ourself an item, we also want to show the notification if received items are disabled
            popupMode |= NotificationPopupMode.Sent;
        }
        NotificationHelper.QueueNotification($"Progressive Song #{unlockedDiffIndex + 1}: {songName}", popupMode);
    }


    public static void Clear()
    {
        SongItemCounts.Clear();
    }
}