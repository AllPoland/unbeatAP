using System;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Arcade.Progression;
using UNBEATAP.Helpers;
using UNBEATAP.Patches;
using UnityEngine.SceneManagement;

namespace UNBEATAP;

[BepInPlugin(PluginReleaseInfo.PLUGIN_GUID, PluginReleaseInfo.PLUGIN_NAME, PluginReleaseInfo.PLUGIN_VERSION)]
[BepInProcess("UNBEATABLE.exe")]
public class Plugin : BaseUnityPlugin
{
    internal static new ManualLogSource Logger;

    public static float MaxScheduleOffset => RhythmConsts.LeniencyMilliseconds + 20f;
    public static float MinScheduleOffset = 20f;
    
    private void Awake()
    {
        // Plugin startup logic
        Logger = base.Logger;
        // ArcadeSongView seems pretty useless if ArcadeDifficultyView is enabled, maybe it can be unpatched on archipelago connection?
        //Harmony.CreateAndPatchAll(typeof(ArcadeSongView));
        //Harmony.CreateAndPatchAll(typeof(ArcadeCharacterView));
        //Harmony.CreateAndPatchAll(typeof(ArcadeDifficultyView));
        Harmony.CreateAndPatchAll(typeof(BlockAuthentication));
        Harmony.CreateAndPatchAll(typeof(UnlockAll));
        Logger.LogInfo($"Plugin {PluginReleaseInfo.PLUGIN_GUID} is loaded!");
    }

    private void Start()
    {
        Logger.LogInfo($"UnlockAll state: {MainProgressionContainer.UnlockAll}");
        SceneManager.activeSceneChanged += GetSceneChange;
    }

    private void GetSceneChange(Scene arg0, Scene arg1)
    {
        //Logger.LogInfo($"Scene {arg1.name} is loaded!");
        if(arg1.name == "ScoreScreenArcadeMode")
        {
            // Can use things from Helpers.RhythmManager when implemented
        }
    }
}