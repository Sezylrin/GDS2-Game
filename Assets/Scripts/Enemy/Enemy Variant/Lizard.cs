using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lizard : Enemy
{
    private LizardScriptableObject LizardSO;
    private Pool<EnemyProjectile> spearPool;
    private Pool<EnemyProjectile> boomerangPool;
    private Pool<EnemyProjectile> grenadePool;

    [Header("Lizard"), SerializeField]
    private GameObject spearPF;
    [SerializeField]
    private GameObject boomerangPF;
    [SerializeField]
    private GameObject grenadePF;
    [SerializeField]
    private Transform projectileSpawnPoint;
    [SerializeField]
    private float spearSpeed;
    [SerializeField]
    private float grenadeSpeed;
    [SerializeField]
    private float boomerangSpeed;
    protected override void Start()
    {
        base.Start();
        GameManager.Instance.PoolingManager.FindPool(spearPF, out spearPool);
        GameManager.Instance.PoolingManager.FindPool(boomerangPF, out boomerangPool);
        GameManager.Instance.PoolingManager.FindPool(grenadePF, out grenadePool);
    }

    protected override void SetInheritanceSO()
    {
        LizardSO = SO as LizardScriptableObject;
    }

    public override void SetStatsFromScriptableObject()
    {
        base.SetStatsFromScriptableObject();
    }

    protected override void DetermineAttackPathing()
    {
        SetDestination(transform.position);
    }

    #region Attack1
    protected override void Attack1()
    {
        GameManager.Instance.AudioManager.PlaySound(AudioRef.Spear);
        dir = (targetTr.position - transform.position);
        PivotPoint.eulerAngles = CustomMath.GetEularAngleToDir(Vector2.right, dir);
        bool initial;
        EnemyProjectile temp = spearPool.GetPooledObj(out initial);
        if (initial)
        {
            temp.NewInstance();
        }
        temp.Init(dir, projectileSpawnPoint.position, TargetLayer, Attack1Damage, Attack1Duration, spearSpeed, AttackKnockback1, transform);
        (temp as ArchProjectile).InitArch(Vector2.up * 0.5f, targetTr.position, false);
    }
    #endregion

    #region Attack2
    protected override void Attack2()
    {
        dir = (targetTr.position - transform.position);
        PivotPoint.eulerAngles = CustomMath.GetEularAngleToDir(Vector2.right, dir);
        bool initial;
        EnemyProjectile temp = grenadePool.GetPooledObj(out initial);
        if (initial)
        {
            temp.NewInstance();
        }
        temp.Init(dir, projectileSpawnPoint.position, TargetLayer, Attack2Damage, Attack2Duration, grenadeSpeed, AttackKnockback2, transform);
        (temp as ArchProjectile).InitArch(Vector2.up * 2f, targetTr.position, true);
    }
    #endregion

    #region Attack3
    protected override void Attack3()
    {
        GameManager.Instance.AudioManager.PlaySound(AudioRef.Boomerang);
        dir = (targetTr.position - transform.position);
        PivotPoint.eulerAngles = CustomMath.GetEularAngleToDir(Vector2.right, dir);
        bool initial;
        EnemyProjectile temp = boomerangPool.GetPooledObj(out initial);
        if (initial)
        {
            temp.NewInstance();
        }
        temp.Init(dir, projectileSpawnPoint.position, TargetLayer, Attack3Damage, Attack3Duration, spearSpeed, AttackKnockback3, transform);
        Vector2 perpendicular = CustomMath.RotateByEularAngles(dir,90).normalized * (Random.Range(0,2) == 0? 1 : -1);
        (temp as ArchProjectile).InitArch(perpendicular * dir.magnitude * 0.25f, targetTr.position, false);
    }
    #endregion

}
