using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RangedEnemy : Enemy
{
    protected enum RangedTimer
    {
        windupDurationTimer,
        warningFlashTimer,
        warningFlashDelay,
        tempAttackDeleterTimer
    }

    [field: SerializeField] protected Timer RangedTimers { get; private set; }
    [field: SerializeField] protected float WindupDuration { get; set; } = 1;
    [field: SerializeField] protected float AttackDuration { get; set; } = 2;
    [field: SerializeField] protected float FlashDuration { get; set; } = 0.1f;

    [field: SerializeField] protected GameObject WarningFlash { get; set; }
    [field: SerializeField] protected GameObject AttackHitbox { get; set; }
    protected bool FlashedOnce { get; set; } = false;

    [field: SerializeField] bool debugAttemptAttack { get; set; }

    protected override void Start()
    {
        base.Start();
        RangedTimers = TimerManager.Instance.GenerateTimers(typeof(RangedTimer), gameObject);
        RangedTimers.times[(int)RangedTimer.windupDurationTimer].OnTimeIsZero += EndWindup;
        RangedTimers.times[(int)RangedTimer.warningFlashTimer].OnTimeIsZero += DisableWarningSign;
        RangedTimers.times[(int)RangedTimer.warningFlashDelay].OnTimeIsZero += SecondWarningFlash;
        RangedTimers.times[(int)RangedTimer.tempAttackDeleterTimer].OnTimeIsZero += EndAttack;
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
        RangedTimers.SetTime((int)RangedTimer.windupDurationTimer, WindupDuration);
        FirstWarningFlash();
    }

    protected virtual void EndWindup(object sender, EventArgs e)
    {
        BeginAttack();
    }

    protected virtual void FirstWarningFlash()
    {
        EnableWarningSign();
        FlashedOnce = true;
    }

    protected virtual void EnableWarningSign()
    {
        RangedTimers.SetTime((int)RangedTimer.warningFlashTimer, FlashDuration);
        WarningFlash.SetActive(true);
    }

    protected virtual void DisableWarningSign(object sender, EventArgs e)
    {
        WarningFlash.SetActive(false);
        if (FlashedOnce) WaitForFlashDelay();
    }

    protected virtual void WaitForFlashDelay()
    {
        RangedTimers.SetTime((int)RangedTimer.warningFlashDelay, FlashDuration);
    }

    protected virtual void SecondWarningFlash(object sender, EventArgs e)
    {
        EnableWarningSign();
        FlashedOnce = false;
    }

    protected virtual void BeginAttack()
    {
        RangedTimers.SetTime((int)RangedTimer.tempAttackDeleterTimer, AttackDuration);
        AttackHitbox.SetActive(true);
    }

    protected virtual void EndAttack(object sender, EventArgs e)
    {
        AttackHitbox.SetActive(false);
    }
}
