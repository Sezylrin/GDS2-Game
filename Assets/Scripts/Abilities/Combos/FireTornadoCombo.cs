using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireTornadoCombo : ComboBase
{
    // Start is called before the first frame update
    [SerializeField]
    private float pullForce;

    protected override void DoDamage(object sender, EventArgs e)
    {
        base.DoDamage(sender, e);
        foreach (Enemy damaged in hitTargets)
        {
            damaged.AddForce(((Vector2)transform.position - damaged.rb.position).normalized * pullForce);
        }
    }
}
