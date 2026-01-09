using System;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using UNBEATAP.Patches;
using UNBEATAP.AP;
using BepInEx.Configuration;
using UNBEATAP.Helpers;
using UNBEATAP.Traps;
using UnityEngine;
using UNBEATAP.Objects;
using UnityEngine.SceneManagement;
using UBUI.Archipelago;

namespace UNBEATAP;

[BepInPlugin(PluginReleaseInfo.PLUGIN_GUID, PluginReleaseInfo.PLUGIN_NAME, PluginReleaseInfo.PLUGIN_VERSION)]
[BepInProcess("UNBEATABLE.exe")]
public class Plugin : BaseUnityPlugin
{
    public const string GameName = "UNBEATABLE Arcade";
    public const string PluginFolder = "BepInEx/plugins/unbeatAP";
    public static readonly string AssetsFolder = PluginFolder + "/Assets";
    public static readonly string SaveFolder = PluginFolder + "/Saves";

    public static readonly string AssetBundleFolder = PluginFolder + "/UNBEATABLE-UI-Resources/Assets/AssetBundles";
    public static readonly string UiResourcesBundlePath = AssetBundleFolder + "/unbeatable-ui-resources";
    public static readonly string FontResourcesBundlePath = AssetBundleFolder + "/unbeatable-ui-fonts";
    public static readonly string ApUiBundlePath = AssetBundleFolder + "/unbeatable-ui-archipelago";

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

    private static ConfigEntry<float> mutedDuration;
    private static ConfigEntry<float> rainbowDuration;
    private static ConfigEntry<float> scrollSpeedDuration;
    private static ConfigEntry<float> stealthDuration;

    private static ConfigEntry<string> configIp;
    private static ConfigEntry<int> configPort;
    private static ConfigEntry<string> configSlot;
    private static ConfigEntry<string> configPassword;

    private static ConfigEntry<bool> configDeathLink;

    private static ConfigEntry<int> backupCount;


    public static void SetupNewClient()
    {
        if(Client.Connected)
        {
            Logger.LogWarning("Tried to set up a new client while already connected!");
            return;
        }

        Logger.LogInfo("Setting up client.");
        Client = new Client(configIp.Value, configPort.Value, configSlot.Value, configPassword.Value, configDeathLink.Value);
    }


    public static void DoBackup()
    {
        Logger.LogInfo("Creating save backups.");
        SaveBackup.BackupChallengeBoard(backupCount.Value);
    }


    public static void SetConnectionInfo(string ip, int port, string slot, string password)
    {
        configIp.Value = ip;
        configPort.Value = port;
        configSlot.Value = slot;
        configPassword.Value = password;
    }


    public static APConnectionInfo GetConnectionInfo()
    {
        return new APConnectionInfo
        {
            ip = configIp.Value,
            port = configPort.Value.ToString(),
            slot = configSlot.Value,
            pass = configPassword.Value
        };
    }


    private void LoadConfig()
    {
        configDeathLink = Config.Bind(
            "Gameplay",
            "DeathLink",
            false,
            "If Death Link is enabled, you'll fail out of a song if someone else dies, and everyone else will die when you fail a song."
        );
        mutedDuration = Config.Bind(
            "Gameplay",
            "Mute Trap Duration",
            20f,
            "The amount of time a Mute Trap mutes the music. Set to 0 to disable Mute Traps client-side."
        );
        rainbowDuration = Config.Bind(
            "Gameplay",
            "Rainbow Trap Duration",
            30f,
            "The amount of time a Rainbow Trap randomizes note colors. Set to 0 to disable Rainbow Traps client-side."
        );
        scrollSpeedDuration = Config.Bind(
            "Gameplay",
            "Scroll Speed Trap Duration",
            20f,
            "The amount of time a Scroll Speed Trap changes the note scroll speed. Set to 0 to disable Scroll Speed Traps client-side."
        );
        stealthDuration = Config.Bind(
            "Gameplay",
            "Stealth Trap Duration",
            20f,
            "The amount of time a Stealth Trap fades out all notes. Set to 0 to disable Stealth Traps client-side."
        );

        backupCount = Config.Bind(
            "Other",
            "Save Backup Count",
            5,
            "When connecting to Archipelago, some of your save files are backed up in case of an error. This adjusts how many backups to save before the oldest is deleted."
        );
        configIp = Config.Bind(
            "Other",
            "IP",
            "archipelago.gg",
            "The archipelago server IP to connect to. If you're running the game on the official website, this is probably 'archipelago.gg'.\n(This is set automatically by the in-game UI)"
        );
        configPort = Config.Bind(
            "Other",
            "Port",
            58008,
            "The port to connect to. This is all the numbers after the ':' in the IP.\n(This is set automatically by the in-game UI)"
        );
        configSlot = Config.Bind(
            "Other",
            "Slot",
            "Player1",
            "The player slot name to connect with. Enter what you set 'name' to in your YAML.\n(This is set automatically by the in-game UI)"
        );
        configPassword = Config.Bind(
            "Other",
            "Password",
            "",
            "The password to use when connecting. If the server has no password, leave this empty.\n(This is set automatically by the in-game UI)"
        );
    }


    private static void UpdateScene(Scene current, Scene next)
    {
        if(string.IsNullOrEmpty(next.name))
        {
            return;
        }

        if(!ArchipelagoManager.Instance)
        {
            Logger.LogInfo($"Creating manager.");
            new GameObject("Archipelago Manager", typeof(ArchipelagoManager));
        }

        ArchipelagoManager.Instance.UpdateScene(current, next);
    }


    private void Awake()
    {
        try
        {
            // Plugin startup logic
            Logger = base.Logger;

            Logger.LogInfo("Loading config.");
            LoadConfig();

            Muted.Timer.Duration = mutedDuration.Value;
            Rainbow.Timer.Duration = rainbowDuration.Value;
            ScrollSpeed.CrawlTimer.Duration = scrollSpeedDuration.Value;
            ScrollSpeed.ZoomTimer.Duration = scrollSpeedDuration.Value;
            Stealth.Timer.Duration = stealthDuration.Value;

            Logger.LogInfo("Loading assets.");
            DifficultyList.Init();

            ArchipelagoManager.LoadAssetBundles();

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
                Harmony.CreateAndPatchAll(typeof(HighScoreListPatch));

                // Death link
                Harmony.CreateAndPatchAll(typeof(RhythmControllerPatch));
                Harmony.CreateAndPatchAll(typeof(PauseMenuPatch));
                
                // Traps
                Harmony.CreateAndPatchAll(typeof(StealthTrapPatch));
                Harmony.CreateAndPatchAll(typeof(ScrollSpeedTrapPatch));
                Harmony.CreateAndPatchAll(typeof(RainbowTrapPatch));
                Harmony.CreateAndPatchAll(typeof(TrapUpdatePatch));

                // UI Patches
                Harmony.CreateAndPatchAll(typeof(LevelManagerPatch));
                Harmony.CreateAndPatchAll(typeof(UIColorPaletteUpdaterPatch));
            }
            catch(Exception e)
            {
                Logger.LogFatal($"Patching failed with error: {e.Message}, {e.StackTrace}");
                return;
            }

            SceneManager.activeSceneChanged += UpdateScene;

            Logger.LogInfo($"Plugin {PluginReleaseInfo.PLUGIN_GUID} is loaded!");
        }
        catch(Exception e)
        {
            Logger.LogFatal($"{e.Message}, {e.StackTrace}");
        }
    }
}