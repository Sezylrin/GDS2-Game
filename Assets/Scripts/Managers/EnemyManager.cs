using System;
using KevinCastejon.MoreAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.VisualScripting;

public class EnemyManager : MonoBehaviour
{
    protected enum EnemyManagerTimer
    {
        attackDelayTimer,
        attackPointRefeshTimer
    }

    public static EnemyManager Instance { get; set; }

    [field: Header("Info")]
    [field: SerializeField, ReadOnly] private int AttackPoints { get; set; }
    [field: SerializeField] private int MaxAttackPoints { get; set; } = 10;
    [field: SerializeField] private float AttackPointRefreshRate { get; set; } = 2;
    [field: SerializeField] private bool CanEnemyAttack { get; set; } = true;
    [field: SerializeField] private float AttackDelay { get; set; } = 0.5f;
    [field: SerializeField] private Timer EnemyManagerTimers { get; set; }
    [field: SerializeField] private Transform TestSpawnPoint { get; set; }

    [field: SerializeField, ReadOnly] private int ActiveEnemies { get; set; }
    [field: SerializeField, ReadOnly] private int TotalEnemiesThisRoom { get; set; }

    [field: SerializeField] private GameObject[] enemyPrefabs;

    private Pool<TestMeleeEnemy> testMeleeEnemyPool;
    private Pool<TestRangedEnemy> testRangedEnemyPool;

    [field: Header("Debug")]
    [field: SerializeField] bool debugTestSpawn { get; set; }


    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
        }
        else
        {
            DestroyImmediate(this);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        EnemyManagerTimers = TimerManager.Instance.GenerateTimers(typeof(EnemyManagerTimer), gameObject);
        EnemyManagerTimers.times[(int)EnemyManagerTimer.attackDelayTimer].OnTimeIsZero += EnableAttack;
        EnemyManagerTimers.times[(int)EnemyManagerTimer.attackPointRefeshTimer].OnTimeIsZero += RefreshAttackPoint;

        AttackPoints = MaxAttackPoints;

        PoolingManager.Instance.FindPool(enemyPrefabs[0], out testMeleeEnemyPool);
        PoolingManager.Instance.FindPool(enemyPrefabs[1], out testRangedEnemyPool);
        
    }

    // Update is called once per frame
    void Update()
    {
        if (debugTestSpawn)
        {
            debugTestSpawn = false;
            SpawnEnemy();
        }
    }

    #region Attacking
    public bool CanAttack()
    {
        return CanEnemyAttack;
    }

    public void ManagerAttack()
    {
        if (AttackPoints == MaxAttackPoints) StartAttackPointRefresh();
        AttackPoints--;
        CanEnemyAttack = false;
        EnemyManagerTimers.SetTime((int)EnemyManagerTimer.attackDelayTimer, AttackDelay);
    }

    private void EnableAttack(object sender, EventArgs e)
    {
        CanEnemyAttack = true;
    }

    private void StartAttackPointRefresh()
    {
        EnemyManagerTimers.SetTime((int)EnemyManagerTimer.attackPointRefeshTimer, AttackPointRefreshRate);
    }

    private void RefreshAttackPoint(object sender, EventArgs e)
    {
        if (AttackPoints < MaxAttackPoints) AttackPoints++;
        if (AttackPoints < MaxAttackPoints) StartAttackPointRefresh();
    }

    public void DecrementActiveEnemyCounter()
    {
        ActiveEnemies--;
    }
    #endregion

    #region Spawning
    private EnemyType SelectEnemyToSpawn()
    {
        return EnemyType.Type1;
    }

    private ElementType SelectEnemyElement()
    {
        return ElementType.noElement;
    }

    private void SpawnEnemy()
    {
        EnemyType enemyToSpawn = SelectEnemyToSpawn();
        ElementType enemyElement = SelectEnemyElement();

        Enemy temp;
        switch (enemyToSpawn)
        {
            case EnemyType.Type1:
                temp = testMeleeEnemyPool.GetPooledObj();
                temp.Init(TestSpawnPoint.position, enemyElement);
                break;
            case EnemyType.Type2:
                temp = testRangedEnemyPool.GetPooledObj();
                temp.Init(TestSpawnPoint.position, enemyElement);
                break;
        }
    }

    #endregion
}
