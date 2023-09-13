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

    [field: Header("Info")]
    [field: SerializeField] private Timer EnemyManagerTimers { get; set; }

    [field: Header("EnemyAttacking")]
    [field: SerializeField, ReadOnly] private int AttackPoints { get; set; }
    [field: SerializeField] private int MaxAttackPoints { get; set; } = 10;
    [field: SerializeField] private float AttackPointRefreshRate { get; set; } = 2;
    [field: SerializeField, ReadOnly] private bool CanEnemyAttack { get; set; } = true;
    [field: SerializeField] private float AttackDelay { get; set; } = 0.5f;
    [field: SerializeField] ElementType debugElementForAttacksList { get; set; }
    [field: SerializeField] bool debugUpdateAttacksList { get; set; }
    [field: SerializeField] bool debugEmptyAttacksList { get; set; }
    [field: SerializeField] private List<ElementType> AttacksList { get; set; }

    [field: Header("Spawning")]
    [field: SerializeField] private GameObject[] enemyPrefabs;
    [field: SerializeField] private List<Transform> SpawnLocations { get; set; }
    [field: SerializeField, ReadOnly] private int ActiveEnemiesCount { get; set; }
    [field: SerializeField] bool debugTestSpawn { get; set; }
    [field: SerializeField, ReadOnly] private int EnemyPoints { get; set; }
    [field: SerializeField] int debugEnemyPoints { get; set; } = 10;
    [field: SerializeField] bool debugSetEnemyPoints { get; set; }
    [field: SerializeField, ReadOnly] int MeleeEnemySpawnChance { get; set; } = 75;
    [field: SerializeField, ReadOnly] int RangedEnemySpawnChance { get; set; } = 25;
    [field: SerializeField, Range(0, 100)] int debugMeleeSpawnChance { get; set; } = 50;
    [field: SerializeField] bool debugSetMeleeSpawnChance { get; set; }
    private Pool<TestMeleeEnemy> testMeleeEnemyPool;
    private Pool<TestRangedEnemy> testRangedEnemyPool;
    private List<Enemy> enemyList = new List<Enemy>();
    [field: SerializeField] bool debugKillEnemies { get; set; }

    private void Awake()
    {
       
    }

    // Start is called before the first frame update
    void Start()
    {
        EnemyManagerTimers = GameManager.Instance.TimerManager.GenerateTimers(typeof(EnemyManagerTimer), gameObject);
        EnemyManagerTimers.times[(int)EnemyManagerTimer.attackDelayTimer].OnTimeIsZero += EnableAttack;
        EnemyManagerTimers.times[(int)EnemyManagerTimer.attackPointRefeshTimer].OnTimeIsZero += RefreshAttackPoint;

        AttackPoints = MaxAttackPoints;

        GameManager.Instance.PoolingManager.FindPool(enemyPrefabs[0], out testMeleeEnemyPool);
        GameManager.Instance.PoolingManager.FindPool(enemyPrefabs[1], out testRangedEnemyPool);
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
        if (debugEmptyAttacksList)
        {
            debugEmptyAttacksList = false;
            EmptyAttacksList();
        }
        if (debugSetEnemyPoints)
        {
            debugSetEnemyPoints = false;
            SetEnemyPoints(debugEnemyPoints);
        }
        if (debugSetMeleeSpawnChance)
        {
            debugSetMeleeSpawnChance = false;
            SetMeleeSpawnChance(debugMeleeSpawnChance);
        }
        if (debugKillEnemies)
        {
            debugKillEnemies = false;
            KillEnemies();
        }
    }

    #region EnemyAttacking
    public bool CanAttack()
    {
        bool CanAttack = CanEnemyAttack && AttackPoints > 0;
        if (CanAttack)
            ManagerAttack();
        return CanAttack;
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
    #endregion

    #region PlayerAttacksList
    public void UpdateAttacksList(ElementType type)
    {
        AttacksList.Add(type);
        if (AttacksList.Count > 25)
        {
            AttacksList.RemoveAt(0);
        }
    }

    public void EmptyAttacksList()
    {
        AttacksList.Clear();
    }
    #endregion

    #region Spawning
    public void StartEnemySpawning(List<Transform> spawnPoints, int enemyPoints)
    {
        SetEnemyPoints(enemyPoints);
        SpawnLocations = spawnPoints;
        SpawnEnemy();
    }
    
    
    public void SetEnemyPoints(int points)
    {
        EnemyPoints = points;
    }

    private void SetMeleeSpawnChance(int chance)
    {
        MeleeEnemySpawnChance = chance;
        RangedEnemySpawnChance = 100 - MeleeEnemySpawnChance;
    }

    private EnemyType SelectEnemyToSpawn()
    {
        EnemyType type = EnemyType.TypeError;
        int cost = 0;
        if (EnemyPoints > 2) cost = Random.Range(1, 4);
        else if (EnemyPoints == 2) cost = Random.Range(1, 3);
        else if (EnemyPoints == 1) cost = 1;
        else
        {
            Debug.LogWarning("No More Enemy Points");
            return type;
        } 

        int randomValue = Random.Range(1, 101);


        cost = 1; // Temp ----------------------------------------------- Temp \\
        switch (cost)
        {
            case 1:
                if (randomValue < MeleeEnemySpawnChance) type = EnemyType.Type1;
                else type = EnemyType.Type2;
                break;
            case 2:
                break;
            case 3:
                break;
            default:
                Debug.LogWarning("Failed To Select Type");
                return type;
        }
        EnemyPoints -= cost;
        
        return type;
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
        Vector2 spawnLocation;

        if (SpawnLocations.Count > 0)
        {
            spawnLocation = SpawnLocations[index].position;
            SpawnLocations.RemoveAt(index);
            return spawnLocation;
        }
        else 
        {
            Debug.LogWarning("No More Spawn Locations");
            return Vector2.zero;
        }
    }

    private void SpawnEnemy()
    {
        EnemyType enemyToSpawn = SelectEnemyToSpawn();
        if (enemyToSpawn == EnemyType.TypeError) return;

        Vector2 spawnLocation = SelectSpawnLocation();
        if (spawnLocation == Vector2.zero) return;

        ElementType enemyElement = SelectEnemyElement();

        Enemy temp;
        switch (enemyToSpawn)
        {
            case EnemyType.Type1:
                temp = testMeleeEnemyPool.GetPooledObj();
                temp.Init(spawnLocation, enemyElement);
                enemyList.Add(temp);
                break;
            case EnemyType.Type2:
                temp = testRangedEnemyPool.GetPooledObj();
                temp.Init(spawnLocation, enemyElement);
                enemyList.Add(temp);
                break;
        }
        ActiveEnemiesCount++;
        if (EnemyPoints > 0) SpawnEnemy();
    }
    #endregion

    public void DecrementActiveEnemyCounter()
    {
        ActiveEnemiesCount--;
        if (ActiveEnemiesCount <= 0)
        {
            enemyList.Clear();
            if(Level.Instance)
                Level.Instance.ClearLevel();
        }
    }

    public void KillEnemies()
    {
        foreach (Enemy enemy in enemyList)
        {
            enemy.OnDeath();
        }
    }

    public void EnableAggro()
    {
        foreach (Enemy enemy in enemyList)
        {
            enemy.BeginAggro();
        }
    }

    public void DebugAddEnemy(Enemy enemy)
    {
        enemyList.Add(enemy);
        ActiveEnemiesCount++;
    }

    public Transform FindNearestEnemy(Transform origin)
    {
        Transform nearest;
        float distance = float.MaxValue;
        if (enemyList.Count == 1)
            return null;
        else if (enemyList[0].Equals(origin))
        {
            nearest = enemyList[1].transform;
        }
        else
        {
            nearest = enemyList[0].transform;
        }
        foreach(Enemy enemy in enemyList)
        {
            if (enemy.transform != origin)
            {
                float temp = Vector3.Distance(origin.position, enemy.transform.position);
                if (temp < distance)
                {
                    distance = temp;
                    nearest = enemy.transform;
                }
            }
            
        }
        return nearest;
    }
}