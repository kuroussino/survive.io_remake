using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public static partial class EventsManager 
{
    public static Action<Lobby> OnClientJoinLobbyWithID;
    public static Action<string> OnHostCreateLobbyWithName;
}
