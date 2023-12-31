using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : ArchProjectile
{
    [SerializeField]
    private float explosionDelay;
    [SerializeField]
    private float explosionDuration;
    [SerializeField]
    private GameObject Explosion;

    public override void InitArch(Vector2 archDir, Vector2 targetPoint, bool fixedArrival)
    {
        base.InitArch(archDir, targetPoint, fixedArrival);

        Invoke("SpawnGrenade", archDuration * 2);
        Duration = archDuration * 2 + explosionDelay + explosionDuration;
        StartLifetime();
    }

    public void SpawnGrenade()
    {
        GameManager.Instance.AudioManager.PlaySound(AudioRef.GrenadeDrop);
        Speed = 0;
        initialV = 0;
        decay = 0;
        Invoke("ExplodeGrenade", explosionDelay);
    }

    public void ExplodeGrenade()
    {
        GameManager.Instance.AudioManager.PlaySound(AudioRef.GrenadeExplosion);

        col2d.enabled = true;
        col2d.offset = col2d.offset + Vector2.right * 0.05f;
        Explosion.SetActive(true);
    }

    public override void PoolSelf()
    {
        col2d.enabled = false;
        Explosion.SetActive(false);
        base.PoolSelf();
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform == shooter)
            return;
        if (!collision.isTrigger)
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
                foundTarget.TakeDamage(damage, 0, ElementType.noElement);
                foundTarget.AddForce((collision.transform.position - transform.position).normalized * knockbackForce);

            }
        }
        if (poolOnContact)
            PoolSelf();
    }
}
