using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestRangedEnemy : RangedEnemy
{
    #region PoolingVariables

    #endregion

    public override void OnDeath(bool overrideKill = false)
    {
        base.OnDeath(overrideKill);
        PoolSelf();
    }


    #region PoolingFunctions
    #endregion

}
