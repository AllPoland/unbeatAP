using HarmonyLib;
using UNBEATAP.Objects;

namespace UNBEATAP.Patches;

[HarmonyPatch(typeof(LevelManager))]
public class LevelManagerPatch
{
    [HarmonyPatch("LoadLevel", [typeof(string), typeof(int), typeof(bool), typeof(bool)])]
    [HarmonyPrefix]
    static bool LoadLevelPrefix(string sceneIndex)
    {
        if(!Plugin.Client.Connected)
        {
            if(ArchipelagoManager.Instance.Connecting)
            {
                // Don't allow the player to change scenes while connecting
                return false;
            }

            return true;
        }

        if(!ArchipelagoManager.Instance.IsArcadeMenu)
        {
            return true;
        }

        if(sceneIndex == JeffBezosController.mainMenuScene)
        {
            // The player clicked the "story mode" button, which we replace with a disconnect button
            Plugin.Client.DisconnectAndClose();
            return false;
        }
        return true;
    }
}