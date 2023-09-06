using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMeleeEnemy : MeleeEnemy, IPoolable<TestMeleeEnemy>
{
    #region PoolingVariables
    public Pool<TestMeleeEnemy> Pool { get; set; }
    public bool IsPooled { get; set; }

    #endregion

    public override void OnDeath()
    {
        base.OnDeath();
        PoolSelf();
    }

    #region PoolingFunctions
    public void PoolSelf()
    {
        Pool.PoolObj(this);
    }
    #endregion 
}
