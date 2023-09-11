using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnPoint : MonoBehaviour
{
    public bool canSpawn { get; private set; }
    private float noSpawnRadius;

    private void Update()
    {
        ValidateCanSpawn();
    }

    private void ValidateCanSpawn()
    {
        canSpawn = true;
        // canSpawn = Vector2.Distance(Player.Instance.transform.position, gameObject.transform.position) > noSpawnRadius;
    }
}