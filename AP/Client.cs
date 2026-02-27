using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.Enums;
using Archipelago.MultiClient.Net.Models;
using Archipelago.MultiClient.Net.Helpers;
using UnityEngine;
using Challenges;
using UNBEATAP.Helpers;
using Archipelago.MultiClient.Net.BounceFeatures.DeathLink;
using UNBEATAP.Traps;
using UBUI.Archipelago;

namespace UNBEATAP.AP;

public class Client
{
    private const string ratingLocPrefix = "Rating Unlock ";

    public bool Connected { get; private set; }

    public ArchipelagoSession Session { get; private set; }
    public SlotData SlotData { get; private set; }
    public DeathLinkService DeathLinkService { get; private set; }

    public List<ItemInfo> ReceivedItems = new List<ItemInfo>();
    public int LastCheckedLocation = 0;

    public string primaryCharacter { get; private set; }
    public string secondaryCharacter { get; private set; }

    public bool MissingDlc { get; private set; }

    public static event Action<FailConnectionReason> OnFailConnect;
    public event Action<ItemInfo> OnItemReceived;

    public readonly string ip;
    public readonly int port;
    public readonly string slot;
    public readonly string password;

    public readonly bool deathLink;
    public readonly DeathLinkReason deathLinkBehavior;

    public readonly NotificationPopupMode popupBehavior;

    private Dictionary<long, ScoutedItemInfo> itemFromLocationID;


    public Client()
    {
        ip = "";
        port = 58008;
        slot = "";
        password = "";
        deathLink = false;
        deathLinkBehavior = DeathLinkReason.Fail;

        Connected = false;
        MissingDlc = false;
    }


    public Client(string ip, int port, string slot, string password, bool deathLink, int deathLinkBehavior, int popupBehavior)
    {
        this.ip = ip;
        this.port = port;
        this.slot = slot;
        this.password = password;
        this.deathLink = deathLink;

        this.deathLinkBehavior = DeathLinkReason.Fail;
        switch(deathLinkBehavior)
        {
            default:
            case 0:
                break;
            case 1:
                this.deathLinkBehavior |= DeathLinkReason.Quit;
                break;
            case 2:
                this.deathLinkBehavior |= DeathLinkReason.Restart;
                break;
            case 3:
                this.deathLinkBehavior |= DeathLinkReason.Quit | DeathLinkReason.Restart;
                break;
        }

        switch(popupBehavior)
        {
            default:
            case 0:
                this.popupBehavior = NotificationPopupMode.None;
                break;
            case 1:
                this.popupBehavior = NotificationPopupMode.Sent;
                break;
            case 2:
                this.popupBehavior = NotificationPopupMode.Received;
                break;
            case 3:
                this.popupBehavior = NotificationPopupMode.Sent | NotificationPopupMode.Received;
                break;
        }

        Connected = false;
        MissingDlc = false;

        Plugin.Logger.LogInfo($"Creating session with server {ip}:{port}");

        if(port < 0 || port > ushort.MaxValue)
        {
            Plugin.Logger.LogError($"Tried to set up a client with an invalid port!\n    {port} is not between 0 and {ushort.MaxValue}");
            Session = null;

            OnFailConnect?.Invoke(FailConnectionReason.ClientError);
        }
        else Session = ArchipelagoSessionFactory.CreateSession(ip, port);
    }


    public bool HasReceivedItem(ItemInfo item)
    {
        PlayerInfo player = item.Player;
        return ReceivedItems.Any(
            x =>
                item.ItemId == x.ItemId
                && item.LocationId == x.LocationId
                && item.LocationName != "Cheat Console"
                && item.Flags == x.Flags
                && player != null
                && player.Slot == x.Player?.Slot
                && player.Team == x.Player?.Team
                && !string.IsNullOrEmpty(player.Name)
                && player.Name == x.Player?.Name
                && !string.IsNullOrEmpty(player.Game)
                && player.Game == x.Player?.Game
        );
    }


    public void SetPrimaryCharacter(string primary)
    {
        primaryCharacter = primary;
        Session.DataStorage[Scope.Slot, "primaryCharacter"] = primary;
    }


    public void SetSecondaryCharacter(string secondary)
    {
        secondaryCharacter = secondary;
        Session.DataStorage[Scope.Slot, "secondaryCharacter"] = secondary;
    }


    public void HandleRatingUpdate(float newRating)
    {
        // The in-game rating is always scaled to be 0 to 100
        if(newRating >= 100f)
        {
            Plugin.Logger.LogInfo("Target rating achieved! Setting goal.");
            Session.SetGoalAchieved();
        }

        float ratingStep = 100f / SlotData.ItemCount;
        int checkedRatingCount = Mathf.FloorToInt(newRating / ratingStep);

        List<long> checkedLocations = new List<long>();
        while(LastCheckedLocation < checkedRatingCount)
        {
            LastCheckedLocation += 1;
            string locationName = $"{ratingLocPrefix}{LastCheckedLocation}";
            checkedLocations.Add(Session.Locations.GetLocationIdFromName(Plugin.GameName, locationName));
        }

        if(checkedLocations.Count > 0)
        {
            // Don't send check if it's already checked! On first connect it sometimes sends the last check, even though it's already checked.
            if(Session.Locations.AllLocationsChecked.Contains(checkedLocations.Last())) return;
            Plugin.Logger.LogInfo($"Completing check for {ratingLocPrefix}{LastCheckedLocation}");
            Session.Locations.CompleteLocationChecks(checkedLocations.ToArray());
            HandleItemSend(checkedLocations);
        }
    }

    private void HandleItemSend(List<long> ids)
    {
        foreach(long id in ids)
        {
            // Only show popups for items we're sending to other people
            ScoutedItemInfo item = itemFromLocationID[id];
            if(!item.IsReceiverRelatedToActivePlayer)
            {
                NotificationHelper.QueueNotification($"Sent {item.ItemDisplayName} to {item.Player.Name}", NotificationPopupMode.Sent);
            }
        }
    }

    private void HandleItemReceive(IReceivedItemsHelper helper)
    {
        ItemInfo item = helper.PeekItem();
        if(HasReceivedItem(item))
        {
            Plugin.Logger.LogWarning($"Received duplicate of {item.ItemName} from {item.LocationName}!");
            helper.DequeueItem();
            return;
        }
        ReceivedItems.Add(item);

        string name = item.ItemName;
        bool sentBySelf = item.Player.Slot == Session.Players.ActivePlayer.Slot;
        if(name.StartsWith(DifficultyController.SongNamePrefix))
        {
            DifficultyController.AddProgressiveSong(name, sentBySelf);
        }
        else if(name.StartsWith(CharacterController.CharPrefix))
        {
            CharacterController.AddCharacter(name, sentBySelf);
        }
        else if(name.EndsWith(TrapController.TrapSuffix))
        {
            TrapController.ActivateTrap(name, sentBySelf);
        }
        else
        {
            Plugin.Logger.LogWarning($"Unable to handle item: {name}");
        }

        OnItemReceived?.Invoke(item);

        helper.DequeueItem();
    }


    private void GetQueuedItems()
    {
        while(Session.Items.Any())
        {
            ItemInfo item = Session.Items.PeekItem();
            if(item.ItemName.EndsWith(TrapController.TrapSuffix))
            {
                // Ignore traps from previous sessions
                Session.Items.DequeueItem();
                continue;
            }

            HandleItemReceive(Session.Items);
        }
    }


    private async Task ScoutLocationItems()
    {
        // Scout all locations ahead of time so that we don't have to wait for responses later
        long[] allIDs = new long[SlotData.ItemCount];
        for(int i = 0; i < SlotData.ItemCount; i++)
        {
            int locIndex = i + 1;
            allIDs[i] = Session.Locations.GetLocationIdFromName(Plugin.GameName, $"{ratingLocPrefix}{locIndex}");
        }

        itemFromLocationID = await Session.Locations.ScoutLocationsAsync(HintCreationPolicy.None, allIDs);
    }


    private void HandleDeathLink(DeathLink deathLink)
    {
        DeathLinkController.TryPerformDeathLink(deathLink);
    }


    private FailConnectionReason FailReasonFromFailure(LoginFailure failure)
    {
        if(failure.ErrorCodes.Contains(ConnectionRefusedError.InvalidSlot))
        {
            return FailConnectionReason.BadSlot;
        }
        if(failure.ErrorCodes.Contains(ConnectionRefusedError.InvalidGame))
        {
            return FailConnectionReason.BadGame;
        }
        if(failure.ErrorCodes.Contains(ConnectionRefusedError.InvalidPassword))
        {
            return FailConnectionReason.WrongPassword;
        }
        if(failure.ErrorCodes.Contains(ConnectionRefusedError.SlotAlreadyTaken))
        {
            return FailConnectionReason.SlotTaken;
        }
        if(failure.ErrorCodes.Contains(ConnectionRefusedError.InvalidItemsHandling))
        {
            return FailConnectionReason.BadItemHandle;
        }
        if(failure.ErrorCodes.Contains(ConnectionRefusedError.IncompatibleVersion))
        {
            return FailConnectionReason.BadVersion;
        }

        if(failure.Errors.Contains("A task was canceled."))
        {
            // This is the message for a timeout for some reason
            return FailConnectionReason.Timeout;
        }

        return FailConnectionReason.General;
    }


    public async Task ConnectAndGetData()
    {
        Plugin.Logger.LogInfo($"Connecting to {ip}:{port} with game {Plugin.GameName} as {slot}");
        LoginResult result;
        try
        {
            await Session.ConnectAsync();
            result = await Session.LoginAsync(
                Plugin.GameName,
                slot,
                ItemsHandlingFlags.AllItems,
                null,
                deathLink ? ["DeathLink"] : null,
                null,
                string.IsNullOrEmpty(password) ? null : password,
                true
            );
        }
        catch(Exception e)
        {
            result = new LoginFailure(e.GetBaseException().Message);
        }

        if(!result.Successful)
        {
            LoginFailure failure = (LoginFailure)result;
            string message = $"Failed to connect to {ip}:{port} as {slot}:";
            foreach(string error in failure.Errors)
            {
                message += $"\n    - {error}";
            }
            foreach(ConnectionRefusedError error in failure.ErrorCodes)
            {
                message += $"\n    -# {error}";
            }

            Plugin.Logger.LogError(message);
            Connected = false;

            OnFailConnect?.Invoke(FailReasonFromFailure(failure));
            return;
        }

        try
        {
            SlotData = new SlotData(await Session.DataStorage.GetSlotDataAsync());

            if(!APVersion.CheckConnectionCompatible(SlotData.WorldVersion, SlotData.CompatibleVersions))
            {
                Plugin.Logger.LogError($"Plugin version {PluginReleaseInfo.PLUGIN_VERSION} is not compatible with apworld version {SlotData.WorldVersion}!\n    Please update your plugin and/or apworld!");
                await Session?.Socket.DisconnectAsync();

                SlotData = null;
                Connected = false;

                OnFailConnect?.Invoke(FailConnectionReason.BadVersion);
                return;
            }

            // Backup save files in case our wacky stuff leads to breaking a save
            Plugin.DoBackup();

            if(SlotData.UseBreakout)
            {
                try
                {
                    DlcList dlcs = Resources.Load<DlcList>("DlcList");
                    if(!dlcs.availableDlcs.Contains("DeluxeEdition"))
                    {
                        Plugin.Logger.LogError("The Breakout Edition DLC was enabled in the world configuration, but is not installed!\n    The randomizer may not be possible without the DLC!");
                        MissingDlc = true;
                    }
                }
                catch {}

                if(!MissingDlc)
                {
                    Plugin.Logger.LogInfo($"Breakout Edition DLC is enabled for this randomizer.");
                }
            }

            Session.Items.ItemReceived += HandleItemReceive;
            using Task itemTask = Task.Run(GetQueuedItems);

            Plugin.Logger.LogInfo("Loading previously saved high scores.");
            await HighScoreSaver.LoadHighScores();
            Session.DataStorage[Scope.Slot, HighScoreSaver.LatestScoreKey].OnValueChanged += HighScoreSaver.OnLatestScoreUpdated;

            if(deathLink)
            {
                DeathLinkService = Session.CreateDeathLinkService();
                DeathLinkService.EnableDeathLink();
                DeathLinkService.OnDeathLinkReceived += HandleDeathLink;
            }

            string primarySelected = await Session.DataStorage[Scope.Slot, "primaryCharacter"].GetAsync<string>();
            string secondarySelected = await Session.DataStorage[Scope.Slot, "secondaryCharacter"].GetAsync<string>();

            await ScoutLocationItems();

            // Make sure all items are gathered at this point, so we know what characters we have
            await itemTask;

            SetPrimaryCharacter(string.IsNullOrEmpty(primarySelected) ? "Beat" : primarySelected);
            SetSecondaryCharacter(string.IsNullOrEmpty(secondarySelected) ? "Quaver" : secondarySelected);
            CharacterController.ForceEquipUnlockedCharacter();

            // All connection steps are done, now send the game to archipelago mode
            Connected = true;

            if(ArcadeProgressController.Instance)
            {
                // Force reload arcade progress so patches can take effect
                Plugin.Logger.LogInfo($"Force reloading progress.");
                ArcadeProgressController.Instance.Init();
            }

            LevelManager.LoadLevel(JeffBezosController.arcadeMenuScene);
        }
        catch(Exception e)
        {
            Plugin.Logger.LogError($"Client failed to connect with error: {e.Message}\n    {e.StackTrace}");
            DisconnectAndClose(false);
            OnFailConnect?.Invoke(FailConnectionReason.ClientError);
        }
    }


    public void DisconnectAndClose(bool reload = true)
    {
        Plugin.Logger.LogInfo($"Disconnecting from {ip}:{port}");

        if(Session != null)
        {
            // Doing an async void call but in theory there isn't much we would do by tracking this
            Session.Socket.DisconnectAsync();

            Session.DataStorage[Scope.Slot, HighScoreSaver.LatestScoreKey].OnValueChanged -= HighScoreSaver.OnLatestScoreUpdated;
            Session.Items.ItemReceived -= HandleItemReceive;
        }
        if(DeathLinkService != null)
        {
            DeathLinkService.OnDeathLinkReceived -= HandleDeathLink;
        }

        DifficultyController.Clear();
        DifficultyList.Clear();
        CharacterList.Clear();
        HighScoreHandler.HighScores = new HighScoreList();
        HighScoreHandler.ResetSavedRating();
        TrapController.DeactivateTraps();

        DeathHelper.Reset();

        Plugin.Client = new Client();

        if(ArcadeProgressController.Instance)
        {
            // Force reload arcade progress so removing patches can take effect
            Plugin.Logger.LogInfo($"Force reloading progress.");
            ArcadeProgressController.Instance.Init();
        }

        if(reload)
        {
            // Reload the arcade menu when disconnecting
            LevelManager.LoadLevel(JeffBezosController.arcadeMenuScene);
        }
    }
}