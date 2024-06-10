using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

public class PlayerSwitcher : MonoBehaviour
{
    [SerializeField] bool debugMode;
    Player[] players;
    int playersIndex;
    Player CurrentPlayer => players[playersIndex];
    private void Awake()
    {
        players = FindObjectsOfType<Player>();
    }
    private void OnEnable()
    {
        EventsManager.isOwnerPlayer += OnIsOwnerPlayer;
        EventsManager.cameraSwitchInput += OnCameraSwitchInput;
    }
    private void OnCameraSwitchInput()
    {
        playersIndex++;
        if(playersIndex >= players.Length)
        {
            playersIndex = 0;
        }
        EventsManager.changePlayerCameraTarget?.Invoke(CurrentPlayer.transform);
    }
    private void Start()
    {
        NetworkManager.Singleton.StartHost();
    }
    private void OnDisable()
    {
        EventsManager.isOwnerPlayer -= OnIsOwnerPlayer;
        EventsManager.cameraSwitchInput -= OnCameraSwitchInput;
    }
    private bool OnIsOwnerPlayer(Player player)
    {
        if (debugMode)
        {
            return player == CurrentPlayer;
        }
        else
        {
            return player.IsOwner;
        }
    }
}
