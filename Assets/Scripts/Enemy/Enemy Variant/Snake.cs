using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snake : Enemy
{
    [field: Header("Snake")]
    [field: SerializeField] protected GameObject ProjectileGlobPF { get; set; }
    [field: SerializeField] protected GameObject AcidGlobPF { get; set; }
    [field: SerializeField] protected Transform GlobSpawnPoint { get; set; }
    private SnakeScriptableObject SnakeSO;
    private Pool<EnemyProjectile> globPool;
    private Pool<EnemyProjectile> acidGlobPool;
    private float SingleShotSpeed;
    private int RapidFireAmount;
    private float RapidFireSpeed;
    private float AcidBlobSpeed;
    public List<GameObject> targetLocation;
    protected override void Start()
    {
        base.Start();
        GameManager.Instance.PoolingManager.FindPool(ProjectileGlobPF, out globPool);
        GameManager.Instance.PoolingManager.FindPool(AcidGlobPF, out acidGlobPool);
        //InvokeRepeating("Attack3", 0, 2.5f);
    }
    protected override void SetInheritanceSO()
    {
        SnakeSO = SO as SnakeScriptableObject;
    }

    public override void SetStatsFromScriptableObject()
    {
        base.SetStatsFromScriptableObject();
        SingleShotSpeed = SnakeSO.SingleShotSpeed;
        RapidFireSpeed = SnakeSO.RapidFireSpeed;
        RapidFireAmount = SnakeSO.RapidFireAmount;
        AcidBlobSpeed = SnakeSO.AcidBlobSpeed;
    }

    protected override void DetermineAttackPathing()
    {
        SetDestination(transform.position);
    }

    #region Attack1
    protected override void Attack1()
    {
        dir = (targetTr.position - transform.position);
        PivotPoint.eulerAngles = CustomMath.GetEularAngleToDir(Vector2.right, dir);
        bool initial;
        EnemyProjectile temp = globPool.GetPooledObj(out initial);
        if (initial)
        {
            temp.NewInstance();
        }
        temp.Init(dir, GlobSpawnPoint.position, TargetLayer, Attack3Damage, Attack3Duration, SingleShotSpeed, AttackKnockback1, transform);
        //Debug.Log("Using NormalAttack");
    }
    #endregion

    #region Attack2
    protected override void Attack2()
    {
        StartCoroutine(ShootMultiple());
        //Debug.Log("Using NormalAttack");
    }

    private IEnumerator ShootMultiple()
    {
        float dur = Attack2Duration / RapidFireAmount;
        for (int i = 0; i < RapidFireAmount; i++)
        {
            dir = (targetTr.position - transform.position);
            PivotPoint.eulerAngles = CustomMath.GetEularAngleToDir(Vector2.right, dir);
            bool initial;
            EnemyProjectile temp = globPool.GetPooledObj(out initial);
            if (initial)
            {
                temp.NewInstance();
            }
            temp.Init(dir, GlobSpawnPoint.position, TargetLayer, Attack2Damage, Attack2Duration * 10, RapidFireSpeed, AttackKnockback2, transform);
            yield return new WaitForSeconds(dur);
        }
    }
    #endregion

    #region Attack3
    [ContextMenu("attack3")]
    protected override void Attack3()
    {
        dir = (targetTr.position - transform.position).normalized;
        PivotPoint.eulerAngles = CustomMath.GetEularAngleToDir(Vector2.right, dir);
        float randomDeviation = Random.Range(0f, 120f);
        Vector2[] positions = { 
            CustomMath.RotateByEularAngles(Vector2.right, randomDeviation),
            CustomMath.RotateByEularAngles(Vector2.right, randomDeviation + 120),
            CustomMath.RotateByEularAngles(Vector2.right, randomDeviation + 240)
        };
        for (int i = 0; i < 3; i++)
        {
            float angle = Random.Range(0f, 360f);
            float dist = Random.Range(0.5f, 2f);
            positions[i] += CustomMath.RotateByEularAngles(Vector2.right * dist, angle);
            Vector2 dir = (positions[i] + (Vector2)targetTr.position) - (Vector2)GlobSpawnPoint.position;

            EnemyProjectile temp = acidGlobPool.GetPooledObj(out bool initial);
            if (initial)
            {
                temp.NewInstance();
            }
            temp.Init(dir, GlobSpawnPoint.position, TargetLayer, Attack3Damage, AcidBlobSpeed + SnakeSO.AcidPoolDuration, AcidBlobSpeed, AttackKnockback3, transform);
            (temp as AcidPool).InitArch(Vector2.up * 2, positions[i] + (Vector2)targetTr.position, true);
            //targetLocation[i].transform.position = positions[i] + (Vector2)targetTr.position;
        }
        
    }

    #endregion

}
