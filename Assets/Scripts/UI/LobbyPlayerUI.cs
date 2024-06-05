using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class LobbyPlayerUI : MonoBehaviour
{
    #region UIVar
    [SerializeField] TextMeshProUGUI PlayerNameText;
    [SerializeField] TextMeshProUGUI HostOrClientText;
    #endregion
    [HideInInspector] public string playerID { get; private set; }  

    /// <summary>
    /// UpdatePlayerUI will be called by the LobbyUIManager in order to compile the UI and update it 
    /// </summary>
    /// <param name="player"></param>
    public void UpdatePlayerUI(Unity.Services.Lobbies.Models.Player player)
    {
        playerID = player.Id;
        PlayerNameText.text = player.Data["PlayerName"].Value.ToString();
        if (NetworkManager.Singleton.IsHost)
        {
            HostOrClientText.text = "HOST";
        }
        else
        {
            HostOrClientText.text = "CLIENT";
        }
    }
    public void RemovePlayer()
    {
        gameObject.SetActive(false);
        Destroy(gameObject);
    }
}
