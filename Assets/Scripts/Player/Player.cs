using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Rendering.UI;

public class Player : MonoBehaviour, I_Damageable
{
    const bool _permanentlyImmuneToDeathZone = false;
    public bool PermanentlyImmuneToDeathZone => _permanentlyImmuneToDeathZone;
    PlayerMovement movement;
    PlayerInventory inventory;
    PlayerResources resources;

    bool isPressingFireInput = false;

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
    private IEnumerator Start()
    {
        yield return new WaitForSeconds(1);
        NetworkManager.Singleton.StartHost();
        A_Weapon weapon = FindObjectOfType<A_Weapon>();
        TryCollectItem(weapon);
    }
    #endregion
    private void OnPlayerAimInput(Vector2 vector)
    {
        movement.OnAimInput(vector);
    }
    private void OnPlayerFireInput(bool fire)
    {
        if (fire == isPressingFireInput)
            return;

        isPressingFireInput = fire;
        if (fire)
            StartCoroutine(FireInputHold());
    }
    IEnumerator FireInputHold()
    {
        while (isPressingFireInput)
        {
            inventory.OnFireInput();
            yield return new WaitForEndOfFrame();
        }
    }
    private void OnPlayerMovementInput(Vector2 vector)
    {
        movement.OnMovementInput(vector);
    }
    public void TakeDamage(float damageAmount)
    {
        resources?.TakeDirectDamage(damageAmount);
    }
    public void TryCollectItem(I_Item item)
    {
        inventory.TryGetItem(item, out EquipmentData equipmentData);
        if(equipmentData.weapon != null)
        {
            A_Weapon weapon = equipmentData.weapon;
            movement.EquipWeapon(weapon);
        }
    }
}
