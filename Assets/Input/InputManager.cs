using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public class InputManager : MonoBehaviour
{
    public void OnMoveInput(CallbackContext context)
    {
        Vector2 direction = context.ReadValue<Vector2>();
        EventsManager.playerMovementInput?.Invoke(direction);
    }
    public void OnAimInput(CallbackContext context)
    {
        Vector2 delta = context.ReadValue<Vector2>();
        EventsManager.playerAimInput?.Invoke(delta);
    }
}
