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
    public void OnFireInput(CallbackContext context)
    {
        EventsManager.playerFireInput?.Invoke(TapAndReleaseBool(context));
    }
    public void OnCameraSwitchInput(CallbackContext context)
    {
        if (!context.performed)
            return;

        EventsManager.cameraSwitchInput?.Invoke();
    }
    public void OnCameraPanInput(CallbackContext context)
    {
        EventsManager.cameraPanInput?.Invoke(TapAndReleaseBool(context));
    }

    //Returns true if the context is performed. Returns false if the context is canceled
    bool TapAndReleaseBool(CallbackContext context)
    {
        if (context.performed)
            return true;

        if (context.canceled)
            return false;

        return false;
    }
}
