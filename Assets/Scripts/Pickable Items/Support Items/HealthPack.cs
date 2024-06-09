using System;
using UnityEngine;

/// <summary>
/// Class that inherits from <c>A_Support</c>. Serves as as a heal for Player's health.
/// </summary>
public sealed class HealthPack : A_Support
{
    [Tooltip("The amount of health the health pack heals.")]
    [SerializeField] private float amountHealthHealed;

    /// <summary>
    /// Instant effect of the HealthPack.
    /// </summary>
    /// <param name="player"></param>
    public void HealEffect(Player player)
    {
        player.Heal(amountHealthHealed);
    }
}
