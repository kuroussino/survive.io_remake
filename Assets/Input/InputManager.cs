using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;

public class InputManager : MonoBehaviour
{
    public void OnMoveInput(CallbackContext context)
    {
        Vector2 direction = context.ReadValue<Vector2>();
    }
}
