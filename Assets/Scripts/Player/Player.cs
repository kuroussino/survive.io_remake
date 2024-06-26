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
        if(inventory != null)
        {
            inventory.weaponEquipped += OnWeaponEquipped;
        }
        if (resources != null)
        {
            resources.playerDeath += OnPlayerDeath;
        }
    }

    private void OnDisable()
    {
        EventsManager.playerMovementInput -= OnPlayerMovementInput;
        EventsManager.playerFireInput -= OnPlayerFireInput;
        if (inventory != null)
        {
            inventory.weaponEquipped -= OnWeaponEquipped;
        }
        if (resources != null)
        {
            resources.playerDeath -= OnPlayerDeath;
        }
    }
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsOwner)
        {
            EventsManager.changePlayerCameraTarget?.Invoke(transform);
            EventsManager.PlayerUIInitialize?.Invoke(resources.MaxHealth, 1, null);
        }
        if (IsServer)
        {
            EventsManager.playerJoinedBattle?.Invoke(this);
        }
    }
    #endregion

    #region Input
    private void Update()
    {
        if (!CanAct())
            return;

        movement.OnAimInput();
    }
    private void OnPlayerFireInput(bool fire)
    {
        if (!CanAct())
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
        if (!canMoveRegardless && !CanAct())
            return;

        movement.OnMovementInput(vector);
    }
    bool CanAct()
    {
        if (resources != null && !resources.IsAlive)
            return false;

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

        if (inventory != null)
            info = inventory.AbsorbDamage(info);

        responseInfo.attackAbsorbed = true;
        resources?.TakeDirectDamage(info.damageAmount);
        UpdatePlayerHealthClientRpc(resources.CurrentHealth, resources.MaxHealth);
        return responseInfo;
    }

    [ClientRpc]
    public void UpdatePlayerHealthClientRpc(float currentHealth, float maxHealth)
    {
        if (!IsOwner)
            return;
        
        EventsManager.OnPlayerDamage?.Invoke(currentHealth, maxHealth);
    }
    public bool TryCollectItem(I_Item item)
    {
        return inventory.TryGetItem(item);
    }
    public void Heal(float amount)
    {
        resources?.Heal(amount);
        UpdatePlayerHealthClientRpc(resources.CurrentHealth, resources.MaxHealth);
    }
    private void OnWeaponEquipped(A_Weapon weapon)
    {
        movement.EquipWeapon(weapon);
    }
    private void OnPlayerDeath()
    {
        EventsManager.playerDeath?.Invoke(this);
        OnPlayerDeathClientRpc();
        Destroy(gameObject);
    }
    [ClientRpc]
    void OnPlayerDeathClientRpc()
    {
        if(!IsOwner) return;
        EventsManager.OnPlayerDead?.Invoke();
    }
}
