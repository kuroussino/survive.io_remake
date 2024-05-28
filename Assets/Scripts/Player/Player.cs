using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    PlayerMovement movement;

    private void Awake()
    {
        movement = GetComponent<PlayerMovement>();
    }
    private void OnEnable()
    {
        EventsManager.playerMovementInput += OnPlayerMovementInput;
        EventsManager.playerAimInput += OnPlayerAimInput;
    }
    private void OnDisable()
    {
        EventsManager.playerMovementInput -= OnPlayerMovementInput;
        EventsManager.playerAimInput -= OnPlayerAimInput;
    }
    private void OnPlayerAimInput(Vector2 vector)
    {
        movement.AimInput(vector);
    }
    private void OnPlayerMovementInput(Vector2 vector)
    {
        movement.MovementInput(vector);
    }
}
