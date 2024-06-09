using Unity.Netcode;
using UnityEngine;

/// <summary>
/// Behaviour of the Box that drops items. Can be destroyed.
/// </summary>
public class LootBox : NetworkBehaviour, I_Damageable
{
    [SerializeField] private PickableInstance pickable;   
    [SerializeField] private int maxRangeItems;

    public bool PermanentlyImmuneToDeathZone => true;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
    }
    public DamageResponseInfo TakeDamage(DamageQueryInfo info)
    {
        int maxItems = Random.Range(1, maxRangeItems + 1);
        for (int i = 0; i < maxItems; i++)
        {
            var item = ItemGetter.Instance.GetRandomItem();
            var weapon = item as A_Weapon;
            var support = item as A_Support;
            if (weapon == null && support == null)
                continue;
            PickableInstance pick = Instantiate(pickable, transform.position, transform.rotation);
            pick.SetPrefabToSpawn(weapon ? ((A_Weapon)item).gameObject : ((A_Support)item).gameObject, item.GetSpriteItem());
        }
        Destroy(gameObject);
        return new DamageResponseInfo();
    }
}
