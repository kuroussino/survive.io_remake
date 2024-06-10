using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static partial class EventsManager
{
    #region Inputs
    public static Action<Vector2> playerMovementInput;
    public static Action<Vector2> playerAimInput;

    public static Action<bool> playerFireInput;
    public static Action playerReloadInput;
    public static Action playerHealInput;
    public static Action cameraSwitchInput;
    public static Action<bool> cameraPanInput;
    #endregion

    public static Action<Transform> changePlayerCameraTarget;

    public static Func<Player, bool> isOwnerPlayer;

    public static Func<Player, Transform> getSpawnPosition;
    public static Func<I_DamageSource, I_Damageable, bool> sourceHasAlreadyHitDamageable;
    public static Action<I_DamageSource> resetAlreadyHitTargets;

    public static Action<Player> playerJoinedBattle;
    public static Action<Player> playerDeath;
}
