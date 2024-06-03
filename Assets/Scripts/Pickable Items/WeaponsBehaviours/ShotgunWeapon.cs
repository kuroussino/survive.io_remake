using UnityEngine;


/// <summary>
/// <para><b>Weapon</b> that inherits from <c>A_Weapons</c>.</para> 
/// <remarks>It's used for weapons that fires more than one shot in a cone radius.</remarks>
/// </summary>

public sealed class ShotgunWeapon : A_Weapon
{
    [Space(6)]
    [Header("Shotgun Variables")]
    [Tooltip("The number of bullets the shotgun uses everytime it shoots")]
    [SerializeField] private int numberOfBulletsPerShoot;
    [Tooltip("A random angle number that makes the bullets spread randomly under a certain range")]
    [SerializeField] private float randomAngleBullets;
    protected override void ShootEffect()
    {
        for (float i = 0; i <= numberOfBulletsPerShoot; i++) {
            Quaternion randomnessVector = Quaternion.Euler(0, 0, Random.Range(-randomAngleBullets, randomAngleBullets));
            var bullet = Instantiate(bulletPrefab, bulletSpawnPoint.transform.position, transform.localRotation * randomnessVector).GetComponent<BulletBehaviour>();
            bullet.SetDataBulletFromWeapon(damage, bulletSpeed, range, bulletSprite);
            Debug.Log("Shoot");
        }
        ShootingCooldown(cooldownShotsTime);
        currentNumberAmmoMagazine -= numberOfBulletsPerShoot;
        EventsManager.WeaponUpdateBullets?.Invoke(currentNumberAmmoMagazine);
    }
}
