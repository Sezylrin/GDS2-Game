using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blizzard : ComboBase
{
    // Start is called before the first frame update
    [SerializeField][Range(0,1)]
    private float slowRatio;
    [SerializeField][Range(1,3)]
    private float bonusDamage;
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        Enemy foundTarget;
        if (UtilityFunction.FindComponent(collision.transform, out foundTarget))
        {
            if (hitTargets.Contains(foundTarget))
            {
                return;
            }
            hitTargets.Add(foundTarget);
            foundTarget.ModifySpeed(slowRatio);
            foundTarget.InBlizzard(bonusDamage);
        }

    }

    protected override void OnTriggerExit2D(Collider2D collision)
    {
        Enemy foundTarget;
        if (UtilityFunction.FindComponent(collision.transform, out foundTarget))
        {
            if (!hitTargets.Contains(foundTarget))
            {
                return;
            }
            hitTargets.Remove(foundTarget);
            foundTarget.ResetSpeed();
            foundTarget.ExitBlizzard();
        }
    }
}
