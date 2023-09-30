using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rhino : MeleeEnemy, IPoolable<Rhino>
{
    #region PoolingVariables
    public Pool<Rhino> Pool { get; set; }
    public bool IsPooled { get; set; }

    #endregion

    public override void OnDeath(bool overrideKill = false)
    {
        base.OnDeath(overrideKill);
        PoolSelf();
    }

    #region PoolingFunctions
    public void PoolSelf()
    {
        Pool.PoolObj(this);
    }
    #endregion 
}
