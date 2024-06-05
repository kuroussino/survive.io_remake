using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSwitcher : MonoBehaviour
{
    [SerializeField] bool debugMode;
    Player[] players;
    int playersIndex;
    Player CurrentPlayer => players[playersIndex];
    private void Start()
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
    private void OnDisable()
    {
        EventsManager.isOwnerPlayer -= OnIsOwnerPlayer;
        EventsManager.cameraSwitchInput -= OnCameraSwitchInput;
    }
    private void OnPlayerSwitchCameraTargetInput()
    {
        OnCameraSwitchInput();
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
