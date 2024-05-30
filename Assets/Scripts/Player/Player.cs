using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.UI;

public class Player : MonoBehaviour, I_Damageable
{
    const bool _permanentlyImmuneToDeathZone = false;
    public bool PermanentlyImmuneToDeathZone => _permanentlyImmuneToDeathZone;
    PlayerMovement movement;
    PlayerInventory inventory;
    PlayerResources resources;

    private void Awake()
    {
        movement = GetComponent<PlayerMovement>();
        resources = GetComponent<PlayerResources>();
        inventory = GetComponent<PlayerInventory>();
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

    public void TakeDamage(float damageAmount)
    {
        resources?.TakeDirectDamage(damageAmount);
    }
}
