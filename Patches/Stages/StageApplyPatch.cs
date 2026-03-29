using Arcade.Unlockables;
using HarmonyLib;

namespace UNBEATAP.Patches;

[HarmonyPatch(typeof(StorableBeatmapOptions))]
public class StageApplyPatch
{
    [HarmonyPatch("CurrentRhythmScene", MethodType.Getter)]
    [HarmonyPrefix]
    static bool CurrentRhythmScenePrefix(ref string __result)
    {
        if(!Plugin.Client.Connected)
        {
            return true;
        }

        // Defer to the client instead of FileStorage
        string stage = Plugin.Client.stage;
        if(RhythmSceneIndex.CachedDefaultIndex.TryGetRhythmScene(stage, out RhythmSceneIndex.RhythmScene scene))
        {
            __result = scene.scene;
        }
        else __result = string.Empty;

        return false;
    }
}