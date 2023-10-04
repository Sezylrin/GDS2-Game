using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stomp : EnemyProjectile, IPoolable<Stomp>
{

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);
    }

    #region Pooling
    public Pool<Stomp> Pool { get; set; }
    public bool IsPooled { get; set; }

    public override void PoolSelf()
    {
        col2d.includeLayers = 0;
        rb.includeLayers = 0;
        col2d.excludeLayers = 0;
        rb.excludeLayers = 0;
        if (!IsPooled)
            Pool.PoolObj(this);
    }
    #endregion
}