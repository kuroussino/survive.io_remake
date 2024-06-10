using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public static partial class EventsManager 
{
    #region Lobby
    //Event called when trying to join throught the list of lobby in UI
    public static Action<Lobby> OnClientJoinLobbyWithID;
    //Event called when creating a lobby throught the UI with a name not equal to "" 
    public static Action<string> OnHostCreateLobbyWithName;
    //Event called when the lobby hub must be showed
    public static Action<Lobby, bool> OnShowLobby;
    //Event called when the host press the start button
    public static Action OnHostStartGame;
    //Event called when a player leave a lobby 
    public static Action<Unity.Services.Lobbies.Models.Player> OnQuittedLobby;
    //Event called when the client has entered the lobby
    public static Action<Unity.Services.Lobbies.Models.Player> OnJoinedLobby;
    //Event called when the player clicked the leave lobby button, the lobby should update by removing the player
    public static Action OnLeaveLobbyButton;
    //Event called after the input of the new player name, lobby should register this data for the player
    public static Action<string> OnPlayerNameSet;
    //Event fire when lobby is starting from relay
    public static Action GameStarting;
    //
    public static Action LobbyReady;
    #endregion

    #region GameUI
    //Event called when the player is dead.
    public static Action OnPlayerDead;
    //Initialize the UI of everyPlayer at the start of the game .
    public static Action<float,int,A_Weapon> PlayerUIInitialize;
    //Event called when the local player takes damage.
    public static Action<float, float> OnPlayerDamage;
    //Event called when the player picks up a new weapon, as parameters are needed the weapon and the starting load of ammo on it.
    public static Action<A_Weapon,int> OnNewWeapon;
    //Event called when the player heals himself, as the parameter is needed the amount of heal he restore.
    public static Action<float, float> OnPlayerHeal;
    //Event called when the player picks up an health pack.
    public static Action OnGrabHealthPack;
    //Event called when the player is using an health pack.Did separate methods in case there are more ways to heal the player.
    public static Action OnPlayerUseHealthPack;
    //Event called when the player shoots or reload, as parameter is needed the amount of bullet in the magazine.
    public static Action<int> OnUpdateBulltes;
    //Event need to be called when trying to notificate the player(e.g. The Death Zone is closing in 20 seconds)
    public static Action<object> OnNotificatePlayer;
    //Event is called by the server, in order to execute for every client, when a player dies and the players alive needs to be updated. As parameter use the player left in the game.
    public static Action<int> OnUpdatePlayerCount;
    //Can add killfeed? 
    #endregion
}
