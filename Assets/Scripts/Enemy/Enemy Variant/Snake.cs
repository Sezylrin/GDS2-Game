using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snake : Enemy, IPoolable<Snake>
{
    [field: Header("Snake")]


    #region PoolingVariables
    public Pool<Snake> Pool { get; set; }
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
