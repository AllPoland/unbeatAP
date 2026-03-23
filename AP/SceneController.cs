using UNBEATAP.Helpers;

namespace UNBEATAP.AP;

public class SceneController
{
    public const string ScenePrefix = "Scene: ";
    public static void AddScene(string itemName, bool sentBySelf)
    {
        string sceneName = itemName.Replace(ScenePrefix, "");

        if(!SceneList.TryAddScene(sceneName))
        {
            // The scene unlock was invalid
            return;
        }

        Plugin.Logger.LogInfo($"Successfully collected Scene: {ScenePrefix}");
        // Don't queue notification if not finished connecting
        if(!Plugin.Client.Connected) return;

        NotificationPopupMode popupMode = NotificationPopupMode.Received;
        if(sentBySelf)
        {
            // If we sent ourself an item, we also want to show the notification if received items are disabled
            popupMode |= NotificationPopupMode.Sent;
        }
        NotificationHelper.QueueNotification($"Scene: {sceneName}", popupMode);
    }
}