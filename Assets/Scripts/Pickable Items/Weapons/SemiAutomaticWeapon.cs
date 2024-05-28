using System.Collections;
using UnityEngine;

/// <summary>
/// <para><b>Weapon</b> that inherits from <c>A_Weapons</c>.</para> 
/// <remarks>It's used for weapons that fires from one to x bullets at a time, with a cooldown on each bullet-stream.</remarks>
/// </summary>
public sealed class SemiAutomaticWeapon : A_Weapon
{
    protected override void ShootEffect()
    {
        throw new System.NotImplementedException();
    }

    // Semi-Automatic Method Coroutine dedicated? (yes)
}
