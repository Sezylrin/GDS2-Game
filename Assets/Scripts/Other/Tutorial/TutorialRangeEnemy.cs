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

    public int Hitpoints { get; set; }

    public Rigidbody2D rb { get; set; }

    void Start()
    {
        target = GameManager.Instance.PlayerTransform;
        InvokeRepeating("ShootAtTarget", 0, 2f);
    }

    public void ShootAtTarget()
    {
        EnemyProjectile temp = Instantiate(projectile).GetComponent<EnemyProjectile>();
        temp.NewInstance();
        temp.OverrideProjectile();
        temp.Init((target.position - transform.position).normalized, transform.position, TargetLayer, 0, 1, 5, 25, transform, transform);

    }

    public void SetHitPoints()
    {

    }

    public void TakeDamage(float amount, int staggerPoints, ElementType type, int tier, ElementType typeTwo = ElementType.noElement)
    {
        Destroy(gameObject);
    }

    public void TakeDamage(float amount, int staggerPoints, ElementType type, ElementType typeTwo = ElementType.noElement)
    {
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
}
