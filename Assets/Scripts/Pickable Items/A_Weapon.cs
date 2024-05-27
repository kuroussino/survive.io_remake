using UnityEngine;
/// <summary>
///<b>Base abstract class</b> for Weapons
/// </summary>
public abstract class A_Weapon : MonoBehaviour, I_Item
{
    [Header("Stats and Variables")]
    [Space(6)]
    [Tooltip("Range in Units/M where the bullets gets destroyed")]
    [SerializeField] private float range;
    [Tooltip("Number of ammos before reloading the weapon")]
    [SerializeField] private float numberAmmoMagazine;
}
