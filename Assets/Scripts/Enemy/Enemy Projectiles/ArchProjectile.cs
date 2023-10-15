using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArchProjectile : EnemyProjectile
{
    protected Vector2 targetPoint;
    protected Vector2 archDir;
    protected float archDuration;
    protected float initialV;
    protected float decay;
    protected Vector2 knockbackdir;
    // Start is called before the first frame update
    public virtual void InitArch(Vector2 archDir, Vector2 targetPoint, bool fixedArrival)
    {
        this.archDir = archDir;
        this.targetPoint = targetPoint;
        if (fixedArrival)
        {
            archDuration = Speed * 0.5f;
            Speed = ((targetPoint - (Vector2)transform.position).magnitude / Speed);
        }
        else
            archDuration = ((targetPoint - (Vector2)transform.position).magnitude / Speed) * 0.5f;
        initialV = (2 * archDir.magnitude) / archDuration;
        decay = initialV / archDuration;
    }
    protected override void MoveProjectileByDir()
    {
        initialV -= decay * Time.deltaTime * 0.5f;
        Vector2 newDir = dir.normalized * Speed * Time.deltaTime;
        Vector2 modifiedArch = archDir.normalized * initialV * Time.deltaTime;
        newDir += modifiedArch;
        initialV -= decay * Time.deltaTime * 0.5f;
        knockbackdir = newDir;
        gameObject.transform.Translate(newDir);
    }
    public override void Init(Vector2 dir, Vector3 spawnPos, LayerMask Target, int damage, float duration, float speed, float knockbackForce, Transform shooter)
    {
        this.dir = dir;
        transform.position = spawnPos;
        SpriteObj.rotation = UtilityFunction.LookAt2D(Vector3.zero, dir);
        if (col2d)
        {
            col2d.includeLayers = terrainMask + Target;
            rb.includeLayers = terrainMask + Target;
            col2d.excludeLayers = ~col2d.includeLayers;
            rb.excludeLayers = ~rb.includeLayers;
        }
        this.damage = damage;
        this.knockbackForce = knockbackForce;
        Duration = duration;
        Speed = speed;
        this.shooter = shooter;
        StartLifetime();
    }

    protected override void DoDamage(IDamageable target)
    {
        target.TakeDamage(damage, 0, ElementType.noElement);
        target.AddForce(knockbackdir.normalized * knockbackForce);
    }
}
