using System.Linq;
using HarmonyLib;
using Rhythm;
using UNBEATAP.Helpers;
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

        if(ArchipelagoManager.Instance.UIManager)
        {
            ArchipelagoManager.Instance.UIManager.HandleSceneLoadStart(sceneIndex == JeffBezosController.arcadeMenuScene);
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
    
    [HarmonyPatch("LoadArcadeLevel")]
    [HarmonyPrefix]
    static void LoadArcadeLevelPrefix(string beatmapName, string beatmapDifficulty, ref string customScene)
    {
        if(!string.IsNullOrEmpty(customScene) || !Plugin.Client.Connected)
        {
            return;
        }

        // If stage is set to default but the default stage is not unlocked, default to the first unlocked stage in the list.
        ArcadeProgression arcadeProgression = new ArcadeProgression($"{beatmapName}/{beatmapDifficulty}", RhythmGameType.ArcadeMode);
        string ext = StageList.GetExternalName(arcadeProgression.stageScene);
        string finalstage = StageList.GetInternalName(StageList.GetStages().First());
        if(StageList.GetStages().Contains(ext) || finalstage == null) // If the stage ends up null, don't crash the game please
            return;
        customScene = finalstage;
    }
}