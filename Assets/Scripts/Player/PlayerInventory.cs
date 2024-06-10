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
    ArmorPack armorPack;
    HealthPack healthPack;

    public void OnFireInput()
    {
        heldWeapon?.Shoot();
    }
    public void OnHealInput(Player player)
    {
        if(healthPack != null)
        {
            healthPack.HealEffect(player);
            healthPack = null;
        }
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

        ArmorPack armorPack = item as ArmorPack;
        if (armorPack != null)
        {
            if (this.armorPack == null)
            {
                this.armorPack = armorPack;
                EventsManager.OnGetArmor?.Invoke();
                return true;
            }
            return false;
        }

        HealthPack healthPack = item as HealthPack;
        if (healthPack != null)
        {
            if (this.healthPack == null)
            {
                this.healthPack = healthPack;
                EventsManager.OnGrabHealthPack?.Invoke();
                return true;
            }
            return false;
        }

        return false;
    }
    void SortWeapon(A_Weapon weapon)
    {
        int weaponID = ItemID.Instance.GetWeaponID(weapon);
        CreateWeaponClientRpc(weaponID);
        EventsManager.OnNewWeapon?.Invoke(heldWeapon, heldWeapon.NumberAmmoMagazine);
    }

    [ClientRpc]
    void CreateWeaponClientRpc(int weaponID)
    {
        A_Weapon weaponPrefab = ItemID.Instance.GetWeaponItem<A_Weapon> (weaponID);
        A_Weapon newWeapon = Instantiate(weaponPrefab);
        heldWeapon = newWeapon;
        weaponEquipped?.Invoke(heldWeapon);
    }

    public DamageQueryInfo AbsorbDamage(DamageQueryInfo info)
    {
        if(armorPack != null)
            info = armorPack.ReduceDamage(info);

        return info;
    }
}