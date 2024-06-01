using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using CI.PowerConsole;

public class LobbyManager : MonoBehaviour
{
    string LobbyName = "LobbySurviveRemake";
    int maxPlayers = 6;
    [SerializeField] private float timePing;
    [SerializeField] private float hearthTime;
    [SerializeField] private TextMeshProUGUI namePlayer;
    [SerializeField] private TextMeshProUGUI namePlayerStart;
    [SerializeField] private TextMeshProUGUI namePlayerLobby;
    private string playerName;
    //[SerializeField] private GameObject panelStart;
    #region Client
    [SerializeField] private TextMeshProUGUI debugConnectionClient;
    [SerializeField] private GameObject panelClientMode;
    #endregion


    #region Server
    [SerializeField] private GameObject panelServerMode;
    [SerializeField] private TextMeshProUGUI debugConnectionServer;

    #endregion
    Lobby myLobby;
    Lobby joinedLobby;
    ILobbyEvents lobbyEvents;
    //private float timerUpdate = 2f;
    //private float currentTimerUpdate;

    private void Awake()
    {
        PowerConsole.Initialise();
    }
    private async void Start()
    {
        playerName = "TestPlayer" + Random.Range(0, 100);
        namePlayerStart.text = playerName;
        // Create an instance of InitializationOptions
        var initializationOptions = new InitializationOptions();

        // Set custom player name option
        initializationOptions.SetProfile(playerName);

        // Initialize Unity Services with the custom options
        await UnityServices.InitializeAsync(initializationOptions);


        //await UnityServices.InitializeAsync();

        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log($" Sing in {AuthenticationService.Instance.PlayerId}");
            PowerConsole.Log(LogLevel.Debug, $" Sing in {AuthenticationService.Instance.PlayerId}");
        };

        await AuthenticationService.Instance.SignInAnonymouslyAsync();

        PowerConsole.CommandEntered += (s, e) =>
        {
            var enteredCommand = e.Command;
        };

        PowerConsole.RegisterCommand(new CustomCommand()
        {
            Command = "CreateLobby",
            Callback = CreateLobby
        });
        PowerConsole.RegisterCommand(new CustomCommand()
        {
            Command = "SearchLobbies",
            Callback = SearchLobbies
        });
        PowerConsole.RegisterCommand(new CustomCommand()
        {
            Command = "JoinLobby",
            Args = new List<CommandArgument>()
            {
                new CommandArgument() {Name = "-code"}
            },
            Callback = JoinLobby
        });
    }

    public async void CreateLobby(CommandCallback command)
    {
        try
        {
            namePlayerLobby.text = playerName;
            LobbyName = LobbyName+ Random.Range(0, 100);
            CreateLobbyOptions createLobbyOptions = new CreateLobbyOptions
            {
                Player = GetPlayer()
            };
            Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(LobbyName, maxPlayers, createLobbyOptions);
            myLobby = lobby;
            debugConnectionServer.text = $"{myLobby.Name} - {myLobby.MaxPlayers} -  {myLobby.AvailableSlots}";
            print($"{myLobby.Name} - {myLobby.MaxPlayers}");
            PowerConsole.Log(LogLevel.Debug, $"Lobby Created!");
            PowerConsole.Log(LogLevel.Debug, $"Lobby Name: {myLobby.Name} - Lobby Max Players: {myLobby.MaxPlayers} Code: {myLobby.LobbyCode}");
            //var callbacks = new LobbyEventCallbacks();
            //callbacks.LobbyChanged += OnLobbyChanged;
            //callbacks.KickedFromLobby += OnKickedFromLobby;
            //callbacks.LobbyEventConnectionStateChanged += OnLobbyEventConnectionStateChanged;
            //try
            //{
            //    lobbyEvents = await Lobbies.Instance.SubscribeToLobbyEventsAsync(lobby.Id, callbacks);
            //}
            //catch (LobbyServiceException ex)
            //{
            //    switch (ex.Reason)
            //    {
            //        case LobbyExceptionReason.AlreadySubscribedToLobby: Debug.LogWarning($"Already subscribed to lobby[{lobby.Id}]. We did not need to try and subscribe again. Exception Message: {ex.Message}"); break;
            //        case LobbyExceptionReason.SubscriptionToLobbyLostWhileBusy: Debug.LogError($"Subscription to lobby events was lost while it was busy trying to subscribe. Exception Message: {ex.Message}"); throw;
            //        case LobbyExceptionReason.LobbyEventServiceConnectionError: Debug.LogError($"Failed to connect to lobby events. Exception Message: {ex.Message}"); throw;
            //        default: throw;
            //    }
            //}


            StartCoroutine(LobbyHeartbeat());
            PrintPLayers(myLobby);
        }
        catch (LobbyServiceException err)
        {
            //debugConnectionServer.gameObject.SetActive(true);
            //StartCoroutine(DisableText(debugConnectionServer.gameObject));
            print("Error!!");
            PowerConsole.Log(LogLevel.Error, $"Error creating lobby {err}");
            debugConnectionServer.text = "ERROR CONNECTION";
        }


    }

    private void PrintPLayers(Lobby lobby)
    {
        print($"Players in lobby {lobby.Name}");
        PowerConsole.Log(LogLevel.Debug, $"Players in lobby {lobby.Name}");
        foreach(Unity.Services.Lobbies.Models.Player player in lobby.Players)
        {
            print($"Player: {player.Data["PlayerName"].Value}");
            PowerConsole.Log(LogLevel.Debug, $"Player: {player.Data["PlayerName"].Value}");
        }
    }

    private void OnLobbyEventConnectionStateChanged(LobbyEventConnectionState state)
    {
        print("State change!!!");
    }

    private void OnKickedFromLobby()
    {
        print("kicked event change!!!");
    }

    private async void OnLobbyChanged(ILobbyChanges changes)
    {
        print($"call event change method");
        if (changes.LobbyDeleted)
        {
            // Handle lobby being deleted
            // Calling changes.ApplyToLobby will log a warning and do nothing
        }
        else
        {
            print($"call event change");
            print(changes.PlayerLeft.Value.Count);
            changes.ApplyToLobby(myLobby);
            print(changes.PlayerLeft.Value.Count);
            if (changes.PlayerLeft.Changed)
            {
                print($"player left change !!");
                foreach (var player in changes.PlayerLeft.Value)
                {
                    print($"kicking some player {player}");
                    try
                    {

                        await LobbyService.Instance.RemovePlayerAsync(myLobby.Id, player.ToString());
                        print($"kicked player {player}");
                    }
                    catch (LobbyServiceException)
                    {

                    }
                }
            }
            else
            {
                print($"player left NOOOOOOOOT change !!");
            }
        }
    }

    public async void SearchLobbies(CommandCallback command)
    {
        namePlayer.text = playerName;
        QueryLobbiesOptions optionsQueryLobbie = new QueryLobbiesOptions
        {
            Count = 10,
            Filters = new List<QueryFilter>()
            {
                //new QueryFilter(QueryFilter.FieldOptions.MaxPlayers, "2", QueryFilter.OpOptions.LT),
                new QueryFilter(QueryFilter.FieldOptions.AvailableSlots, "0", QueryFilter.OpOptions.GT)
            },
            Order = new List<QueryOrder>
            {
                new QueryOrder(false, QueryOrder.FieldOptions.Created)
            }
        };
        try
        {
            QueryResponse response = await Lobbies.Instance.QueryLobbiesAsync(optionsQueryLobbie);
            print($"Avaiable lobbies {response.Results.Count}");
            PowerConsole.Log(LogLevel.Debug, $"Avaiable lobbies number: {response.Results.Count}");
            if (response.Results.Count <= 0)
            {
                print("No Lobbie avaiable");
                PowerConsole.Log(LogLevel.Warning, $"No Lobbie avaiable");
                debugConnectionClient.text = "No Lobbie avaiable";
                //StartCoroutine(DisableText(debugConnectionClient.gameObject));
                return;
            }
            //print("exist lobbie?: " + response.Results[0].Id + response.Results[0].Name);
            //Lobby lobby = await Lobbies.Instance.JoinLobbyByIdAsync(response.Results[0].Id);
            //joinedLobby = lobby;
            string result = "";
            foreach(Lobby lobbie in response.Results)
            {
                result = result + "Lobbie Name: " + lobbie.Name + "Lobbie slots free: " + lobbie.AvailableSlots + "\n";
            }
            debugConnectionClient.text = result;
            PowerConsole.Log(LogLevel.Debug, $"all lobbies found: {result}");

            //print("players: " + myLobby.Players.Count);
            //NetworkManager.Singleton.StartClient();
            //panelPlayStart.SetActive(true);
            //panelClientMode.SetActive(false);
        }
        catch (LobbyServiceException error)
        {
            print("ERROR IN CONNECT LOBBY " + error);
            PowerConsole.Log(LogLevel.Error, $"Error found lobbies {error}");
            debugConnectionClient.text = "Error connecting to lobbie";
            //debugConnectionClient.gameObject.SetActive(true);
            //StartCoroutine(DisableText(debugConnectionClient.gameObject));
            throw;
        }
        //UpdateLobby();
    }

    public async void JoinLobby(CommandCallback command)
    {
        var lobbyToJoin = command.Args["-code"];
        JoinLobbyByCodeOptions joinLobbyByCodeOptions = new JoinLobbyByCodeOptions()
        {
            Player = GetPlayer()
        };
        try 
        {
            Lobby lobby = await Lobbies.Instance.JoinLobbyByCodeAsync(lobbyToJoin, joinLobbyByCodeOptions);
            joinedLobby = lobby;
            print($"Joined to {joinedLobby.Name}");
            PowerConsole.Log(LogLevel.Debug, $"Joined to {joinedLobby.Name}");
            PrintPLayers(joinedLobby); 
        }
        catch(LobbyServiceException err)
        {
            print($"Error joined lobby {err}");
            PowerConsole.Log(LogLevel.Error, $"Error joined lobby {err}");
        }

    }
    private IEnumerator LobbyHeartbeat()
    {
        print("Call coroutine hearthbeat");
        PowerConsole.Log(LogLevel.Debug, $"Call coroutine hearthbeat");
        while (myLobby != null)
        {
            print("Lobby is not null in coroutine");
            PowerConsole.Log(LogLevel.Debug, $"Lobby is not null in coroutine");
            yield return new WaitForSeconds(hearthTime);
            print($"After {hearthTime} seconds");
            PowerConsole.Log(LogLevel.Debug, $"After {hearthTime} seconds");
            LobbyService.Instance.SendHeartbeatPingAsync(myLobby.Id);
            print("ping lobby for not die");
            PowerConsole.Log(LogLevel.Debug, $"ping lobby for not die");
        }
        print("Stop corotutine hearthbeat");
        PowerConsole.Log(LogLevel.Debug, $"Stop corotutine hearthbeat");

        //while (true)
        //{
        //    yield return new WaitForSeconds(time);

        //}
    }

    private Unity.Services.Lobbies.Models.Player GetPlayer()
    {
        return new Unity.Services.Lobbies.Models.Player
        {
            Data = new Dictionary<string, PlayerDataObject>
                    {
                        { "PlayerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, playerName) }
                    }
        };
    }

    private void OnApplicationQuit()
    {
        //print("call application kick");
        //try
        //{
        //    LobbyService.Instance.RemovePlayerAsync(myLobby.Id, AuthenticationService.Instance.PlayerId);
        //}
        //catch (LobbyServiceException err) { }
        //print($"kicked myself {player}");
    }

}

