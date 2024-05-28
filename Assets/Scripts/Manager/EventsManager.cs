using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static partial class EventsManager
{
    public static Action<Vector2> playerMovementInput;
    public static Action<Vector2> playerAimInput;

    public static Action playerFireInput;
    public static Action playerReloadInput;
    public static Action playerHealInput;
}
