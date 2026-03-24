using UNBEATAP.Helpers;

namespace UNBEATAP.AP;

public static class StageController
{
    public const string StagePrefix = "Stage: ";
    public static void AddStage(string itemName, bool sentBySelf)
    {
        string stageName = itemName.Replace(StagePrefix, "");

        if(!StageList.TryAddStage(stageName))
        {
            // The stage unlock was invalid
            return;
        }

        Plugin.Logger.LogInfo($"Successfully collected Stage: {stageName}");
        // Don't queue notification if not finished connecting
        if(!Plugin.Client.Connected) return;

        NotificationPopupMode popupMode = NotificationPopupMode.Received;
        if(sentBySelf)
        {
            // If we sent ourself an item, we also want to show the notification if received items are disabled
            popupMode |= NotificationPopupMode.Sent;
        }
        NotificationHelper.QueueNotification($"Stage: {stageName}", popupMode);
    }
}