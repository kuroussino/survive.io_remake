using UnityEngine;

public class Desi_Debug : MonoBehaviour
{
    [SerializeField] A_Weapon weapon;
    [SerializeField] PickableInstance pickablePrefab;
    [SerializeField] LootBox itembox;

    private PickableInstance pick;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Mouse0))
        {
            weapon.Shoot();
            //pick = Instantiate(pickablePrefab, transform.position, Quaternion.identity);
            //pick.SetPrefabToSpawn(weapon);
        }
        if (Input.GetKeyDown(KeyCode.V))
        {
            itembox.TakeDamage();
        }
    }
}
