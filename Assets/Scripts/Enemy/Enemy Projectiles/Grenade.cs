using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : ArchProjectile
{
    [SerializeField]
    private float explosionDelay;
    [SerializeField]
    private float explosionDuration;

    public override void InitArch(Vector2 archDir, Vector2 targetPoint, bool fixedArrival)
    {
        base.InitArch(archDir, targetPoint, fixedArrival);

        Invoke("SpawnGrenade", archDuration * 2);
        Duration = archDuration * 2 + explosionDelay + explosionDuration;
        StartLifetime();
    }

    public void SpawnGrenade()
    {
        Speed = 0;
        initialV = 0;
        decay = 0;
        Invoke("ExplodeGrenade", explosionDelay);
    }

    public void ExplodeGrenade()
    {
        col2d.enabled = true;
        col2d.offset = col2d.offset + Vector2.right * 0.05f;
    }

    public override void PoolSelf()
    {
        col2d.enabled = false;
        base.PoolSelf();
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform == shooter)
            return;
        PlayerSystem foundTarget;
        if (UtilityFunction.FindComponent(collision.transform, out foundTarget))
        {
            if (foundTarget.GetState() == playerState.perfectDodge)
            {
                foundTarget.InstantRegenPoint();
                foundTarget.Counter();
            }
            else
            {
                foundTarget.TakeDamage(damage, 0, ElementType.noElement);
                foundTarget.AddForce((collision.transform.position - transform.position).normalized * knockbackForce);

            }
        }
        if (poolOnContact)
            PoolSelf();
    }
}
