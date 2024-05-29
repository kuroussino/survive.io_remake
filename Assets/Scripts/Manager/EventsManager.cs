using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static partial class EventsManager
{
    #region Inputs
    public static Action<Vector2> playerMovementInput;
    public static Action<Vector2> playerAimInput;

    public static Action playerFireInput;
    public static Action playerReloadInput;
    public static Action playerHealInput;
    public static Action cameraSwitchInput;
    #endregion

    public static Action<Transform> changePlayerCameraTarget;
}
