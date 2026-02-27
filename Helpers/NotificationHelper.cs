using System.Collections.Generic;
using System.Linq;
using Arcade.Progression;
using Arcade.UI.SongSelect;

namespace UNBEATAP.Helpers;

public class NotificationHelper
{
    private static List<MainProgressionContainer.Unlock> unlockqueue = new List<MainProgressionContainer.Unlock>();
    public static void QueueNotification(string unlock)
    {
        Plugin.Logger.LogInfo($"Queued: {unlock}");
        unlockqueue.Add(GetUnlock(unlock));
    }
    
    public static void ShowNotification()
    {
        if(unlockqueue.Any())
        {
            // Show the notification if any are queued 
            ArcadeNotification.Show<ArcadeRewardsNotification>("Rewards").Fill(unlockqueue);
            unlockqueue.Clear();
            if(ArcadeSongDatabase.Instance)
            {
                // Refresh song database after showing notification
                // Doing this in DifficultyController made the game heavily lag on release
                ArcadeSongDatabase.Instance.LoadDatabase();
                ArcadeSongDatabase.Instance.RefreshSongList();
            }
        }
    }

    private static MainProgressionContainer.Unlock GetUnlock(string name)
    {
        MainProgressionContainer.Unlock unlock = new()
        {
            UnlockName = name,
            UnlockManager = "Archipelago"
        };
        return unlock;
    }
}