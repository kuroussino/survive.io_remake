using System;
using UnityEngine;

/// <summary>
/// Class that inherits from <c>A_Support</c>. Serves as armor for Player's armor.
/// </summary>
public sealed class ArmorPack : A_Support
{
    /// <summary>
    /// Choose which effect to activate based on <paramref name="isEquippable"/> variable.
    /// </summary>
    /// <param name="player"></param>
    /// <param name="isEquippable"></param>
    protected override void ActivateEffect(Player player)
    {
        if (isEquippable)
            EquipmentEffect(player);
        else
            InstantEffect(player);
    }

    /// <summary>
    /// Instant effect of the HealthPack.
    /// </summary>
    /// <param name="player"></param>
    private void InstantEffect(Player player)
    {
        throw new NotImplementedException();
    }


    /// <summary>
    /// Effect of the HealthPack support item. 
    /// </summary>
    /// <param name="player"></param>
    private void EquipmentEffect(Player player)
    {
        throw new NotImplementedException();
    }
}
