using Unity.Netcode;
using UnityEngine;
/// <summary>
/// <para><b>Weapon</b> that inherits from <c>A_Weapons</c>.</para> 
/// <remarks>It's used for weapons that fires from one to x bullets at a time, with a cooldown on each bullet-stream.</remarks>
/// </summary>
public sealed class SemiAutomaticWeapon : A_Weapon
{
    protected override void ShootEffect()
    {
        var bullet = Instantiate(bulletPrefab, bulletSpawnPoint.transform.position, transform.rotation).GetComponent<BulletBehaviour>();
        BulletData bulletData = new BulletData
        {
            damage = damage,
            speed = bulletSpeed,
            range = range,
            sprite = bulletSprite,
            source = owner
        };
        bullet.SetDataBulletFromWeapon(bulletData);
        Debug.Log("Shoot");
        currentNumberAmmoMagazine--;
        EventsManager.OnUpdateBulltes?.Invoke(NumberAmmoMagazine);
        EventsManager.WeaponUpdateBullets?.Invoke(currentNumberAmmoMagazine);
    }

    // Semi-Automatic Method Coroutine dedicated? (yes)
}
