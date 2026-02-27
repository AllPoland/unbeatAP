using System;
using System.Collections.Generic;
using System.Linq;
using Arcade.Progression;

namespace UNBEATAP.Helpers;

public class NotificationHelper
{
    private static List<MainProgressionContainer.Unlock> unlockqueue = new List<MainProgressionContainer.Unlock>();
    public static void QueueNotification(string unlock, NotificationPopupMode popupMode)
    {
        if(!Plugin.Client.Connected || (Plugin.Client.popupBehavior & popupMode) == 0)
        {
            Plugin.Logger.LogInfo($"Unlock masked by options: {unlock}");
            return;
        }

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


[Flags]
public enum NotificationPopupMode
{
    None = 0,
    Sent = 1,
    Received = 2
}