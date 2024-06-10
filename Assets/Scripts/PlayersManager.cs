using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayersManager : NetworkBehaviour
{
    List<Player> players = new List<Player>();
    private void OnEnable()
    {
        EventsManager.playerJoinedBattle += OnPlayerJoinedBattle;
        EventsManager.playerDeath += OnPlayerDeath;
    }
    private void OnDisable()
    {
        EventsManager.playerJoinedBattle -= OnPlayerJoinedBattle;
        EventsManager.playerDeath -= OnPlayerDeath;
    }

    private void OnPlayerDeath(Player player)
    {
        if (!players.Contains(player))
            return;

        players.Remove(player);
        UpdatePlayersAmountClientRpc(players.Count);
    }

    private void OnPlayerJoinedBattle(Player player)
    {
        if (players.Contains(player))
            return;

        players.Add(player);
        UpdatePlayersAmountClientRpc(players.Count);
    }
    [ClientRpc]
    void UpdatePlayersAmountClientRpc(int playersAmount)
    {
        EventsManager.OnUpdatePlayerCount?.Invoke(playersAmount);
    }
}
