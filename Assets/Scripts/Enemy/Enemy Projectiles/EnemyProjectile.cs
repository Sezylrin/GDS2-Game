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
    protected float knockbackForce;
    protected ElementType element;
    protected Enemy owner;
    protected Transform shooter;
    public virtual void NewInstance()
    {
        ProjectileTimers = GameManager.Instance.TimerManager.GenerateTimers(typeof(ProjectileTimer), gameObject);
        ProjectileTimers.times[(int)ProjectileTimer.lifetimeTimer].OnTimeIsZero += PoolSelf;
    }

    public virtual void Init(Vector2 dir, Vector3 spawnPos, LayerMask Target, int damage, float knockbackForce, ElementType element, Enemy owner, Transform shooter = null)
    {
        StartLifetime();
        this.dir = dir;
        transform.position = spawnPos;
        SpriteObj.rotation = UtilityFunction.LookAt2D(Vector3.zero, dir);
        col2d.includeLayers = terrainMask + Target;
        rb.includeLayers = terrainMask + Target;
        col2d.excludeLayers = ~col2d.includeLayers;
        rb.excludeLayers = ~rb.includeLayers;
        this.damage = damage;
        this.knockbackForce = knockbackForce;
        this.element = element;
        this.owner = owner;
        if (shooter == null)
            this.shooter = owner.transform;
        else
            this.shooter = shooter;
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
        IDamageable foundTarget;
        if (UtilityFunction.FindComponent(collision.transform, out foundTarget))
        {
            if (foundTarget is PlayerSystem)
            {
                PlayerSystem temp = foundTarget as PlayerSystem;
                if (temp.GetState() == playerState.perfectDodge)
                {
                    temp.InstantRegenPoint();
                    temp.CounterSuccesful(owner, this);
                    gameObject.SetActive(false);
                }
                else
                {
                    DoDamage(foundTarget);
                    PoolSelf();
                }
            }
            else
            {
                DoDamage(foundTarget);
                PoolSelf();
            }
                
        }
    }

    private void DoDamage(IDamageable target)
    {
        target.TakeDamage(damage, 0, element);
        target.AddForce(dir.normalized * knockbackForce);
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
        col2d.excludeLayers = 0;
        rb.excludeLayers = 0;
        if(!IsPooled)
            Pool.PoolObj(this);
    }

    public void CounterProjectile(Enemy target, Vector3 spawnPos, LayerMask enemy, Transform newShooter)
    {
        gameObject.SetActive(true);
        Vector2 newDir = target.transform.position - spawnPos;
        Init(newDir,spawnPos, enemy, damage, knockbackForce, element, owner, newShooter);
    }
    #endregion
}
