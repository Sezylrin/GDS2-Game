using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brambles : ComboBase
{
    // Start is called before the first frame update
    [SerializeField][Range(0,1)]
    private float[] slowRatio = new float[3];

    protected override void DoDamage(object sender, EventArgs e)
    {
        base.DoDamage(sender, e);
        foreach (IDamageable damaged in hitTargets)
        {
            float currentvelocity = damaged.rb.velocity.magnitude;
            currentvelocity *= slowRatio[tier];
            damaged.rb.velocity = damaged.rb.velocity.normalized * currentvelocity;
        }
    }
}
