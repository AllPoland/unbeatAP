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
    static bool LoadArcadeLevelPrefix(string beatmapName, string beatmapDifficulty, int spawn, bool transition, ref string customScene)
    {
        if(!string.IsNullOrEmpty(customScene) || !Plugin.Client.Connected)
        {
            return true;
        }

        // If scene is set to default but the default scene is not unlocked, default to the first unlocked scene in the list.
        ArcadeProgression arcadeProgression = new ArcadeProgression($"{beatmapName}/{beatmapDifficulty}", RhythmGameType.ArcadeMode);
        string ext = SceneList.GetExternalName(arcadeProgression.stageScene);
        string finalscene = SceneList.GetInternalName(SceneList.GetScenes().First());
        if(SceneList.GetScenes().Contains(ext) || finalscene == null) // If the scene ends up null, don't crash the game please
            return true;
        customScene = finalscene;
        return true;
    }
}