using System;
using UnityEngine;

/// <summary>
/// Class that inherits from <c>A_Support</c>. Serves as armor for Player's armor.
/// </summary>
public sealed class ArmorPack : A_Support
{
    [Tooltip("The amount of damage reduction received by the armor pack.")]
    [Range(0,100)]
    [SerializeField] private float armorReductionPercentage;
    /// <summary>
    /// The damage reduction effect of the armor pack
    /// </summary>
    /// <param name="player"></param>
    public DamageQueryInfo ReduceDamage(DamageQueryInfo queryInfo)
    {
        queryInfo.damageAmount *= (100f - armorReductionPercentage) / 100f;
        return queryInfo;
    }
}
