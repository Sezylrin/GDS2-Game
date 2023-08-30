using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MeleeEnemy : Enemy
{
    protected enum MeleeTimer
    {
        windupDurationTimer,
        attackDurationTimer
    }

    [field: SerializeField] protected Timer MeleeTimers { get; private set; }
    [field: SerializeField] protected int WindupDuration { get; set; } = 1;
    [field: SerializeField] protected float AttackDuration { get; set; } = 2;

    [field: SerializeField] protected GameObject WarningBox { get; set; }
    [field: SerializeField] protected GameObject AttackHitbox { get; set; }

    [field: SerializeField] bool debugAttemptAttack { get; set; }

    protected override void Start()
    {
        base.Start();
        MeleeTimers = TimerManager.Instance.GenerateTimers(typeof(MeleeTimer), gameObject);
        MeleeTimers.times[(int)MeleeTimer.windupDurationTimer].OnTimeIsZero += EndWindup;
        MeleeTimers.times[(int)MeleeTimer.attackDurationTimer].OnTimeIsZero += EndAttack;
    }

    protected override void Update()
    {
        base.Update();
        if (debugAttemptAttack)
        {
            debugAttemptAttack = false;
            AttemptAttack();
        }
    }

    protected override void Attack()
    {
        base.Attack();
        BeginWindup();
    }

    protected virtual void BeginWindup()
    {
        MeleeTimers.SetTime((int)MeleeTimer.windupDurationTimer, WindupDuration);
        WarningBox.SetActive(true);
    }

    protected virtual void EndWindup(object sender, EventArgs e)
    {
        WarningBox.SetActive(false);
        BeginAttack();
    }

    protected virtual void BeginAttack()
    {
        MeleeTimers.SetTime((int)MeleeTimer.attackDurationTimer, AttackDuration);
        AttackHitbox.SetActive(true);
    }

    protected virtual void EndAttack(object sender, EventArgs e)
    {
        AttackHitbox.SetActive(false);
    }

    
}
