using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialRangeEnemy : MonoBehaviour, IDamageable
{
    // Start is called before the first frame update
    public GameObject projectile;
    public LayerMask TargetLayer;
    public Transform target;
    public ModifyBoundary boundary;

    private Pool<EnemyProjectile> projPool;

    public GameObject debug;

    private bool killable;

    public int Hitpoints { get; set; }

    public Rigidbody2D rb { get; set; }

    void Start()
    {
        target = GameManager.Instance.PlayerTransform;
        GameManager.Instance.PoolingManager.FindPool(projectile, out projPool);
        InvokeRepeating("ShootAtTarget", 0, 2f);
    }

    private void Update()
    {
        if (GameManager.Instance.PCM.system.isCountered)
        {
            if (killable) return;
            killable = true;
        }
    }

    public void ShootAtTarget()
    {
        bool initial;
        EnemyProjectile temp = projPool.GetPooledObj(out initial);
        if (initial)
        {
            temp.NewInstance();
        }

        // EnemyProjectile temp = Instantiate(projectile).GetComponent<EnemyProjectile>();
        // temp.NewInstance();
        // temp.OverrideProjectile();
        temp.Init((target.position - transform.position).normalized, transform.position, TargetLayer, 0, 10, 10, 6,
            transform, transform);

    }

    public void SetHitPoints()
    {

    }

    public void TakeDamage(int amount, int staggerPoints, ElementType type, int tier,
        ElementType typeTwo = ElementType.noElement)
    {
        // if (!killable) return;
        Destroy(gameObject);
    }

    public void TakeDamage(int amount, int staggerPoints, ElementType type, ElementType typeTwo = ElementType.noElement)
    {
        // if (!killable) return;
        Destroy(gameObject);
        boundary.DisableBoundary();
    }

    public void AddForce(Vector2 force)
    {

    }

    public void ModifySpeed(float percentage)
    {

    }

    public void ResetSpeed()
    {

    }

    // private void OnCollisionEnter2D(Collision2D other)
    // {
    //     Debug.Log(other.gameObject.name);
    //     debug = other.gameObject;
    //     if (!other.gameObject.CompareTag("Player")) return;
    //     if (!killable) return;
    //     Destroy(gameObject);
    // }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Debug.Log(other.gameObject.name);
        debug = other.gameObject;
        AbilityBase foundAbility;
        // Debug.Log(UtilityFunction.FindComponent(other.gameObject.transform, out foundAbility));
        // if (!other.gameObject.CompareTag("Player")) return;
        if (!UtilityFunction.FindComponent(other.gameObject.transform, out foundAbility)) return;
        if (!killable) return;
        Destroy(gameObject);
    }
}