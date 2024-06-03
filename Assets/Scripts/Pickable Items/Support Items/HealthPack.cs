using System;
using UnityEngine;

/// <summary>
/// Class that inherits from <c>A_Support</c>. Serves as as a heal for Player's health.
/// </summary>
public sealed class HealthPack : A_Support
{
    /// <summary>
    /// Choose which effect to activate based on <paramref name="isEquippable"/> variable.
    /// </summary>
    /// <param name="player"></param>
    /// <param name="isEquippable"></param>
    protected override void ActivateEffect(MonoBehaviour player)
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
    private void InstantEffect(MonoBehaviour player)
    {
        throw new NotImplementedException();
    }


    /// <summary>
    /// Effect of the HealthPack support item. 
    /// </summary>
    /// <param name="player"></param>
    private void EquipmentEffect(MonoBehaviour player)
    {
        throw new NotImplementedException();
    }
}
