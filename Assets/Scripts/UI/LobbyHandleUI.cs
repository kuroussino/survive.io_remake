using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies.Models;
using Unity.VisualScripting;
using UnityEngine;

public class LobbyHandleUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI LobbyNameText;
    [SerializeField] TextMeshProUGUI activePlayerInLobbyText;
    [SerializeField] TextMeshProUGUI maxPlayerInLobbyText; 
    public void UpdateUI(FakeLobbies lobby)
    {
        //LobbyNameText.text = lobby.Name;
        //activePlayerInLobbyText.text = (lobby.MaxPlayers - lobby.AvailableSlots).ToString();
        //maxPlayerInLobbyText.text = lobby.MaxPlayers.ToString();
        LobbyNameText.text = lobby.LobbyName;
        activePlayerInLobbyText.text = lobby.currentPlayer.ToString();
        maxPlayerInLobbyText.text = lobby.maxPlayer.ToString();
    }
}
