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

namespace UNBEATAP.AP;

public class Client
{
    public bool Connected { get; private set; }

    public ArchipelagoSession Session { get; private set; }
    public SlotData SlotData { get; private set; }
    public DeathLinkService DeathLinkService { get; private set; }

    public List<ItemInfo> ReceivedItems = new List<ItemInfo>();
    public int LastCheckedLocation = 0;

    public string primaryCharacter { get; private set; }
    public string secondaryCharacter { get; private set; }

    public bool MissingDlc { get; private set; }

    public event Action<ItemInfo> OnItemReceived;

    public readonly string ip;
    public readonly int port;
    public readonly string slot;
    public readonly string password;
    public readonly bool deathLink;


    public Client()
    {
        ip = "";
        port = 58008;
        slot = "";
        password = "";
        deathLink = false;

        Connected = false;
        MissingDlc = false;
    }


    public Client(string ip, int port, string slot, string password, bool deathLink)
    {
        this.ip = ip;
        this.port = port;
        this.slot = slot;
        this.password = password;
        this.deathLink = deathLink;

        Connected = false;
        MissingDlc = false;

        Plugin.Logger.LogInfo($"Creating session with server {ip}:{port}");
        Session = ArchipelagoSessionFactory.CreateSession(ip, port);
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
        const string ratingLocPrefix = "Rating Unlock ";

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
            Plugin.Logger.LogInfo($"Completing check for {ratingLocPrefix}{LastCheckedLocation}");
            Session.Locations.CompleteLocationChecks(checkedLocations.ToArray());
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
        if(name.StartsWith(DifficultyController.SongNamePrefix))
        {
            DifficultyController.AddProgressiveSong(name);
        }
        else if(name.StartsWith(CharacterController.CharPrefix))
        {
            CharacterController.AddCharacter(name);
        }
        else if(name.EndsWith(TrapController.TrapSuffix))
        {
            TrapController.ActivateTrap(name);
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


    private void HandleDeathLink(DeathLink deathLink)
    {
        DeathLinkController.TryPerformDeathLink(deathLink);
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
                message += $"\n    - {error}";
            }

            Plugin.Logger.LogError(message);
            Connected = false;
            return;
        }

        // Backup save files in case our wacky stuff leads to breaking a save
        Plugin.DoBackup();
        
        Connected = true;

        SlotData = new SlotData(Session.DataStorage.GetSlotData());

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

        Plugin.Logger.LogInfo("Loading previously saved high scores.");
        await HighScoreSaver.LoadHighScores();
        Session.DataStorage[Scope.Slot, HighScoreSaver.LatestScoreKey].OnValueChanged += HighScoreSaver.OnLatestScoreUpdated;

        string primarySelected = await Session.DataStorage[Scope.Slot, "primaryCharacter"].GetAsync<string>();
        string secondarySelected = await Session.DataStorage[Scope.Slot, "secondaryCharacter"].GetAsync<string>();

        Session.Items.ItemReceived += HandleItemReceive;
        GetQueuedItems();

        SetPrimaryCharacter(string.IsNullOrEmpty(primarySelected) ? "Beat" : primarySelected);
        SetSecondaryCharacter(string.IsNullOrEmpty(secondarySelected) ? "Quaver" : secondarySelected);
        CharacterController.ForceEquipUnlockedCharacter();

        if(ArcadeProgressController.Instance)
        {
            // Force reload arcade progress so patches can take effect
            Plugin.Logger.LogInfo($"Force reloading progress.");
            ArcadeProgressController.Instance.Init();
        }

        if(deathLink)
        {
            DeathLinkService = Session.CreateDeathLinkService();
            DeathLinkService.EnableDeathLink();
            DeathLinkService.OnDeathLinkReceived += HandleDeathLink;
        }
    }
}