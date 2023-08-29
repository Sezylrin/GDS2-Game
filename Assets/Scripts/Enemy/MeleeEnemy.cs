using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MeleeEnemy : Enemy
{
    [field: SerializeField] protected GameObject WarningBox { get; set; }
    [field: SerializeField] protected GameObject AttackHitbox { get; set; }

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
    }

    protected override void Attack()
    {
        base.Attack();
        BeginWindup();
    }

    protected override void BeginWindup()
    {
        base.BeginWindup();
        WarningBox.SetActive(true);
    }

    protected override void EndWindup(object sender, EventArgs e)
    {
        base.EndWindup(sender, e);
        WarningBox.SetActive(false);
        BeginAttack();
    }

    protected override void BeginAttack()
    {
        base.BeginAttack();
        AttackHitbox.SetActive(true);
    }

    protected override void EndAttack(object sender, EventArgs e)
    {
        base.EndAttack(sender, e);
        AttackHitbox.SetActive(false);
    }
}
