using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Rendering.UI;

public class Player : NetworkBehaviour, I_Damageable, I_DamageOwner
{
    const bool _permanentlyImmuneToDeathZone = false;
    public bool PermanentlyImmuneToDeathZone => _permanentlyImmuneToDeathZone;
    PlayerMovement movement;
    PlayerInventory inventory;
    PlayerResources resources;
    [SerializeField] bool canActRegardless;
    [SerializeField] bool canMoveRegardless;

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
        EventsManager.playerFireInput += OnPlayerFireInput;
        inventory.weaponEquipped += OnWeaponEquipped;
    }

    private void OnDisable()
    {
        EventsManager.playerMovementInput -= OnPlayerMovementInput;
        EventsManager.playerFireInput -= OnPlayerFireInput;
        inventory.weaponEquipped += OnWeaponEquipped;
    }
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsOwner)
            EventsManager.changePlayerCameraTarget?.Invoke(transform);
    }
    #endregion

    #region Input
    private void Update()
    {
        if (!IsControlledPlayer())
            return;

        movement.OnAimInput();
    }
    private void OnPlayerFireInput(bool fire)
    {
        if (!IsControlledPlayer())
            return;

        OnPlayerFireInputServerRpc(fire);
    }

    [ServerRpc]
    private void OnPlayerFireInputServerRpc(bool fire)
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
            if (inventory.IsBareHanded)
                movement.Punch(this);
            else
                inventory.OnFireInput();
            yield return new WaitForEndOfFrame();
        }
    }
    private void OnPlayerMovementInput(Vector2 vector)
    {
        if (!canMoveRegardless && !IsControlledPlayer())
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

    public DamageResponseInfo TakeDamage(DamageQueryInfo info)
    {
        DamageResponseInfo responseInfo = new DamageResponseInfo();
        if((object)info.owner == this)
        {
            responseInfo.attackAbsorbed = false;
            return responseInfo;
        }

        Debug.Log($"{name} took damage!");
        resources?.TakeDirectDamage(info.damageAmount);
        return responseInfo;
    }
    public bool TryCollectItem(I_Item item)
    {
        return inventory.TryGetItem(item);
    }
    public void Heal(float amount)
    {
        resources?.Heal(amount);
    }
    private void OnWeaponEquipped(A_Weapon weapon)
    {
        movement.EquipWeapon(weapon);
    }
}
