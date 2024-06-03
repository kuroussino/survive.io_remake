using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies.Models;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class LobbyHandleUI : MonoBehaviour
{
    #region UI Var
    [SerializeField] TextMeshProUGUI LobbyNameText;
    [SerializeField] TextMeshProUGUI activePlayerInLobbyText;
    [SerializeField] TextMeshProUGUI maxPlayerInLobbyText;
    [SerializeField] Button joinButton;
    Lobby myLobby;
    #endregion

    #region Mono
    private void Awake()
    {
        joinButton.onClick.AddListener(joinLobbywithID);
    }
    #endregion

    #region Methods
    /// <summary>
    /// Updating UI based on lobby data
    /// </summary>
    /// <param name="lobby"></param>
    public void UpdateUI(FakeLobbies lobby)
    {
        //LobbyNameText.text = lobby.Name;
        //activePlayerInLobbyText.text = (lobby.MaxPlayers - lobby.AvailableSlots).ToString();
        //maxPlayerInLobbyText.text = lobby.MaxPlayers.ToString();
        LobbyNameText.text = lobby.LobbyName;
        activePlayerInLobbyText.text = lobby.currentPlayer.ToString();
        maxPlayerInLobbyText.text = lobby.maxPlayer.ToString();
        //lobbyID=lobbby.id;
        //myLobby=lobby;
    }

    /// <summary>
    /// Entering Lobby with ID by clicking on the Join Button
    /// </summary>
    public void joinLobbywithID()
    {
        Debug.Log($"Entering lobby by ID {LobbyNameText.text}");
        //EventsManager.OnClientJoinLobbyWithID?.Invoke(myLobby);
    }
    #endregion
}
