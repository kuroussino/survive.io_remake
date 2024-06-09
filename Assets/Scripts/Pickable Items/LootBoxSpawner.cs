using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

/// <summary>
/// Enter your class explanation here
/// </summary>
public class LootBoxSpawner : NetworkBehaviour
{

    #region Variables & Properties

    [SerializeField] private Transform[] lootBoxSpawnPositions;
    [SerializeField] private LootBox lootBoxPrefab;

    #endregion

    #region Mono Methods
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (!IsServer)
            return;
        SpawnLootBoxes();
    }
    #endregion

    #region Custom Methods

    private void SpawnLootBoxes()
    {
        for(int i = 0; i < lootBoxSpawnPositions.Length; i++)
        {
            NetworkObject lootBox = Instantiate(lootBoxPrefab, lootBoxSpawnPositions[i].position,Quaternion.identity).GetComponent<NetworkObject>();
            lootBox.Spawn();
        }
    }
    #endregion

}
