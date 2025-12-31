using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.Enums;
using Archipelago.MultiClient.Net.Models;
using Archipelago.MultiClient.Net.Helpers;

namespace UNBEATAP.AP;

public class Client
{
    public bool Connected { get; private set; }

    public ArchipelagoSession Session;

    public List<ItemInfo> ReceivedItems = new List<ItemInfo>();

    public string primaryCharacter;
    public string secondaryCharacter;

    public event Action<ItemInfo> OnItemReceived;

    private string ip;
    private int port;
    private string slot;
    private string password;
    private bool deathLink;


    public Client(string ip, int port, string slot, string password, bool deathLink)
    {
        this.ip = ip;
        this.port = port;
        this.slot = slot;
        this.password = password;
        this.deathLink = deathLink;

        Plugin.Logger.LogInfo($"Creating session with server {ip}:{port}");
        Session = ArchipelagoSessionFactory.CreateSession(ip, port);
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


    private void HandleItemReceive(IReceivedItemsHelper helper)
    {
        ItemInfo item = helper.PeekItem();
        if(ReceivedItems.Contains(item))
        {
            Plugin.Logger.LogWarning($"Received duplicate of {item.ItemName} from {item.LocationName}!");
            helper.DequeueItem();
            return;
        }

        string name = item.ItemName;
        if(name.StartsWith(DifficultyController.SongNamePrefix))
        {
            DifficultyController.AddProgressiveSong(name);
        }
        else if(name.StartsWith(CharacterController.CharPrefix))
        {
            CharacterController.AddCharacter(name);
        }
        else
        {
            Plugin.Logger.LogWarning($"Unable to handle item: {name}");
        }

        ReceivedItems.Add(item);
        OnItemReceived?.Invoke(item);

        helper.DequeueItem();
    }


    private void GetQueuedItems()
    {
        while(Session.Items.Any())
        {
            HandleItemReceive(Session.Items);
        }
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
        
        Connected = true;

        string primarySelected = await Session.DataStorage[Scope.Slot, "primaryCharacter"].GetAsync<string>();
        string secondarySelected = await Session.DataStorage[Scope.Slot, "secondaryCharacter"].GetAsync<string>();

        Session.Items.ItemReceived += HandleItemReceive;
        GetQueuedItems();

        SetPrimaryCharacter(string.IsNullOrEmpty(primarySelected) ? "Beat" : primarySelected);
        SetSecondaryCharacter(string.IsNullOrEmpty(secondarySelected) ? "Quaver" : secondarySelected);
    }
}