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
    [SerializeField] bool testing;

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
        if (!IsControlledPlayer())
            yield break;

        if (testing)
        {
            yield return new WaitForSeconds(1);
            NetworkManager.Singleton.StartHost();
            A_Weapon weapon = FindObjectOfType<A_Weapon>();
            TryCollectItem(weapon);
        }
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
        I_Item item = ItemGetter.Instance.GetItem();
        if (item == null)
            Debug.Log("Nothing");
        else
            Debug.Log($"{item.GetSpriteItem().name} is the image");
    }
    bool IsControlledPlayer()
    {
        bool? isControlled = EventsManager.isOwnerPlayer?.Invoke(this);
        if (isControlled == null)
            return false;

        return isControlled.Value;
    }
    #endregion

    public void TakeDamage(float damageAmount)
    {
        Debug.Log($"{name} took damage!");
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
    public void Heal(float amount)
    {
        resources?.Heal(amount);
    }

}
