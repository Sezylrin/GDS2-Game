using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour, IPoolable<EnemyProjectile>
{
    protected enum ProjectileTimer
    {
        lifetimeTimer
    }

    [field: SerializeField] protected float Speed { get; set; } = 1;
    [field: SerializeField] protected float Duration { get; set; } = 3;
    [field: SerializeField] protected Timer ProjectileTimers { get; private set; }
    [field: SerializeField] protected Transform SpriteObj { get; set; }
    [field: SerializeField] protected Collider2D col2d { get; set; }
    [field: SerializeField] protected Rigidbody2D rb { get; set; }
    [field: SerializeField] protected LayerMask terrainMask { get; set; }
    [field: SerializeField] protected bool poolOnContact { get; private set; } = true;
    protected Vector2 dir;
    protected int damage;
    protected float knockbackForce;
    protected Transform shooter;
    protected Transform target;
    [SerializeField]
    protected bool overrideProjectile = false;
    public virtual void NewInstance()
    {
        ProjectileTimers = GameManager.Instance.TimerManager.GenerateTimers(typeof(ProjectileTimer), gameObject);
        ProjectileTimers.times[(int)ProjectileTimer.lifetimeTimer].OnTimeIsZero += PoolSelf;
    }

    public virtual void Init(Vector2 dir, Vector3 spawnPos, LayerMask Target, int damage, float duration, float speed, float knockbackForce, Transform shooter)
    {
        this.dir = dir;
        transform.position = spawnPos;
        SpriteObj.rotation = UtilityFunction.LookAt2D(Vector3.zero, dir);
        col2d.includeLayers = terrainMask + Target;
        rb.includeLayers = terrainMask + Target;
        col2d.excludeLayers = ~col2d.includeLayers;
        rb.excludeLayers = ~rb.includeLayers;
        this.damage = damage;
        this.knockbackForce = knockbackForce;
        Duration = duration;
        Speed = speed;
        this.shooter = shooter;
        StartLifetime();
    }

    public void Init(Vector2 dir, Vector3 spawnPos, LayerMask Target, int damage, float duration, float speed, float knockbackForce, Transform target, Transform shooter)
    {
        this.target = target;
        Init(dir, spawnPos, Target, damage, duration, speed, knockbackForce, shooter);
    }

    public void OverrideProjectile()
    {
        overrideProjectile = true;
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
        if (collision.transform == shooter)
            return;
        PlayerSystem foundTarget;
        if (UtilityFunction.FindComponent(collision.transform, out foundTarget))
        {
            if (foundTarget.GetState() == playerState.perfectDodge)
            {
                foundTarget.Counter();
            }
            else
            {
                DoDamage(foundTarget);
            }
        }
        if (poolOnContact)
            PoolSelf();
    }

    protected virtual void DoDamage(IDamageable target)
    {
        target.TakeDamage(damage, 0, ElementType.noElement);
        target.AddForce(dir.normalized * knockbackForce);
    }
    #region Pooling
    public void PoolSelf(object sender, EventArgs e)
    {
        PoolSelf();
    }
    public Pool<EnemyProjectile> Pool { get; set; }
    public bool IsPooled { get; set; }

    public virtual void PoolSelf()
    {
        if (col2d)
        {
            col2d.includeLayers = 0;
            rb.includeLayers = 0;
            col2d.excludeLayers = 0;
            rb.excludeLayers = 0;
        }
        if (!IsPooled)
            Pool.PoolObj(this);
    }
    #endregion
}
