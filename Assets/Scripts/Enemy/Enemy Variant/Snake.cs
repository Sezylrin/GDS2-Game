using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snake : Enemy, IPoolable<Snake>
{
    [field: Header("Snake")]
    [field: SerializeField] protected GameObject SwipeHitbox { get; set; }
    [field: SerializeField] protected GameObject DaggerPrefab { get; set; }
    [field: SerializeField] protected Transform DaggerSpawnPoint { get; set; }
    [field: SerializeField] protected float DaggerMoveSpeed { get; set; } = 3;
    [field: SerializeField] protected GameObject ChainHitbox { get; set; }
    [field: SerializeField] protected BoxCollider2D col2D { get; set; }
    private SnakeScriptableObject SnakeSO;


    #region PoolingVariables
    public Pool<Snake> Pool { get; set; }
    public bool IsPooled { get; set; }

    protected Pool<Dagger> pool;
    //protected Pool<Dagger> pool;
    #endregion

    protected override void Start()
    {
        base.Start();
        GameManager.Instance.PoolingManager.FindPool(DaggerPrefab, out pool);
    }

    public override void SetInheritanceSO()
    {
        SnakeSO = SO as SnakeScriptableObject;
    }

    public override void SetStatsFromScriptableObject()
    {
        base.SetStatsFromScriptableObject();
    }

    protected override void DetermineAttackPathing()
    {
        switch (CurrentAttack)
        {
            case 1:
                SetDestination(transform.position);
                Debug.Log("Attempting to Lunge");
                break;
            case 2:
                Debug.Log("Attempting to Spit Venom");
                break;
            case 3:
                SetDestination(transform.position);
                Debug.Log("Attempting to Acid Pool");
                break;
        }
    }

    #region 1 - Lunge
    protected override void Attack1()
    {

    }
    #endregion

    #region 2 - Spit Venom
    protected override void Attack2()
    {

    }
    #endregion

    #region 3 - Acid Pool
    protected override void Attack3()
    {

    }
    #endregion

    protected override void EndAttack(object sender, EventArgs e)
    {
        base.EndAttack(sender, e);
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

    public override void OnDeath(bool overrideKill = false)
    {
        base.OnDeath(overrideKill);
        PoolSelf();
    }

    #region PoolingFunctions
    public void PoolSelf()
    {
        Pool.PoolObj(this);
    }
    #endregion 
}
