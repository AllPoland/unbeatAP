using Arcade.Progression;
using Arcade.Unlockables;
using HarmonyLib;
using UNBEATAP.Helpers;

namespace UNBEATAP.Patches;

public class ArcadeSceneView
{
    [HarmonyPatch(typeof(RhythmSceneUnlocksManager), "GetRhythmSceneState")]
    [HarmonyPrefix]
    static bool GetRhythmSceneStatePatch(ref bool __result, RhythmSceneIndex.RhythmScene rhythmScene)
    {
        if(!Plugin.Client.Connected)
        {
            return true;
        }

        if(SceneList.GetScenes().Contains(rhythmScene.name))
        {
            __result = true;
            return true;
        }

        __result = false;
        return false;
    }
}