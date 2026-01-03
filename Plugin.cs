using System;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using UNBEATAP.Patches;
using UNBEATAP.AP;
using BepInEx.Configuration;
using UNBEATAP.Helpers;

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

    private static Client _client = new Client();
    public static Client Client
    {
        get => _client;
        set
        {
            if(value == null)
            {
                // Do not allow the client to be set to null, lest the trout population all perish
                _client = new Client();
            }
            else _client = value;
        }
    }

    private static ConfigEntry<string> configIp;
    private static ConfigEntry<int> configPort;
    private static ConfigEntry<string> configSlot;
    private static ConfigEntry<string> configPassword;

    private static ConfigEntry<bool> configDeathLink;

    private static ConfigEntry<int> backupCount;


    public static void DoBackup()
    {
        Logger.LogInfo("Creating save backups.");
        SaveBackup.BackupChallengeBoard(backupCount.Value);
    }


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

        backupCount = Config.Bind(
            "Other",
            "Save Backup Count",
            5,
            "When connecting to Archipelago, some of your save files are backed up in case of an error. This adjusts how many backups to save before the oldest is deleted."
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
            DifficultyList.Init();

            Logger.LogInfo("Setting up client.");
            Client = new Client(configIp.Value, configPort.Value, configSlot.Value, configPassword.Value, configDeathLink.Value);

            Logger.LogInfo("Applying patches.");
            try
            {
                // Override general progression
                Harmony.CreateAndPatchAll(typeof(BlockAuthentication));
                Harmony.CreateAndPatchAll(typeof(MainProgressionContainerPatch));
                Harmony.CreateAndPatchAll(typeof(UnlockAll));
                Harmony.CreateAndPatchAll(typeof(ArcadeMenuPaletteView));

                // Override challenges
                Harmony.CreateAndPatchAll(typeof(ProfileInfoPatch));
                Harmony.CreateAndPatchAll(typeof(BaseChallengeDescriptorPatch));

                // Override high scores
                Harmony.CreateAndPatchAll(typeof(FileStoragePatch));

                // Rando song handling
                Harmony.CreateAndPatchAll(typeof(ArcadeDifficultyView));
                Harmony.CreateAndPatchAll(typeof(BeatmapIndexPatch));

                // Rando character handling
                Harmony.CreateAndPatchAll(typeof(ArcadeCharacterView));
                Harmony.CreateAndPatchAll(typeof(ArcadeCharacterTogglePatch));
                Harmony.CreateAndPatchAll(typeof(RhythmCharacterSelectorPatch));

                // Rating handling
                Harmony.CreateAndPatchAll(typeof(PlayerStatsHelperPatch));
                Harmony.CreateAndPatchAll(typeof(SongRatingDisplayPatch));
                Harmony.CreateAndPatchAll(typeof(ArcadeSongScorePatch));
                Harmony.CreateAndPatchAll(typeof(SongButtonPatch));
            }
            catch(Exception e)
            {
                Logger.LogFatal($"Patching failed with error: {e.Message}, {e.StackTrace}");
                return;
            }

            Logger.LogInfo($"Plugin {PluginReleaseInfo.PLUGIN_GUID} is loaded!");

            Logger.LogInfo($"Attempting to connect to Archipelago server.");
            await Client.ConnectAndGetData();
        }
        catch(Exception e)
        {
            Logger.LogFatal($"{e.Message}, {e.StackTrace}");
        }
    }
}