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

    private Pool<EnemyProjectile> projPool;
    private Pool<EnemyProjectile> projPool2;

    [SerializeField]
    private Animator one;
    [SerializeField]
    private Animator two;
    private void Start()
    {
        GameManager.Instance.PoolingManager.FindPool(projectile, out projPool);
        GameManager.Instance.PoolingManager.FindPool(projectile, out projPool2);
        InvokeRepeating("SpawnProjectile", 0, 1.5f);
    }
    public void SpawnProjectile()
    {
        bool initial;
        EnemyProjectile temp = projPool.GetPooledObj(out initial);
        if (initial)
        {
            temp.NewInstance();
        }
        one.Play("ChargingOne");
        two.Play("ChargingOne");
        temp.Init(Vector2.down, spawnPointOne.position, TargetLayer, 0, 10, 5, 25, transform, transform);
        bool initial2;
        EnemyProjectile temp2 = projPool2.GetPooledObj(out initial2);
        if (initial2)
        {
            temp2.NewInstance();
        }
        temp2.Init(Vector2.down, spawnPointTwo.position, TargetLayer, 0, 10, 5, 25, transform, transform);
        // EnemyProjectile temp = Instantiate(projectile).GetComponent<EnemyProjectile>();
        // temp.NewInstance();
        // temp.OverrideProjectile();
        // temp.Init(Vector2.down, spawnPointOne.position, TargetLayer, 0, 1, 5, 25, transform, transform);
        // EnemyProjectile temp2 = Instantiate(projectile).GetComponent<EnemyProjectile>();
        // temp2.NewInstance();
        // temp2.OverrideProjectile();
        // temp2.Init(Vector2.down, spawnPointTwo.position, TargetLayer, 0, 1, 5, 25, transform, transform);
    }
    // Update is called once per frame
    
}