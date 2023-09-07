using System;
using KevinCastejon.MoreAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.VisualScripting;
using Random = UnityEngine.Random;

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
    [field: SerializeField] private List<Transform> SpawnLocations { get; set; }


    [field: SerializeField] ElementType debugElementForAttacksList { get; set; }
    [field: SerializeField] bool debugUpdateAttacksList { get; set; }
    [field: SerializeField] private List<ElementType> AttacksList { get; set; }

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
        if (debugUpdateAttacksList)
        {
            debugUpdateAttacksList = false;
            UpdateAttacksList(debugElementForAttacksList);
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
    private void UpdateAttacksList(ElementType type)
    {
        AttacksList.Add(type);
        if (AttacksList.Count > 25) 
        {
            AttacksList.RemoveAt(0);
        }
    }
    
    
    private EnemyType SelectEnemyToSpawn()
    {
        return EnemyType.Type1;
    }

    private ElementType SelectEnemyElement()
    {
        int noElementCount = 0;
        int fireCount = 0;
        int waterCount = 0;
        int shockCount = 0;
        int windCount = 0;
        int poisonCount = 0;
        int natureCount = 0;

        foreach (ElementType element in AttacksList)
        {
            switch (element) 
            {
                case ElementType.noElement: noElementCount++; break;
                case ElementType.fire: fireCount++; break;
                case ElementType.water: waterCount++; break;
                case ElementType.electric: shockCount++; break;
                case ElementType.wind: windCount++; break;
                case ElementType.poison: poisonCount++; break;
                case ElementType.nature: natureCount++; break;
            }
        }
        int randomValue = Random.Range(1,101);

        int fireChance = fireCount * 3;
        int waterChance = fireChance + waterCount * 3;
        int shockChance = waterChance + shockCount * 3;
        int windChance = shockChance + windCount * 3;
        int poisonChance = windChance + poisonCount * 3;
        int natureChance = poisonChance + natureCount * 3;

        if (randomValue < fireChance) return ElementType.fire;
        else if (fireChance < randomValue && randomValue < waterChance) return ElementType.water;      
        else if (waterChance < randomValue && randomValue < shockChance) return ElementType.electric;
        else if (shockChance < randomValue && randomValue < windChance) return ElementType.wind;
        else if (windChance < randomValue && randomValue < poisonChance) return ElementType.poison;
        else if (poisonChance < randomValue && randomValue < natureChance) return ElementType.nature;
        else return ElementType.noElement;
    }

    private Vector2 SelectSpawnLocation()
    {
        int index = Random.Range(0, SpawnLocations.Count());
        Vector2 spawnLocation = SpawnLocations[index].position;
        SpawnLocations.RemoveAt(index);

        return spawnLocation;
    }

    private void SpawnEnemy()
    {
        EnemyType enemyToSpawn = SelectEnemyToSpawn();
        ElementType enemyElement = SelectEnemyElement();
        Vector2 spawnLocation = SelectSpawnLocation();

        Enemy temp;
        switch (enemyToSpawn)
        {
            case EnemyType.Type1:
                temp = testMeleeEnemyPool.GetPooledObj();
                temp.Init(spawnLocation, enemyElement);
                break;
            case EnemyType.Type2:
                temp = testRangedEnemyPool.GetPooledObj();
                temp.Init(spawnLocation, enemyElement);
                break;
        }
        ActiveEnemies++;
    }

    #endregion
}
