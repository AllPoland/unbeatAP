using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.Enums;
using Archipelago.MultiClient.Net.Models;
using Archipelago.MultiClient.Net.Helpers;

namespace UNBEATAP.AP;

public class Client
{
    public bool Connected = false;

    public ArchipelagoSession Session;

    public List<string> ReceivedItems = new List<string>();
    public bool ItemsDirty = false;

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


    private void HandleItemReceive(IReceivedItemsHelper helper)
    {
        ItemInfo item = helper.PeekItem();
        string name = item.ItemName;

        ReceivedItems.Add(name);
        ItemsDirty = true;

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

        helper.DequeueItem();
    }


    private void GetQueuedItems()
    {
        while(Session.Items.Any())
        {
            HandleItemReceive(Session.Items);
        }
    }


    private async Task GetStoredItems()
    {
        
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

        await GetStoredItems();
        GetQueuedItems();

        Session.Items.ItemReceived += HandleItemReceive;
    }
}