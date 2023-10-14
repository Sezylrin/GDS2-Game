using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMeleeEnemy : MeleeEnemy
{


    public override void OnDeath(bool overrideKill = false)
    {
        base.OnDeath(overrideKill);
        PoolSelf();
    }

}
