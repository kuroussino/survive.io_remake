using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Rendering.UI;

public class Player : NetworkBehaviour, I_Damageable
{
    const bool _permanentlyImmuneToDeathZone = false;
    public bool PermanentlyImmuneToDeathZone => _permanentlyImmuneToDeathZone;
    PlayerMovement movement;
    PlayerInventory inventory;
    PlayerResources resources;
    [SerializeField] bool canActRegardless;

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
        inventory.weaponEquipped += OnWeaponEquipped;
    }

    private void OnWeaponEquipped()
    {
        throw new NotImplementedException();
    }

    private void OnDisable()
    {
        EventsManager.playerMovementInput -= OnPlayerMovementInput;
        EventsManager.playerAimInput -= OnPlayerAimInput;
        EventsManager.playerFireInput -= OnPlayerFireInput;
        inventory.weaponEquipped += OnWeaponEquipped;
    }
    #endregion

    #region Input
    private void OnPlayerAimInput(Vector2 vector)
    {
        if (!IsControlledPlayer())
            return;

        movement.OnAimInput(vector);
    }
    private void OnPlayerFireInput(bool fire)
    {
        if (!IsControlledPlayer())
            return;

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
        if (!IsControlledPlayer())
            return;

        movement.OnMovementInput(vector);
    }
    bool IsControlledPlayer()
    {
        if (canActRegardless)
            return true;

        bool? isControlled = EventsManager.isOwnerPlayer?.Invoke(this);
        if (isControlled == null)
            return IsOwner;

        return isControlled.Value;
    }
    #endregion

    public void TakeDamage(float damageAmount)
    {
        Debug.Log($"{name} took damage!");
        resources?.TakeDirectDamage(damageAmount);
    }
    public bool TryCollectItem(I_Item item)
    {
        inventory.TryGetItem(item, out EquipmentData equipmentData);
        if(equipmentData.weapon != null)
        {
            A_Weapon weapon = equipmentData.weapon;
            var instanceNetworkObject = weapon.GetComponent<NetworkObject>();
            instanceNetworkObject.Spawn();
            movement.EquipWeapon(weapon);
            return true;
        }

        return false;
    }
    public void Heal(float amount)
    {
        resources?.Heal(amount);
    }

}
