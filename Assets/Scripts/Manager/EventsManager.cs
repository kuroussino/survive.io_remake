using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Lobbies.Models;

public static partial class EventsManager
{
    #region Inputs
    public static Action<Vector2> playerMovementInput;
    public static Action<Vector2> playerAimInput;

    public static Action<bool> playerFireInput;
    public static Action playerReloadInput;
    public static Action playerHealInput;
    public static Action cameraSwitchInput;
    public static Action<bool> cameraPanInput;
    #endregion

    public static Action<Transform> changePlayerCameraTarget;

    public static Action LobbyReady;
    public static Action<Lobby> OnClientJoinLobbyWithID;
    public static Action<string> OnHostCreateLobbyWithName;
}
