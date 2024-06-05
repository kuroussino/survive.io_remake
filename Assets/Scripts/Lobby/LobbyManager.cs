using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using CI.PowerConsole;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Networking.Transport.Relay;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine.SceneManagement;
using System.ComponentModel;
using System.Threading.Tasks;
using Unity.VisualScripting.Antlr3.Runtime.Misc;

public class LobbyManager : MonoBehaviour
{
    string LobbyName = "LobbySurviveRemake";
    int maxPlayers = 6;
    [SerializeField] private int minimumPlayersForStart = 2; 
    [SerializeField] private float timeUpdate;
    [SerializeField] private float hearthTime;
    [SerializeField] private float checkConnectedTimer;
    [SerializeField] private float confirmedConnectedTimer;
    private float currentTimer;
    private float currentTimeForConnected;
    private bool doOnceConnected = true;
    private bool doOnceResetPlayers = true;
    private string playerName;
    private int countForStartGame;
    private int countOfPlayersLoaded;
    private bool doOnce = true;
    [SerializeField] private GameObject playerPrefab;
    Lobby myLobby;
    //ILobbyEvents lobbyEvents;
    //private float timerUpdate = 2f;
    //private float currentTimerUpdate;

    private void OnEnable()
    {
        EventsManager.OnHostCreateLobbyWithName += CreateLobby;
        EventsManager.OnClientJoinLobbyWithID += JoinLobby;
    }
    private void OnDisable()
    {
        EventsManager.OnHostCreateLobbyWithName -= CreateLobby;
        EventsManager.OnClientJoinLobbyWithID -= JoinLobby;
    }

    private void Awake()
    {
        PowerConsole.Initialise();
        DontDestroyOnLoad(this.gameObject);   

    }

    private async void Start()

    {
        NetworkManager.Singleton.OnClientConnectedCallback += ConnectedClientEvent;
        playerName = "TestPlayer" + Random.Range(0, 100);
        var initializationOptions = new InitializationOptions();

        initializationOptions.SetProfile(playerName);

        await UnityServices.InitializeAsync(initializationOptions);


        //await UnityServices.InitializeAsync();

        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log($" Sing in {AuthenticationService.Instance.PlayerId}");
            PowerConsole.Log(CI.PowerConsole.LogLevel.Debug, $" Sing in {AuthenticationService.Instance.PlayerId} - {playerName}");
        };

        await AuthenticationService.Instance.SignInAnonymouslyAsync();

        EventsManager.LobbyReady?.Invoke();

        #region Powerconsole
        PowerConsole.CommandEntered += (s, e) =>
        {
            var enteredCommand = e.Command;
        };

        PowerConsole.RegisterCommand(new CustomCommand()
        {
            Command = "createlobby",
            Callback = CreateLobbyCommand
        });
        PowerConsole.RegisterCommand(new CustomCommand()
        {
            Command = "searchlobbies",
            Callback = SearchLobbiesCommand
        });
        PowerConsole.RegisterCommand(new CustomCommand()
        {
            Command = "joinlobby",
            Args = new List<CommandArgument>()
            {
                new CommandArgument() {Name = "-code"}
            },
            Callback = JoinLobbyCommand
        });
        PowerConsole.RegisterCommand(new CustomCommand()
        {
            Command = "printinfolobby",
            Callback = PrintInfoLobbyCommand
        });
        PowerConsole.RegisterCommand(new CustomCommand()
        {
            Command = "leavelobby",
            Callback = LeaveLobbyCommand
        });
        PowerConsole.RegisterCommand(new CustomCommand()
        {
            Command = "starthost",
            Callback = StartHostCommand
        });
        PowerConsole.RegisterCommand(new CustomCommand()
        {
            Command = "startclient",
            Args = new List<CommandArgument>()
            {
                new CommandArgument() {Name = "-code"}
            },
            Callback = JoinRelayCommand
        });
        #endregion


    }

    private async void Update()
    {
        if (myLobby != null)
        {
            PowerConsole.Log(CI.PowerConsole.LogLevel.Debug, $"last updated lobby {myLobby.LastUpdated}");
            //PowerConsole.Log(CI.PowerConsole.LogLevel.Debug, $"host id : {myLobby.HostId}");
            currentTimer += Time.deltaTime;
            currentTimeForConnected += Time.deltaTime;
            if (currentTimeForConnected < confirmedConnectedTimer && doOnceResetPlayers && isHost())
            {
                doOnceResetPlayers = false;
                PowerConsole.Log(CI.PowerConsole.LogLevel.Debug, $"call reset players connection in time {currentTimeForConnected}");
                foreach (Unity.Services.Lobbies.Models.Player player in myLobby.Players)
                {
                    await Lobbies.Instance.UpdatePlayerAsync(myLobby.Id, player.Id, new UpdatePlayerOptions
                    {
                        Data = new Dictionary<string, PlayerDataObject>
                        {
                            { "IsConnected", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, "false") }
                        }
                    });
                }

            }
            if (currentTimer >= timeUpdate)
            {
                currentTimer = 0;
                try
                {
                    myLobby = await LobbyService.Instance.GetLobbyAsync(myLobby.Id);
                    
                    PowerConsole.Log(CI.PowerConsole.LogLevel.Debug, $"lobby updated");
                    PowerConsole.Log(CI.PowerConsole.LogLevel.Debug, $"current host id: {myLobby.HostId}");

                    foreach (Unity.Services.Lobbies.Models.Player player in myLobby.Players)
                    {
                        PowerConsole.Log(CI.PowerConsole.LogLevel.Debug, $"status of my player {player.Data["PlayerName"].Value} is: {player.Data["IsConnected"].Value}");
                    }
                    if (myLobby.Data["KEY_START_GAME"].Value != "null" && !isHost() && doOnce)
                    {
                        doOnce = false;
                        PowerConsole.Log(CI.PowerConsole.LogLevel.Debug, $"key for start game: {myLobby.Data["KEY_START_GAME"].Value} is not null call join client");
                        JoinRelay();
                    }
                }
                catch (LobbyServiceException err)
                {
                    PowerConsole.Log(CI.PowerConsole.LogLevel.Error, $"Error updating lobby : {err}");
                }

            }
            if(currentTimeForConnected >= confirmedConnectedTimer && doOnceConnected)
            {
                PowerConsole.Log(CI.PowerConsole.LogLevel.Debug, $"call method for change status connection player in {currentTimeForConnected} and doOnce is {doOnceConnected}");
                doOnceConnected = false;
                await SetPlayerConnection("true");
            }
            if(currentTimeForConnected > checkConnectedTimer)
            {
                PowerConsole.Log(CI.PowerConsole.LogLevel.Debug, $"call checkstatus connection in time {currentTimeForConnected}");
                currentTimeForConnected = 0;
                doOnceConnected = true;
                foreach (Unity.Services.Lobbies.Models.Player player in myLobby.Players)
                {
                    if (isHost() && player.Data["IsConnected"].Value.Equals("false"))
                    {
                        PowerConsole.Log(CI.PowerConsole.LogLevel.Debug, $"this player must be kicked - {player.Data["PlayerName"].Value} - {player.Data["IsConnected"].Value}");
                        await KickPlayer(player.Id);
                    }
                }
                //await SetPlayerConnection("false");
            }

        }
    }

    private async Task SetPlayerConnection(string status)
    {
        PowerConsole.Log(CI.PowerConsole.LogLevel.Debug, $"inside set player connection status to set is : {status}");
        foreach (Unity.Services.Lobbies.Models.Player player in myLobby.Players)
        {
            if (player.Id == AuthenticationService.Instance.PlayerId)
            {
                await Lobbies.Instance.UpdatePlayerAsync(myLobby.Id, player.Id, new UpdatePlayerOptions
                {
                    Data = new Dictionary<string, PlayerDataObject>
                        {
                            { "IsConnected", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, status) }
                        }
                });
            }
        }
    }
    private Unity.Services.Lobbies.Models.Player GetPlayer()
    {
        return new Unity.Services.Lobbies.Models.Player
        {
            Data = new Dictionary<string, PlayerDataObject>
                    {
                        { "PlayerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, playerName) },
                        { "IsConnected", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, "false") }
                    }        
        };
    }


    public async void CreateLobby(string lobbyFromUI)
    {
        try
        {
            CreateLobbyOptions createLobbyOptions = new CreateLobbyOptions
            {
                
                Player = GetPlayer(),
                Data = new Dictionary<string, DataObject>
                {
                    
                    { "KEY_START_GAME", new DataObject(DataObject.VisibilityOptions.Member, "null") }
                }
            };
            Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyFromUI, maxPlayers, createLobbyOptions);
            myLobby = lobby;
           
            print ($"Lobby Created!");
            print($"Lobby Name: {myLobby.Name} - Lobby Max Players: {myLobby.MaxPlayers} Code: {myLobby.LobbyCode}");
            PowerConsole.Log(CI.PowerConsole.LogLevel.Debug, $"Lobby Created!");
            PowerConsole.Log(CI.PowerConsole.LogLevel.Debug, $"Lobby Name: {myLobby.Name} - Lobby Max Players: {myLobby.MaxPlayers} Code: {myLobby.LobbyCode}");
            StartCoroutine(LobbyHeartbeat());
            PrintPLayers(myLobby);
        }
        catch (LobbyServiceException err)
        {
            print($"Error creating lobby {err}");
        }


    }


    public async Task<List<Lobby>> SearchLobbies()
    {
        QueryLobbiesOptions optionsQueryLobbie = new QueryLobbiesOptions
        {
            Count = 10,
            Filters = new List<QueryFilter>()
            {
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
            if (response.Results.Count <= 0)
            {
                print("No Lobbie avaiable");
                return null;
            }
            string result = "";
            foreach (Lobby lobbie in response.Results)
            {
                result = result + "Lobbie Name: " + lobbie.Name + "Lobbie slots free: " + lobbie.AvailableSlots + "\n";
            }
            return response.Results;
        }
        catch (LobbyServiceException error)
        {
            print("ERROR IN CONNECT LOBBY " + error);
            return null;
        }
    }

    public async void JoinLobby(Lobby lobbyToJoin)
    {
        JoinLobbyByIdOptions joinLobbyById = new JoinLobbyByIdOptions()
        {
            Player = GetPlayer()
        };
        try
        {
            Lobby lobby = await Lobbies.Instance.JoinLobbyByIdAsync(lobbyToJoin.Id, joinLobbyById);
            myLobby = lobby;
            print($"Joined to {myLobby.Name}");
            PrintPLayers(myLobby);
        }
        catch (LobbyServiceException err)
        {
            print($"Error joined lobby {err}");
        }

    }

    private void PrintPLayers(Lobby lobby)
    {
        print($"Players in lobby {lobby.Name}");
        foreach(Unity.Services.Lobbies.Models.Player player in lobby.Players)
        {
            print($"Player: {player.Data["PlayerName"].Value}");
        }
    }

    public async Task LeaveLobby()
    {
        try
        {
            await LobbyService.Instance.RemovePlayerAsync(myLobby.Id, AuthenticationService.Instance.PlayerId);
            print($"Player {playerName} leave {myLobby.Name}");
            PowerConsole.Log(CI.PowerConsole.LogLevel.Debug, $"Player {playerName} leave {myLobby.Name}");
        }
        catch (LobbyServiceException err)
        {
            print($"Error in leavin player {playerName} from lobby {myLobby.Name} - errror: {err}");
            PowerConsole.Log(CI.PowerConsole.LogLevel.Debug, $"Player {playerName} leave {myLobby.Name}");
        }

    }

    public async Task KickPlayer(string playerId)
    {
        Unity.Services.Lobbies.Models.Player currentPlayerToKick = null;
        foreach (Unity.Services.Lobbies.Models.Player player in myLobby.Players)
        {
            if(player.Id == playerId)
            {
                currentPlayerToKick = player;
            }
        }
        if(currentPlayerToKick != null)
        {
            try
            {
                await LobbyService.Instance.RemovePlayerAsync(myLobby.Id, playerId);
                print($"Player {currentPlayerToKick.Data["PlayerName"].Value} leave {myLobby.Name}");
                PowerConsole.Log(CI.PowerConsole.LogLevel.Debug, $"Kicked Player {playerName} from {myLobby.Name}");
            }
            catch (LobbyServiceException err)
            {
                print($"Error in leavin player {currentPlayerToKick.Data["PlayerName"].Value} from lobby {myLobby.Name} - errror: {err}");
                PowerConsole.Log(CI.PowerConsole.LogLevel.Debug, $"Error in kicking player {currentPlayerToKick.Data["PlayerName"].Value} from lobby {myLobby.Name} - errror: {err}");
            }
        }
    }
    private async void JoinRelay()
    {
        try
        {
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(myLobby.Data["KEY_START_GAME"].Value);

            RelayServerData relayServerData = new RelayServerData(joinAllocation, "dtls");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
            NetworkManager.Singleton.StartClient();
            PowerConsole.Log(CI.PowerConsole.LogLevel.Debug, $"all connected client started");
        }
        catch (RelayServiceException err)
        {
            PowerConsole.Log(CI.PowerConsole.LogLevel.Error, $"Error joining relay: {err}");
            //throw;
        }
    }

    private void ConnectedClientEvent(ulong obj)
    {
        countForStartGame++;
        PowerConsole.Log(CI.PowerConsole.LogLevel.Debug, $"client connected current count is {countForStartGame}");
        if(countForStartGame == myLobby.Players.Count && isHost())
        {
            PowerConsole.Log(CI.PowerConsole.LogLevel.Debug, $"HOST START GAME ! YEAH!");
            NetworkManager.Singleton.SceneManager.OnLoadComplete += LoadCompleteEvent;
            NetworkManager.Singleton.SceneManager.LoadScene("TestGameplay", LoadSceneMode.Single);
        }
    }

    private void LoadCompleteEvent(ulong clientId, string sceneName, LoadSceneMode loadSceneMode)
    {
        PowerConsole.Log(CI.PowerConsole.LogLevel.Debug, $"{clientId} finish load scene");
        countOfPlayersLoaded++;
        if(countOfPlayersLoaded == myLobby.Players.Count)
        {
            PowerConsole.Log(CI.PowerConsole.LogLevel.Debug, $"all players are sync in scene");
            foreach(NetworkClient client in NetworkManager.Singleton.ConnectedClientsList)
            {
                GameObject playerInstance = Instantiate(playerPrefab);
                playerInstance.GetComponent<NetworkObject>().SpawnAsPlayerObject(client.ClientId);
            }
            CloseLobby();
            
        }
    }

    private async void CloseLobby()
    {
        foreach(Unity.Services.Lobbies.Models.Player player in myLobby.Players)
        {
            if (!isHost())
            {
                await KickPlayer(player.Id);
            }
            
        }
        await LeaveLobby();
        PowerConsole.Log(CI.PowerConsole.LogLevel.Debug, $"Lobby close with no errors");
        Destroy(this.gameObject);
    }


    private bool isHost()
    {
        if(AuthenticationService.Instance.PlayerId == myLobby.HostId)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private IEnumerator LobbyHeartbeat()
    {
        while (myLobby != null)
        {
            yield return new WaitForSeconds(hearthTime);
            LobbyService.Instance.SendHeartbeatPingAsync(myLobby.Id);
            print("ping lobby for not die");
            PowerConsole.Log(CI.PowerConsole.LogLevel.Debug, $"ping lobby for not die");
        }
        print("Stop corotutine hearthbeat");
        PowerConsole.Log(CI.PowerConsole.LogLevel.Debug, $"Stop corotutine hearthbeat");

    }

    #region Commands

    private async void CreateLobbyCommand(CommandCallback command)
    {
        try
        {
            //namePlayerLobby.text = playerName;
            LobbyName = LobbyName + Random.Range(0, 100);
            CreateLobbyOptions createLobbyOptions = new CreateLobbyOptions
            {
                Player = GetPlayer(),
                Data = new Dictionary<string, DataObject>
                {
                    { "KEY_START_GAME", new DataObject(DataObject.VisibilityOptions.Member, "null") }
                }
            };
            Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(LobbyName, maxPlayers, createLobbyOptions);
            myLobby = lobby;
            
            print($"{myLobby.Name} - {myLobby.MaxPlayers}");
            PowerConsole.Log(CI.PowerConsole.LogLevel.Debug, $"Lobby Created!");
            PowerConsole.Log(CI.PowerConsole.LogLevel.Debug, $"Lobby Name: {myLobby.Name} - Lobby Max Players: {myLobby.MaxPlayers} Code: {myLobby.LobbyCode}");
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
            //        case LobbyExceptionReason.SubscriptionToLobbyLostWhileBusy: Debug.LogError($"Subscription to lobby events was lost while it was busy trying to subscribe. Exception Message: {ex.Message}"); //throw;
            //        case LobbyExceptionReason.LobbyEventServiceConnectionError: Debug.LogError($"Failed to connect to lobby events. Exception Message: {ex.Message}"); //throw;
            //        default: //throw;
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
            PowerConsole.Log(CI.PowerConsole.LogLevel.Error, $"Error creating lobby {err}");
           
        }


    }
    private async void LeaveLobbyCommand(CommandCallback command)
    {
        try
        {
            await LobbyService.Instance.RemovePlayerAsync(myLobby.Id, AuthenticationService.Instance.PlayerId);
            print($"Player {playerName} leave {myLobby.Name}");
            PowerConsole.Log(CI.PowerConsole.LogLevel.Debug, $"Player {playerName} leave {myLobby.Name}");
        }
        catch (LobbyServiceException)
        {
            print($"Error in leavin player {playerName} from lobby {myLobby.Name}");
            PowerConsole.Log(CI.PowerConsole.LogLevel.Error, $"Error in leavin player {playerName} from lobby {myLobby.Name}");
        }

    }

    private async void JoinLobbyCommand(CommandCallback command)
    {
        var lobbyToJoin = command.Args["-code"];
        JoinLobbyByCodeOptions joinLobbyByCodeOptions = new JoinLobbyByCodeOptions()
        {
            Player = GetPlayer()
        };
        try
        {
            Lobby lobby = await Lobbies.Instance.JoinLobbyByCodeAsync(lobbyToJoin, joinLobbyByCodeOptions);
            myLobby = lobby;
            print($"Joined to {myLobby.Name}");
            PowerConsole.Log(CI.PowerConsole.LogLevel.Debug, $"Joined to {myLobby.Name}");
            PrintPLayers(myLobby);
        }
        catch (LobbyServiceException err)
        {
            print($"Error joined lobby {err}");
            PowerConsole.Log(CI.PowerConsole.LogLevel.Error, $"Error joined lobby {err}");
        }

    }

    private async void SearchLobbiesCommand(CommandCallback command)
    {
        //namePlayer.text = playerName;
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
            PowerConsole.Log(CI.PowerConsole.LogLevel.Debug, $"Avaiable lobbies number: {response.Results.Count}");
            if (response.Results.Count <= 0)
            {
                print("No Lobbie avaiable");
                PowerConsole.Log(CI.PowerConsole.LogLevel.Warning, $"No Lobbie avaiable");

                //StartCoroutine(DisableText(debugConnectionClient.gameObject));
                return;
            }
            //print("exist lobbie?: " + response.Results[0].Id + response.Results[0].Name);
            //Lobby lobby = await Lobbies.Instance.JoinLobbyByIdAsync(response.Results[0].Id);
            //joinedLobby = lobby;
            string result = "";
            foreach (Lobby lobbie in response.Results)
            {
                result = result + "Lobbie Name: " + lobbie.Name + "Lobbie slots free: " + lobbie.AvailableSlots + "\n";
            }

            PowerConsole.Log(CI.PowerConsole.LogLevel.Debug, $"all lobbies found: {result}");

            //print("players: " + myLobby.Players.Count);
            //NetworkManager.Singleton.StartClient();
            //panelPlayStart.SetActive(true);
            //panelClientMode.SetActive(false);
        }
        catch (LobbyServiceException error)
        {
            print("ERROR IN CONNECT LOBBY " + error);
            PowerConsole.Log(CI.PowerConsole.LogLevel.Error, $"Error found lobbies {error}");

            //debugConnectionClient.gameObject.SetActive(true);
            //StartCoroutine(DisableText(debugConnectionClient.gameObject));
            //throw;
        }
        //UpdateLobby();
    }

    private void PrintInfoLobbyCommand(CommandCallback callback)
    {
        print($"Players in lobby {myLobby}");
        PowerConsole.Log(CI.PowerConsole.LogLevel.Debug, $"Players in lobby {myLobby.Name}");
        foreach (Unity.Services.Lobbies.Models.Player player in myLobby.Players)
        {
            print($"Player: {player.Data["PlayerName"].Value}");
            PowerConsole.Log(CI.PowerConsole.LogLevel.Debug, $"Player: {player.Data["PlayerName"].Value}");
        }
    }

    private async void StartHostCommand(CommandCallback callbacl)
    {
        if(isHost())
        {
            if(myLobby.Players.Count >= minimumPlayersForStart)
            {
                PowerConsole.Log(CI.PowerConsole.LogLevel.Debug, "host can start lobby");
                try
                {
                    Allocation allocation =  await RelayService.Instance.CreateAllocationAsync(myLobby.Players.Count);
                    string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
                    PowerConsole.Log(CI.PowerConsole.LogLevel.Debug, $"Relay created correctly. code relay: {joinCode}");

                    RelayServerData relayServerData = new RelayServerData(allocation,"dtls");
                    NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
                    NetworkManager.Singleton.StartHost();
                    PowerConsole.Log(CI.PowerConsole.LogLevel.Debug, $"All connected host started");

                    myLobby = await Lobbies.Instance.UpdateLobbyAsync(myLobby.Id, new UpdateLobbyOptions
                    {
                        Data = new Dictionary<string, DataObject>
                        {
                            { "KEY_START_GAME", new DataObject(DataObject.VisibilityOptions.Member, joinCode) }
                        }
                    });
                    PowerConsole.Log(CI.PowerConsole.LogLevel.Debug, $"key lobby share");

                }
                catch (RelayServiceException err)
                {
                    PowerConsole.Log(CI.PowerConsole.LogLevel.Error, $"Error creating relay: {err}");
                    //throw;
                }
                
            }
            else
            {
                PowerConsole.Log(CI.PowerConsole.LogLevel.Warning, "not minimun players");
            }
        }
        else
        {
            PowerConsole.Log(CI.PowerConsole.LogLevel.Error, "only host can start lobby");
        }
    }

    private async void JoinRelayCommand(CommandCallback callback)
    {
        var code = callback.Args["-code"];
        try
        {
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(code);

            RelayServerData relayServerData = new RelayServerData(joinAllocation, "dtls");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
            NetworkManager.Singleton.StartClient();
            PowerConsole.Log(CI.PowerConsole.LogLevel.Debug, $"all connected client started");
        }
        catch (RelayServiceException err)
        {
            PowerConsole.Log(CI.PowerConsole.LogLevel.Error, $"Error joining relay: {err}");
            //throw;
        }
    }
    #endregion

}

