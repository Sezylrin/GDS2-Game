using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileShooter : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject projectile;
    public Transform spawnPointOne;
    public Transform spawnPointTwo;
    public LayerMask TargetLayer;
    private void Start()
    {
        InvokeRepeating("SpawnProjectile", 0, 1.5f);
    }
    public void SpawnProjectile()
    {
        EnemyProjectile temp = Instantiate(projectile).GetComponent<EnemyProjectile>();
        temp.NewInstance();
        temp.OverrideProjectile();
        temp.Init(Vector2.down, spawnPointOne.position, TargetLayer, 0, 1, 5, 25, transform, transform);
        EnemyProjectile temp2 = Instantiate(projectile).GetComponent<EnemyProjectile>();
        temp2.NewInstance();
        temp2.OverrideProjectile();
        temp2.Init(Vector2.down, spawnPointTwo.position, TargetLayer, 0, 1, 5, 25, transform, transform);
    }
    // Update is called once per frame
    
}
