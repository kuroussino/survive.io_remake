using Unity.Netcode;
using UnityEngine;

/// <summary>
/// Behaviour of the Box that drops items. Can be destroyed.
/// </summary>
public class LootBox : NetworkBehaviour, I_Damageable
{
    [SerializeField] PickableInstance pickable;   
    [SerializeField] private int maxRangeItems;

    public bool PermanentlyImmuneToDeathZone => true;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
    }

    public void TakeDamage(float damageAmount)
    {
        int maxItems = Random.Range(1, maxRangeItems + 1);
        for (int i = 0; i < maxItems; i++)
        {
            PickableInstance pick = Instantiate(pickable, transform.position, transform.rotation);
            var wep = ItemGetter.Instance.GetRandomItem();
            if(wep == null)
            {
                Debug.Log("No item");
                Destroy(gameObject);
                return;
            }
            pick.SetPrefabToSpawn(wep, wep.GetComponent<I_Item>().GetSpriteItem());
        }
        Destroy(gameObject);
    }
}
