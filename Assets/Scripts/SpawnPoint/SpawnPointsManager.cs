using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPointsManager : MonoBehaviour
{
    [SerializeField] Transform[] spawnPositions;
    Dictionary<Transform, Player> spawnedPlayers;

    private void OnEnable()
    {
        EventsManager.getSpawnPosition += OnGetSpawnPosition;
    }
    private void OnDisable()
    {
        EventsManager.getSpawnPosition -= OnGetSpawnPosition;
    }

    private Transform OnGetSpawnPosition(Player player)
    {
        List<int> indexes = new List<int>();
        for (int i = 0; i < spawnPositions.Length; i++)
        {
            indexes.Add(i);
        }

        while(indexes.Count > 0)
        {
            int randomIndex = UnityEngine.Random.Range(0, indexes.Count);
            int index = indexes[randomIndex];
            Transform position = spawnPositions[index];
            if(!spawnedPlayers.TryGetValue(position, out Player dude))
            {
                spawnedPlayers[position] = player;
                return position;
            }
            else
            {
                if (player == null)
                {
                    spawnedPlayers[position] = player;
                    return position;
                }
            }
            indexes.Remove(index);
        }

        return null;
    }
}
