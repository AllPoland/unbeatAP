using System.Collections.Generic;
using System.Linq;
using Arcade.Progression;

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
        Plugin.Logger.LogInfo($"Current Unlock Queue: {unlockqueue.Count}");
        if(unlockqueue.Any())
        {
            // Show the notification if any are queued 
            ArcadeNotification.Show<ArcadeRewardsNotification>("Rewards").Fill(unlockqueue);
            unlockqueue.Clear();
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