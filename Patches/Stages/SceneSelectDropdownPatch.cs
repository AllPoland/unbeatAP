using System.Collections.Generic;
using Arcade.UI.YourProfile;
using Arcade.Unlockables;
using HarmonyLib;

namespace UNBEATAP.Patches;

[HarmonyPatch(typeof(SceneSelectDropdown))]
public class SceneSelectDropdownPatch
{
    private static string storedRhythmScene;


    [HarmonyPatch("OnItemClicked")]
    [HarmonyPrefix]
    static bool OnItemClickedPrefix(SceneSelectDropdown __instance, int index)
    {
        if(!Plugin.Client.Connected)
        {
            return true;
        }

        // Redirect stage selection to the client
        Traverse traverse = new Traverse(__instance);
        traverse.Field("_value").SetValue(index);
        traverse.Method("UpdatePreview", index).GetValue();

        List<RhythmSceneIndex.RhythmScene> scenes = traverse.Field("_options").GetValue<List<RhythmSceneIndex.RhythmScene>>();
        RhythmSceneIndex.RhythmScene scene = scenes[index];
        Plugin.Client.SetStage(scene.name);
        __instance.Hide();

        return false;
    }


    [HarmonyPatch("OnEnable")]
    [HarmonyPrefix]
    static bool OnEnablePrefix()
    {
        if(!Plugin.Client.Connected)
        {
            return true;
        }

        // Wrap the method to spoof FileStorage.beatmapOptions.rhythmScene
        storedRhythmScene = FileStorage.beatmapOptions.rhythmScene;
        FileStorage.beatmapOptions.rhythmScene = Plugin.Client.stage;
        return true;
    }


    [HarmonyPatch("OnEnable")]
    [HarmonyPostfix]
    static void OnEnablePostfix()
    {
        if(!Plugin.Client.Connected)
        {
            return;
        }

        // Restore the original player options
        FileStorage.beatmapOptions.rhythmScene = storedRhythmScene;
        return;
    }
}