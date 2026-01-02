using System;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using UNBEATAP.Patches;
using UNBEATAP.AP;
using BepInEx.Configuration;
using UNBEATAP.Helpers;
using Rhythm;

namespace UNBEATAP;

[BepInPlugin(PluginReleaseInfo.PLUGIN_GUID, PluginReleaseInfo.PLUGIN_NAME, PluginReleaseInfo.PLUGIN_VERSION)]
[BepInProcess("UNBEATABLE.exe")]
public class Plugin : BaseUnityPlugin
{
    public const string GameName = "UNBEATABLE Arcade";
    public const string PluginFolder = "BepInEx/plugins/unbeatAP";
    public static readonly string AssetsFolder = PluginFolder + "/Assets";
    public static readonly string SaveFolder = PluginFolder + "/Saves";

    internal static new ManualLogSource Logger;

    public static Client Client = new Client();

    private ConfigEntry<string> configIp;
    private ConfigEntry<int> configPort;
    private ConfigEntry<string> configSlot;
    private ConfigEntry<string> configPassword;

    private ConfigEntry<bool> configDeathLink;


    private void LoadConfig()
    {
        configIp = Config.Bind(
            "Connection",
            "IP",
            "archipelago.gg",
            "The archipelago server IP to connect to. If you're running the game on the official website, this is probably 'archipelago.gg'."
        );
        configPort = Config.Bind(
            "Connection",
            "Port",
            58008,
            "The port to connect to. This is all the numbers after the ':' in the IP."
        );
        configSlot = Config.Bind(
            "Connection",
            "Slot",
            "Player1",
            "The player slot name to connect with. Enter what you set 'name' to in your YAML."
        );
        configPassword = Config.Bind(
            "Connection",
            "Password",
            "",
            "The password to use when connecting. If the server has no password, leave this empty."
        );

        configDeathLink = Config.Bind(
            "Gameplay",
            "DeathLink",
            false,
            "If Death Link is enabled, you'll fail out of a song if someone else dies, and everyone else will die when you fail a song."
        );
    }


    private async void Awake()
    {
        try
        {
            // Plugin startup logic
            Logger = base.Logger;

            Logger.LogInfo("Loading config.");
            LoadConfig();

            Logger.LogInfo("Loading assets.");
            await DifficultyList.Init();

            Logger.LogInfo("Setting up client.");
            Client = new Client(configIp.Value, configPort.Value, configSlot.Value, configPassword.Value, configDeathLink.Value);

            Logger.LogInfo("Applying patches.");
            try
            {
                // Disable general progression
                Harmony.CreateAndPatchAll(typeof(BlockAuthentication));
                Harmony.CreateAndPatchAll(typeof(MainProgressionContainerPatch));
                Harmony.CreateAndPatchAll(typeof(UnlockAll));

                // Disable challenges
                Harmony.CreateAndPatchAll(typeof(ProfileInfoPatch));
                Harmony.CreateAndPatchAll(typeof(BaseChallengeDescriptorPatch));

                // Rando song handling
                Harmony.CreateAndPatchAll(typeof(ArcadeDifficultyView));
                Harmony.CreateAndPatchAll(typeof(BeatmapIndexPatch));

                // Rando character handling
                Harmony.CreateAndPatchAll(typeof(ArcadeCharacterView));
                Harmony.CreateAndPatchAll(typeof(ArcadeCharacterTogglePatch));
                Harmony.CreateAndPatchAll(typeof(RhythmCharacterSelectorPatch));
            }
            catch(Exception e)
            {
                Logger.LogFatal($"Patching failed with error: {e.Message}, {e.StackTrace}");
                return;
            }

            Logger.LogInfo($"Attempting to connect to Archipelago server.");
            await Client.ConnectAndGetData();

            Logger.LogInfo($"Plugin {PluginReleaseInfo.PLUGIN_GUID} is loaded!");
        }
        catch(Exception e)
        {
            Logger.LogFatal($"{e.Message}, {e.StackTrace}");
        }
    }
}