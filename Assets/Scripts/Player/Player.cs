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

    #region Mono
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
        EventsManager.playerFireInput += OnPlayerFireInput;
    }
    private void OnDisable()
    {
        EventsManager.playerMovementInput -= OnPlayerMovementInput;
        EventsManager.playerAimInput -= OnPlayerAimInput;
        EventsManager.playerFireInput -= OnPlayerFireInput;
    }
    private void Start()
    {
        A_Weapon weapon = FindObjectOfType<A_Weapon>();
        inventory.TryGetItem(weapon);
    }
    #endregion
    private void OnPlayerAimInput(Vector2 vector)
    {
        movement.OnAimInput(vector);
    }
    private void OnPlayerFireInput(bool fire)
    {
        inventory.OnFireInput();
    }
    private void OnPlayerMovementInput(Vector2 vector)
    {
        movement.OnMovementInput(vector);
    }
    public void TakeDamage(float damageAmount)
    {
        resources?.TakeDirectDamage(damageAmount);
    }
}
