using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MeleeEnemy : Enemy
{
    [field: Header("Melee Stats")]
    [field: SerializeField] protected Transform hitboxCentre { get; set; }
    [field: SerializeField] protected GameObject WarningBox { get; set; }
    [field: SerializeField] protected GameObject AttackHitbox { get; set; }
    [field: SerializeField] protected float MinimumAttackRange { get; set; }
    [field: SerializeField] protected BoxCollider2D col2D { get; set; }
    protected Vector2 dir { get; set; }
    protected bool hitTarget = false;
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
        dir = (targetTr.position - transform.position).normalized;
        hitboxCentre.eulerAngles = new Vector3(0, 0, CustomMath.ClampedDirection(Vector2.right, dir));
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
        col2D.includeLayers = TargetLayer;
        AttackHitbox.SetActive(true);
    }

    protected override void EndAttack(object sender, EventArgs e)
    {
        base.EndAttack(sender, e);
        AttackHitbox.SetActive(false);
        col2D.includeLayers = 0;
        hitTarget = false;
    }

    protected override void InterruptAttack()
    {
        base.InterruptAttack();
        WarningBox.SetActive(false);
        AttackHitbox.SetActive(false);
        col2D.excludeLayers = 0;
    }

    protected override void DetermineAttackPathing()
    {
        Vector3 targetpoint = targetTr.position;
        Vector3 minimumRange = transform.position - targetpoint;
        minimumRange = minimumRange.normalized * MinimumAttackRange;
        targetpoint += minimumRange;
        SetDestination(targetpoint);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.isTrigger || !AttackHitbox.activeSelf || hitTarget)
            return;
        IDamageable foundTarget;
        if (UtilityFunction.FindComponent(collision.transform, out foundTarget))
        {
            if (foundTarget is PlayerSystem)
            {
                PlayerSystem temp = foundTarget as PlayerSystem;
                if (temp.GetState() == playerState.perfectDodge)
                {
                    InterruptAttack();
                    temp.InstantRegenPoint();
                    temp.CounterSuccesful(this);
                }
                else
                {
                    DoDamage(foundTarget);
                }
            }
            else
                DoDamage(foundTarget);
            
        }
    }

    private void DoDamage(IDamageable target)
    {
        target.TakeDamage(Damage, 0, Element);
        target.AddForce(dir.normalized * AttackKnockback);
        hitTarget = true;
    }
}
