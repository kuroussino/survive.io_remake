using UnityEngine;

public class Desi_Debug : MonoBehaviour
{
    [SerializeField] A_Weapon weapon;
    [SerializeField] PickableInstance pickablePrefab;

    private PickableInstance pick;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Mouse0))
        {
            pick = Instantiate(pickablePrefab, transform.position, Quaternion.identity);
            pick.SetPrefabToSpawn(weapon);
        }
        else if (Input.GetKeyUp(KeyCode.Mouse0))
        {
             
        }
    }
}
