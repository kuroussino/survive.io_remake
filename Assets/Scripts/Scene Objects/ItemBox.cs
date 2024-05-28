using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

/// <summary>
/// Behaviour of the Box that drops items. Can be destroyed.
/// </summary>
public class ItemBox : NetworkBehaviour
{
    [Tooltip("Prefab that spawns whenever the item is destroyed")]
    [SerializeField] private I_Item droppedItemPrefab;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (droppedItemPrefab == null)
            print("I don't have an item! I should get a random one from a generator/scriptable!");
        // Maybe use a item getter or a random from a scriptable
    }

    public void TakeDamage()
    {
        //Drop Item, Instantiate Item on Network and Despawn
    }
}
