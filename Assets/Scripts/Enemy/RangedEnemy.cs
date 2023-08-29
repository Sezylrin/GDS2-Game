using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RangedEnemy : Enemy
{
    protected enum RangedTimer
    {
        warningFlashTimer,
        warningFlashDelay
    }

    [field: SerializeField] protected Timer RangedTimers { get; private set; }
    [field: SerializeField] protected float FlashDuration { get; set; } = 0.1f;

    [field: SerializeField] protected GameObject WarningFlash { get; set; }
    [field: SerializeField] protected GameObject ProjectilePrefab { get; set; }
    [field: SerializeField] protected Transform ProjectileSpawnPoint { get; set; }

    protected bool FlashedOnce { get; set; } = false;

    protected override void Start()
    {
        base.Start();
        RangedTimers = TimerManager.Instance.GenerateTimers(typeof(RangedTimer), gameObject);
        RangedTimers.times[(int)RangedTimer.warningFlashTimer].OnTimeIsZero += DisableWarningSign;
        RangedTimers.times[(int)RangedTimer.warningFlashDelay].OnTimeIsZero += SecondWarningFlash;
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
        FirstWarningFlash();
    }

    protected override void EndWindup(object sender, EventArgs e)
    {
        base.EndWindup(sender, e);
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

    protected override void BeginAttack()
    {
        base.BeginAttack();
        Instantiate(ProjectilePrefab, ProjectileSpawnPoint.position, Quaternion.identity);
    }

    protected override void EndAttack(object sender, EventArgs e)
    {
        base.EndAttack(sender, e);
    }
}
