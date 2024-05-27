using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviour
{
    string LobbyName = "LobbySurviveRemake";
    int maxPlayers = 6;
    [SerializeField] private float timePing;
    [SerializeField] private float hearthTime;
    //[SerializeField] private GameObject panelStart;
    #region Client
    [SerializeField] private TextMeshProUGUI debugConnectionClient;
    [SerializeField] private GameObject panelClientMode;
    #endregion


    #region Server
    [SerializeField] private GameObject panelServerMode;
    [SerializeField] private TextMeshProUGUI debugConnectionServer;

    #endregion
    Lobby myLobby;
    //private float timerUpdate = 2f;
    //private float currentTimerUpdate;
    private async void Start()
    {
        await UnityServices.InitializeAsync();

        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log($" Sing in {AuthenticationService.Instance.PlayerId}");
        };

        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    public async void CreateLobby()
    {
        try
        {
            Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(LobbyName, maxPlayers);
            myLobby = lobby;
            debugConnectionServer.text = $"{myLobby.Name} - {myLobby.MaxPlayers}";
            print($"{myLobby.Name} - {myLobby.MaxPlayers}");

            StartCoroutine(LobbyHeartbeat());
        }
        catch (LobbyServiceException)
        {
            //debugConnectionServer.gameObject.SetActive(true);
            //StartCoroutine(DisableText(debugConnectionServer.gameObject));
            print("Error!!");
            debugConnectionServer.text = "ERROR CONNECTION";
        }


    }

    public async void ConnectToLobby(string mode)
    {
        QueryLobbiesOptions optionsQueryLobbie = new QueryLobbiesOptions
        {
            Count = 10,
            Filters = new List<QueryFilter>()
            {
                //new QueryFilter(QueryFilter.FieldOptions.MaxPlayers, "2", QueryFilter.OpOptions.LT),
                new QueryFilter(QueryFilter.FieldOptions.AvailableSlots, "5", QueryFilter.OpOptions.LT)
            },
            Order = new List<QueryOrder>
            {
                new QueryOrder(false, QueryOrder.FieldOptions.Created)
            }
        };
        try
        {
            QueryResponse response = await Lobbies.Instance.QueryLobbiesAsync(optionsQueryLobbie);
            print($"Avaiable lobbies {response.Results.Count}");
            if (response.Results.Count <= 0)
            {
                print("No Lobbie avaiable");
                debugConnectionClient.text = "No Lobbie avaiable";
                //StartCoroutine(DisableText(debugConnectionClient.gameObject));
                return;
            }
            //print("exist lobbie?: " + response.Results[0].Id + response.Results[0].Name);
            Lobby lobby = await Lobbies.Instance.JoinLobbyByIdAsync(response.Results[0].Id);
            myLobby = lobby;
            //print("players: " + myLobby.Players.Count);
            //NetworkManager.Singleton.StartClient();
            //panelPlayStart.SetActive(true);
            //panelClientMode.SetActive(false);
        }
        catch (LobbyServiceException error)
        {
            print("ERROR IN CONNECT LOBBY " + error);
            debugConnectionClient.text = "Error connecting to lobbie";
            //debugConnectionClient.gameObject.SetActive(true);
            //StartCoroutine(DisableText(debugConnectionClient.gameObject));
            throw;
        }
        //UpdateLobby();
    }
    private IEnumerator LobbyHeartbeat()
    {
        while (myLobby != null)
        {
            yield return new WaitForSeconds(hearthTime);
            LobbyService.Instance.SendHeartbeatPingAsync(myLobby.Id);
            print("ping lobby for not die");
        }

        //while (true)
        //{
        //    yield return new WaitForSeconds(time);

        //}
    }

}

