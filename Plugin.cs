using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

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

        Logger.LogInfo($"Plugin {PluginReleaseInfo.PLUGIN_GUID} is loaded!");
    }
}