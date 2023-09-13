using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brambles : ComboBase
{
    // Start is called before the first frame update
    [SerializeField][Range(0,1)]
    private float[] slowRatio = new float[3];
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        IDamageable foundTarget;
        if (UtilityFunction.FindComponent(collision.transform, out foundTarget))
        {
            if (hitTargets.Contains(foundTarget))
            {
                return;
            }
            hitTargets.Add(foundTarget);
            foundTarget.ModifySpeed(slowRatio[tier]);
        }

    }

    protected override void OnTriggerExit2D(Collider2D collision)
    {
        IDamageable foundTarget;
        if (UtilityFunction.FindComponent(collision.transform, out foundTarget))
        {
            if (!hitTargets.Contains(foundTarget))
            {
                return;
            }
            hitTargets.Remove(foundTarget);
            foundTarget.ResetSpeed();
        }
    }
}
