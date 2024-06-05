using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public static partial class EventsManager 
{
    //Event called when trying to join throught the list of lobby in UI
    public static Action<Lobby> OnClientJoinLobbyWithID;
    //Event called when creating a lobby throught the UI with a name not equal to "" 
    public static Action<string> OnHostCreateLobbyWithName;
    //Event called when the lobby hub must be showed
    public static Action<Lobby> OnShowLobby;
    //Event called when the host press the start button
    public static Action OnHostStartGame;
    //Event called when a player leave a lobby 
    public static Action<Unity.Services.Lobbies.Models.Player> OnQuittedLobby;
    //Event called when the client has entered the lobby
    public static Action<Unity.Services.Lobbies.Models.Player> OnJoinedLobby;
}
