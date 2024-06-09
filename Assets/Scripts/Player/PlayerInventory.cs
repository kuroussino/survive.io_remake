using System;
using Unity.Netcode;

public class PlayerInventory : NetworkBehaviour
{
    public Action<A_Weapon> weaponEquipped;
    public bool IsBareHanded => heldWeapon == null;
    //heal
    //armor
    //TryAbsorbDamage
    A_Weapon heldWeapon;

    public void OnFireInput()
    {
        heldWeapon?.Shoot();
    }
    public void OnReloadInput()
    {
        heldWeapon?.Reload();
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
        int weaponID = ItemID.Instance.GetWeaponID(weapon);
        CreateWeaponClientRpc(weaponID);
    }

    [ClientRpc]
    void CreateWeaponClientRpc(int weaponID)
    {
        A_Weapon weaponPrefab = ItemID.Instance.GetWeaponItem<A_Weapon> (weaponID);
        A_Weapon newWeapon = Instantiate(weaponPrefab);
        heldWeapon = newWeapon;
        weaponEquipped?.Invoke(heldWeapon);
    }
}