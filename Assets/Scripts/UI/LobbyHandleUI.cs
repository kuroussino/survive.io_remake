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
    string lobbyID;
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
    public void UpdateUI(Lobby lobby)
    {
        myLobby = lobby;
        LobbyNameText.text = myLobby.Name;
        activePlayerInLobbyText.text = (myLobby.MaxPlayers - myLobby.AvailableSlots).ToString();
        maxPlayerInLobbyText.text = myLobby.MaxPlayers.ToString();
        lobbyID = myLobby.Id;
 
    }

    /// <summary>
    /// Entering Lobby with ID by clicking on the Join Button
    /// </summary>
    public void joinLobbywithID()
    {
        Debug.Log($"Entering lobby by ID {LobbyNameText.text}");
        EventsManager.OnClientJoinLobbyWithID?.Invoke(myLobby);
    }
    #endregion
}
