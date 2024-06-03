using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
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
    public void TryGetItem(I_Item item, out EquipmentData equipmentData)
    {
        equipmentData = new EquipmentData();
        A_Weapon weapon = item as A_Weapon;
        if(weapon != null)
        {
            SortWeapon(weapon);
            equipmentData.weapon = weapon;
            return;
        }
    }
    void SortWeapon(A_Weapon weapon)
    {
        this.weapon = weapon;
    }
}

public class EquipmentData
{
    public A_Weapon weapon;
}