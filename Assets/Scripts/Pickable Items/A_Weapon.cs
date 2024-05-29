using System.Collections;
using Unity.Netcode;
using UnityEngine;

/// <summary>
///<para>Base abstract class for <b>Weapons</b></para>
///<remarks> P.S. Maybe add an Interface I_Weapon just to give an abstract implementation for the player?</remarks>
/// </summary>
public abstract class A_Weapon : NetworkBehaviour, I_Item
{
    #region Variables and Properties

    [Header("Stats and Variables")]
    [Space(6)]
    [Tooltip("The damage the bullet of the weapon deals.")]
    [SerializeField] protected float damage; // To check with the bullet speed, in order to work well
    [Tooltip("Range in Units/m where the bullets gets destroyed.")]
    [SerializeField] protected float range; // To check with the bullet speed, in order to work well
    [Tooltip("Number of ammos before reloading the weapon.")]
    [SerializeField] protected int numberAmmoMagazine;
    [Tooltip("Cooldown in seconds before the weapon can shoot again after the ammo magazine went empty.")]
    [SerializeField] protected float reloadTime;
    [Tooltip("Cooldown in seconds from one bullet stream to another.")]
    [SerializeField] protected float cooldownShotsTime;
    [Tooltip("The speed which the bullet travels.")]
    [SerializeField] protected float bulletSpeed;
    [Tooltip("If true, this weapon continues to fire when holding the button for firing.")]
    [SerializeField] protected bool holdForAutoFire;
    [Tooltip("The prefab of the bullet shooted by the weapon.")]
    [SerializeField] protected GameObject bulletPrefab;
    [Tooltip("The prefab of the bullet shooted by the weapon.")]
    [SerializeField] protected Transform bulletSpawnPoint;
    [Tooltip("The appearence of the bullet shooted by the weapon.")]
    [SerializeField] protected Sprite bulletSprite;
    [Tooltip("Sound used when shooting.")]
    [SerializeField] protected AudioClip shootSound;
    [Tooltip("Sound used when the weapon reloads.")]
    [SerializeField] protected AudioClip reloadSound;
    [Tooltip("Sound used when the weapon is out of bullets and it's still trying to shoot.")]
    [SerializeField] protected AudioClip outOfBulletsSound;

    /// <summary>
    /// Linked to the canHoldForAutomaticFire variable, goes true when player used the fire button
    /// </summary>
    private bool alreadyShooted = false;

    /// <summary>
    /// Used to check if the weapon can shoot.
    /// </summary>
    private bool canShoot = true;


    private int currentNumberAmmoMagazine;

    #endregion

    #region Mono Methods

    private void Awake()
    {
        currentNumberAmmoMagazine = numberAmmoMagazine;
    }

    /// <summary>
    /// 
    /// </summary>
    private void OnValidate()
    {
        range = Mathf.Clamp(range, 0, float.MaxValue);
        numberAmmoMagazine = Mathf.Clamp(numberAmmoMagazine, 1, int.MaxValue);
        reloadTime = Mathf.Clamp(reloadTime, 0, float.MaxValue);
        cooldownShotsTime = Mathf.Clamp(cooldownShotsTime, 0, float.MaxValue);
        bulletSpeed = Mathf.Clamp(bulletSpeed, 0, float.MaxValue);
    }

    #endregion

    #region Custom Methods


    /// <summary>
    /// Checks if the weapon can shoot, then activates the shoot effect.
    /// <para>~ Public Access Method for Shooting</para>
    /// </summary>
    public void Shoot()
    {
        if (!canShoot || (alreadyShooted && !holdForAutoFire))
        {
            Debug.Log("Can't Shoot!");
            return;
        }
        if (numberAmmoMagazine > 0)
        {
            Debug.Log("Out of bullets!");
            return;
        }
        ShootEffect();
        currentNumberAmmoMagazine--;
        EventsManager.WeaponUpdateBullets?.Invoke(currentNumberAmmoMagazine);
        if (!holdForAutoFire)
            alreadyShooted = true;
    }


    /// <summary>
    /// Reload method. It has a base implementation for reloading with reloadTime, but it's open for any further implementation.
    /// Should be used by other classes to call the reload
    /// </summary>
    public virtual void Reload()
    {
        StartCoroutine(ReloadCoroutine());
    }

    /// <summary>
    /// Reload Coroutine used for waiting until the end of the delay to reload all bullets
    /// </summary>
    /// <returns></returns>
    private IEnumerator ReloadCoroutine()
    {
        StopCoroutine("ShootingCooldownCoroutine");
        yield return ShootingCooldownCoroutine(reloadTime);
        currentNumberAmmoMagazine = numberAmmoMagazine;
        EventsManager.WeaponUpdateBullets?.Invoke(currentNumberAmmoMagazine);
    }

    /// <summary>
    /// Method that can be called when the shooting input is released.
    /// <para>This enables the gun to shoot again if it isnt automatic when holding the fire button.</para>
    /// </summary>
    public void OnShootRelease()
    {
        if (!holdForAutoFire)
            alreadyShooted = false;
    }

    /// <summary>
    ///<para>Describes the main behaviour of the gun when it shoots.</para> 
    ///<para> You can use and implement the ShootingCooldown method for implementing the cooldown between each shots burst.</para>
    /// </summary>
    protected abstract void ShootEffect();



    /// <summary>
    /// Method that uses the ShootingCooldownCoroutine for stopping the weapon from shooting
    /// <para>P.S. Should this be hidden from implementations and only called in the Reload Method?
    /// Probably not because of the Shot Burst Cooldown.</para>
    /// </summary>
    /// <param name="time"></param>
    protected void ShootingCooldown(float time)
    {
        StopCoroutine("StopShootingCooldownCoroutine");
        StartCoroutine(ShootingCooldownCoroutine(time));
    }



    /// <summary>
    /// Cooldown based on <paramref name="time"/> for the weapon before it can shoot again.
    /// It can be used for reloading or after every burst of bullets.
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    private IEnumerator ShootingCooldownCoroutine(float time)
    {
        canShoot = false;
        yield return new WaitForSeconds(time);
        canShoot = true;
    }



    #endregion
}
