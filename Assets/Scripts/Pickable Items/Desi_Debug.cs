using UnityEngine;

public class Desi_Debug : MonoBehaviour
{
    [SerializeField] A_Weapon weapon;
    [SerializeField] LootBox itembox;

    private PickableInstance pick;
    PickableInstance[] pickablePrefab;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Mouse0))
        {
            pickablePrefab = FindObjectsOfType<PickableInstance>();
            if (pickablePrefab != null)
                foreach(var pre in pickablePrefab)
                {
                    pre.ActivateUI();
                }
        }
        if (Input.GetKey(KeyCode.Mouse1))
        {
            if (pickablePrefab != null)
                foreach (var pre in pickablePrefab)
                {
                    pre.DeactivateUI();
                }
        }
        if(itembox != null)
        {
            DamageQueryInfo info = new DamageQueryInfo();
            info.damageAmount = 20;
            itembox.TakeDamage(info);
        }
    }
}
