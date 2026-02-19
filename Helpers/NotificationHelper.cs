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
        while(unlockqueue.Any())
        {
            // Make queue into batches of 10 items. If there's too many on one notification, it will go offscreen.
            int takecount = unlockqueue.Count >= 10 ? 10 : unlockqueue.Count;
            IEnumerable<MainProgressionContainer.Unlock> batch = unlockqueue.Take(takecount);
            unlockqueue = unlockqueue.Skip(takecount).ToList();
            Plugin.Logger.LogInfo($"New Unlock Queue: {unlockqueue.Count}");
            ArcadeNotification.Show<ArcadeRewardsNotification>("Rewards").Fill(batch.ToList());
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