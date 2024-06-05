using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] VariableReference<float> _baseMovement;

    [SerializeField] Transform firstHand;
    [SerializeField] Transform secondHand;

    [SerializeField] Transform firstHandMainPosition;
    [SerializeField] Transform secondHandMainPosition;

    [SerializeField] Transform playerSprites;

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
    public void EquipWeapon(A_Weapon weapon)
    {
        if(weapon == null)
        {
            ResetHandsPosition();
            return;
        }

        weapon.gameObject.transform.SetParent(playerSprites);
        weapon.transform.rotation = playerSprites.rotation;
        firstHand.position = firstHandMainPosition.position;
        weapon.transform.position = firstHand.position;
        secondHand.position = secondHandMainPosition.position;
    }
    void ResetHandsPosition()
    {
        firstHand.position = firstHandMainPosition.position;
        secondHand.position = secondHandMainPosition.position;
    }
    private void Update()
    {
        Vector3 direction = lastMovementInput;
        transform.position += direction * _baseMovement.Value * Time.deltaTime;
    }
}
