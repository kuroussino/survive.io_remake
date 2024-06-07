using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerInventory : NetworkBehaviour
{
    public Action<A_Weapon> weaponEquipped;
    //heal
    //armor
    //TryAbsorbDamage
    A_Weapon weapon;

    public void OnFireInput()
    {
        weapon?.Shoot();
    }
    public void OnReloadInput()
    {
        weapon?.Reload();
    }
    public bool TryGetItem(I_Item item)
    {
        A_Weapon weapon = item as A_Weapon;
        if(weapon != null)
        {
            SortWeapon(weapon);
            return true;
        }
        return false;
    }
    void SortWeapon(A_Weapon weapon)
    {
        this.weapon = weapon;
        A_Weapon newWeapon = Instantiate(weapon);
        var instanceNetworkObject = newWeapon.GetComponent<NetworkObject>();
        instanceNetworkObject.Spawn();
    }
}