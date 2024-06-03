using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public static partial class EventsManager
{
    public static Action<Vector2> playerMovementInput;
    public static Action<Vector2> playerAimInput;

    public static Action playerFireInput;
    public static Action playerReloadInput;
    public static Action playerHealInput;

    #region UI
    public static Action<Lobby> OnClientJoinLobbyWithID;
    public static Action<string> OnHostCreateLobbyWithName;
    #endregion
}
