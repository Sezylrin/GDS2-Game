using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cheetah : Enemy
{
    [field: Header("Cheetah")]
    [field: SerializeField] protected GameObject SwipeHitbox { get; set; }
    [field: SerializeField] protected GameObject DaggerPrefab { get; set; }
    [field: SerializeField] protected Transform DaggerSpawnPoint { get; set; }
    [field: SerializeField] protected float DaggerMoveSpeed { get; set; } = 3;
    [field: SerializeField] protected GameObject ChainHitbox { get; set; }


    #region PoolingVariables

    protected Pool<EnemyProjectile> pool;
    #endregion


    #region 1 - Swipe
    protected override void Attack1()
    {
        dir = (targetTr.position - transform.position).normalized;
        PivotPoint.eulerAngles = new Vector3(0, 0, CustomMath.ClampedDirection(Vector2.right, dir));
        col2D.includeLayers = TargetLayer;
        SwipeHitbox.SetActive(true);
    }
    #endregion

    #region 2 - Daggers
    protected override void Attack2()
    {
        dir = (targetTr.position - transform.position).normalized;
        PivotPoint.eulerAngles = new Vector3(0, 0, CustomMath.ClampedDirection(Vector2.right, dir));
        col2D.includeLayers = TargetLayer;
        bool initial;
        EnemyProjectile temp = pool.GetPooledObj(out initial);
        if (initial)
        {
            temp.NewInstance();
        }
        temp.Init(targetTr.position - transform.position, DaggerSpawnPoint.position, TargetLayer, Attack2Damage, Attack2Duration, DaggerMoveSpeed, AttackKnockback, transform);
    }
    #endregion

    #region 3 - Chain
    protected override void Attack3()
    {
        dir = (targetTr.position - transform.position).normalized;
        PivotPoint.eulerAngles = new Vector3(0, 0, CustomMath.ClampedDirection(Vector2.right, dir));
        col2D.includeLayers = TargetLayer;
        ChainHitbox.SetActive(true);
    }
    #endregion

    protected override void EndAttack(object sender, EventArgs e)
    {
        base.EndAttack(sender, e);
        SwipeHitbox.SetActive(false);
        ChainHitbox.SetActive(false);
    }

    protected override void DetermineAttackPathing()
    {
        Vector3 targetpoint = targetTr.position;
        Vector3 minimumRange = transform.position - targetpoint;
        minimumRange = minimumRange.normalized * 2;
        targetpoint += minimumRange;
        SetDestination(targetpoint);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.isTrigger || !SwipeHitbox.activeSelf || !ChainHitbox.activeSelf || hitTarget)
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
                    temp.Counter();
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



}
