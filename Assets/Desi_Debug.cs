using UnityEngine;

public class Desi_Debug : MonoBehaviour
{
    [SerializeField] A_Weapon weapon;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Mouse0))
        {
            weapon.Shoot();
        }
        else if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            weapon.OnShootRelease();
        }
    }
}
