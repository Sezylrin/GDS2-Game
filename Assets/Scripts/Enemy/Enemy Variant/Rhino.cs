using BehaviourTree;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rhino : Enemy
{
    [field: Header("Rhino")]
    [field: SerializeField] protected GameObject ChargeHitbox { get; set; }
    [field: SerializeField] protected float ChargeSpeed { get; set; } = 1.5f;
    [field: SerializeField] public bool Charging { get; set; } = false;
    [field: SerializeField] protected GameObject ShockwaveHitbox { get; set; }
    [field: SerializeField] protected GameObject StompPrefab { get; set; }
    [field: SerializeField] protected Transform StompSpawnPoint { get; set; }
    [field: SerializeField] protected float StompMoveSpeed { get; set; }
    [field: SerializeField] protected float Attack2Range { get; set; } = 3;
    [field: SerializeField] protected CircleCollider2D shockwaveCol2D { get; set; }
    [field: SerializeField] protected float ShockwaveStartRadius { get; set; } = 3;
    [field: SerializeField] protected float ShockwaveEndRadius { get; set; } = 8;
    [field: SerializeField] protected float ShockwaveGrowthSpeed { get; set; } = 1;
    private RhinoScriptableObject RhinoSO;
    private Coroutine shockwaveCoroutine;
    private Pool<EnemyProjectile> pool;
    [SerializeField]
    private Animator shockwave;

    #region Tutorial
    public ModifyBoundary boundary;
    #endregion


    protected override void Start()
    {
        base.Start();
        GameManager.Instance.PoolingManager.FindPool(StompPrefab, out pool);
    }

    protected override void SetInheritanceSO()
    {
        RhinoSO = SO as RhinoScriptableObject;
    }

    public override void SetStatsFromScriptableObject()
    {
        base.SetStatsFromScriptableObject();
        ChargeSpeed = RhinoSO.ChargeSpeed;
        Attack2Range = RhinoSO.attack2Range;
        ShockwaveStartRadius = RhinoSO.shockwaveStartRadius;
        ShockwaveEndRadius = RhinoSO.shockwaveEndRadius;
        ShockwaveGrowthSpeed = RhinoSO.shockwaveGrowthSpeed;
        StompMoveSpeed = RhinoSO.stompMoveSpeed;
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
        PivotPoint.eulerAngles = CustomMath.GetEularAngleToDir(Vector2.right, dir);
        col2D.includeLayers = TargetLayer;
        ChargeHitbox.SetActive(true);
        StartCoroutine(StartCharge());
        Debug.Log("Using Charge");
    }

    protected IEnumerator StartCharge()
    {
        StartCoroutine(AutoEndCharge());
        
        Charging = true;
        float time = 0;
        GameManager.Instance.AudioManager.PlaySound(AudioRef.RhinoCharge);
        while (Charging)
        {
            if (time > 0.1f)
            {
                GameManager.Instance.AudioManager.PlaySound(AudioRef.RhinoCharge);
                time = 0;
            }
            time += Time.deltaTime;
            RaycastHit2D hit = Physics2D.CircleCast(transform.position + (Vector3)col2D.offset, ((CircleCollider2D)col2D).radius, dir, (dir * Time.deltaTime * ChargeSpeed).magnitude,TerrainLayers);
            if (hit.collider)
            {
                Charging = false;
                EnemyTimers.SetTime((int)EnemyTimer.attackDurationTimer, 0);
                break;
            }
            else
                transform.Translate(dir * Time.deltaTime * ChargeSpeed);

            yield return null;
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
        //EnemyTimers.ResetSpecificToZero((int)EnemyTimer.attackDurationTimer);
        ChargeHitbox.SetActive(false);
    }
    #endregion

    #region 2 - Shockwave
    protected override void Attack2()
    {
        shockwave.Play("ExpandingCircle");
        GameManager.Instance.AudioManager.PlaySound(AudioRef.RhinoStomp);
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
        GameManager.Instance.AudioManager.PlaySound(AudioRef.RhinoStomp);
        dir = (targetTr.position - transform.position);
        PivotPoint.eulerAngles = CustomMath.GetEularAngleToDir(Vector2.right, dir);
        col2D.includeLayers = TargetLayer;
        bool initial;
        EnemyProjectile temp = pool.GetPooledObj(out initial);
        if (initial)
        {
            temp.NewInstance();
        }
        temp.Init(dir, StompSpawnPoint.position, TargetLayer, Attack3Damage, Attack3Duration, StompMoveSpeed, AttackKnockback3, transform);
        Debug.Log("Using Stomp");
    }
    #endregion

    protected override void EndAttack()
    {
        base.EndAttack();
        ChargeHitbox.SetActive(false);
        ShockwaveHitbox.SetActive(false);
        StopCoroutine(StartCharge());
        Charging = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {        
        if ((!ChargeHitbox.activeSelf && !ShockwaveHitbox.activeSelf) || hitTarget)
            return;
        if (!collision.isTrigger)
            return;
        PlayerSystem foundTarget;
        if (UtilityFunction.FindComponent(collision.transform, out foundTarget))
        {
            if (Charging) Charging = false;
            if (foundTarget.GetState() == playerState.perfectDodge)
            {
                InterruptAttack();
                foundTarget.Counter();
            }
            else
            {
                DoDamage(foundTarget);
            }
        }
    }

    public override void PoolSelf()
    {
        ChargeHitbox.SetActive(false);
        ShockwaveHitbox.SetActive(false);
        base.PoolSelf();
    }

    public override void InterruptAttack()
    {
        EnemyTimers.ResetSpecificToZero((int)EnemyTimer.attackDurationTimer);
        base.InterruptAttack();
    }
    #region AI
    #endregion
}
