using BehaviourTree;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rhino : Enemy, IPoolable<Rhino>
{
    [field: Header("Rhino")]
    [field: SerializeField] protected GameObject ChargeHitbox { get; set; }
    [field: SerializeField] protected float ChargeSpeedMultiplier { get; set; } = 1.5f;
    [field: SerializeField] public bool Charging { get; set; } = false;
    [field: SerializeField] protected GameObject ShockwaveHitbox { get; set; }
    [field: SerializeField] protected GameObject StompPrefab { get; set; }
    [field: SerializeField] protected Transform StompSpawnPoint { get; set; }
    [field: SerializeField] protected float Attack2Range { get; set; } = 3;
    [field: SerializeField] protected Collider2D col2D { get; set; }
    [field: SerializeField] protected CircleCollider2D shockwaveCol2D { get; set; }
    [field: SerializeField] protected float ShockwaveStartRadius { get; set; } = 3;
    [field: SerializeField] protected float ShockwaveEndRadius { get; set; } = 8;
    [field: SerializeField] protected float ShockwaveGrowthSpeed { get; set; } = 1;
    private RhinoScriptableObject RhinoSO;
    private Coroutine shockwaveCoroutine;

    #region Tutorial
    public ModifyBoundary boundary;
    #endregion

    #region PoolingVariables
    public Pool<Rhino> Pool { get; set; }
    public bool IsPooled { get; set; }

    protected Pool<Stomp> pool;
    #endregion

    protected override void Start()
    {
        base.Start();
        GameManager.Instance.PoolingManager.FindPool(StompPrefab, out pool);
    }

    public override void SetInheritanceSO()
    {
        RhinoSO = SO as RhinoScriptableObject;
    }

    public override void SetStatsFromScriptableObject()
    {
        base.SetStatsFromScriptableObject();
        ChargeSpeedMultiplier = RhinoSO.chargeSpeedMultiplier;
        Attack2Range = RhinoSO.attack2Range;
        ShockwaveStartRadius = RhinoSO.shockwaveStartRadius;
        ShockwaveEndRadius = RhinoSO.shockwaveEndRadius;
        ShockwaveGrowthSpeed = RhinoSO.shockwaveGrowthSpeed;
    }

    protected override void DetermineAttackPathing()
    {
        switch (CurrentAttack)
        {
            case 1:
                SetDestination(transform.position);
                Debug.Log("Attempting to Charge");
                break;
            case 2:
                Vector3 targetpoint = targetTr.position;
                Vector3 minimumRange = transform.position - targetpoint;
                minimumRange = minimumRange.normalized * Attack2Range;
                targetpoint += minimumRange;
                SetDestination(targetpoint);
                Debug.Log("Attempting to Shockwave");
                break;
            case 3:
                SetDestination(transform.position);
                Debug.Log("Attempting to Stomp");
                break;
        }
    }

    #region 1 - Charge
    protected override void Attack1()
    {
        dir = (targetTr.position - transform.position).normalized;
        col2D.includeLayers = TargetLayer;
        ChargeHitbox.SetActive(true);
        StartCoroutine(StartCharge());
        Debug.Log("Using Charge");
    }

    protected IEnumerator StartCharge()
    {
        StartCoroutine(AutoEndCharge());
        
        Charging = true;
        ModifySpeed(ChargeSpeedMultiplier);
        while (Charging)
        {
            yield return null;
            transform.Translate(dir * Time.deltaTime * Speed);
        }
        EndCharge();
    }

    protected IEnumerator AutoEndCharge()
    {
        yield return new WaitForSeconds(Attack1Duration);
        if (Charging) Charging = false;
    }

    protected void EndCharge()
    {
        EnemyTimers.ResetSpecificToZero((int)EnemyTimer.attackDurationTimer);
    }
    #endregion

    #region 2 - Shockwave
    protected override void Attack2()
    {
        col2D.includeLayers = TargetLayer;
        ShockwaveHitbox.SetActive(true);
        //StartShockwave();
        Debug.Log("Using Shockwave");
    }

    private void StartShockwave()
    {
        if (ShockwaveGrowthSpeed != 0)
        {
            shockwaveCoroutine = StartCoroutine(StartAoeExpand());
        }
        else
        {
            shockwaveCol2D.radius = ShockwaveEndRadius;
        }
    }

    private IEnumerator StartAoeExpand()
    {
        float startTime = Time.time;
        float endRadius = ShockwaveEndRadius;
        for (float timer = 0; timer < ShockwaveGrowthSpeed; timer += Time.deltaTime)
        {
            float ratio = (Time.time - startTime) / ShockwaveGrowthSpeed;
            shockwaveCol2D.radius = ratio * endRadius;
            if (timer + Time.deltaTime >= ShockwaveGrowthSpeed)
                shockwaveCol2D.radius = endRadius;
            float radius = shockwaveCol2D.radius;
            ShockwaveHitbox.transform.localScale = new Vector3(radius, radius, radius);
            yield return null;
        }
        StopExpand();
    }

    private void StopExpand()
    {
        if (shockwaveCoroutine != null)
        {
            shockwaveCoroutine = null;
        }
        shockwaveCol2D.radius = ShockwaveStartRadius;
    }
    #endregion

    #region 3 - Stomp
    protected override void Attack3()
    {
        dir = (targetTr.position - transform.position).normalized;
        PivotPoint.eulerAngles = new Vector3(0, 0, CustomMath.ClampedDirection(Vector2.right, dir));
        col2D.includeLayers = TargetLayer;
        bool initial;
        Stomp temp = pool.GetPooledObj(out initial);
        if (initial)
        {
            temp.NewInstance();
        }
        temp.Init(targetTr.position - transform.position, StompSpawnPoint.position, TargetLayer, Attack3Damage, Attack3Duration, AttackKnockback, this);
        Debug.Log("Using Stomp");
    }
    #endregion

    protected override void EndAttack(object sender, EventArgs e)
    {
        base.EndAttack(sender, e);
        ChargeHitbox.SetActive(false);
        ShockwaveHitbox.SetActive(false);
        StopCoroutine(StartCharge());
        Charging = false;
    }

    public override void OnDeath(bool overrideKill = false)
    {
        base.OnDeath(overrideKill);
        PoolSelf();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.isTrigger || !ChargeHitbox.activeSelf || !ShockwaveHitbox.activeSelf || hitTarget)
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
                    if (Charging) Charging = false;
                }
            }
            else
                DoDamage(foundTarget);
        }
    }

    #region PoolingFunctions
    public void PoolSelf()
    {
        if(Pool != null)
            Pool.PoolObj(this);
        else
        {
            Destroy(gameObject);
            if (boundary)
                boundary.DisableBoundary();
        }
    }
    #endregion 
}
