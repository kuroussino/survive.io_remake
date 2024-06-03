using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.LowLevel;
using static UnityEngine.GraphicsBuffer;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] VariableReference<float> _baseMovement;
    Vector2 lastMovementInput;
    public void OnMovementInput(Vector2 direction)
    {
        lastMovementInput = direction;
    }
    public void OnAimInput(Vector2 delta)
    {
        Vector2Control mousePositionControl = Mouse.current.position;
        Vector2 mousePosition = mousePositionControl.value;
        Vector2 myPositionOnScreen = Camera.main.WorldToScreenPoint(transform.position);
        Vector2 direction = mousePosition - myPositionOnScreen;
        direction.Normalize();

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
    private void Update()
    {
        Vector3 direction = lastMovementInput;
        transform.position += direction * _baseMovement.Value * Time.deltaTime;
    }
}
