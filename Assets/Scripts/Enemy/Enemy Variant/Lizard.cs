using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lizard : Enemy, IPoolable<Lizard>
{
    [field: Header("Lizard")]


    #region PoolingVariables
    public Pool<Lizard> Pool { get; set; }
    public bool IsPooled { get; set; }

    //protected Pool<Dagger> pool;
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
