using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyProjectile : MonoBehaviour, IPoolable<EnemyProjectile>
{
    protected enum ProjectileTimer
    {
        lifetimeTimer
    }

    [field: SerializeField] protected float Speed { get; set; } = 1;
    [field: SerializeField] protected float Duration { get; set; } = 3;
    [field: SerializeField] protected Timer ProjectileTimers { get; private set; }
    [field: SerializeField] protected Transform SpriteObj { get; set; }
    [field: SerializeField] protected BoxCollider2D col2d { get; set; }
    [field: SerializeField] protected Rigidbody2D rb { get; set; }
    [field: SerializeField] protected LayerMask terrainMask { get; set; }
    protected Vector2 dir;
    protected int damage;
    protected ElementType element;
    protected Transform owner;
    public virtual void NewInstance()
    {
        ProjectileTimers = GameManager.Instance.TimerManager.GenerateTimers(typeof(ProjectileTimer), gameObject);
        ProjectileTimers.times[(int)ProjectileTimer.lifetimeTimer].OnTimeIsZero += PoolSelf;
    }

    public virtual void Init(Vector2 dir, Vector3 spawnPos, LayerMask Target, int damage, ElementType element, Transform owner)
    {
        StartLifetime();
        this.dir = dir;
        transform.position = spawnPos;
        SpriteObj.rotation = UtilityFunction.LookAt2D(Vector3.zero, dir);
        col2d.includeLayers = terrainMask + Target;
        rb.includeLayers = terrainMask + Target;
        this.damage = damage;
        this.element = element;
        this.owner = owner;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        MoveProjectileByDir();
    }
    protected void MoveProjectile(Vector2 dir)
    {
        gameObject.transform.Translate(dir.normalized * Speed * Time.deltaTime);
    }

    protected virtual void MoveProjectileByDir()
    {
        MoveProjectile(dir);
    }
    protected virtual void StartLifetime()
    {
        ProjectileTimers.SetTime((int)ProjectileTimer.lifetimeTimer, Duration);
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform == owner)
            return;
        IDamageable foundEnemy;
        if (UtilityFunction.FindComponent(collision.transform, out foundEnemy))
        {
            foundEnemy.TakeDamage(damage, 0, element);
        }
        PoolSelf();
    }

    #region Pooling
    public Pool<EnemyProjectile> Pool { get; set; }
    public bool IsPooled { get; set; }
    public void PoolSelf(object sender, EventArgs e)
    {
        PoolSelf();
    }
    public void PoolSelf()
    {
        col2d.includeLayers = 0;
        rb.includeLayers = 0;
        if(!IsPooled)
            Pool.PoolObj(this);
    }
    #endregion
}
