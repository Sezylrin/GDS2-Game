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
    [field: SerializeField, ReadOnly] private int NumberOfEnemiesToSpawn { get; set; }
    [field: SerializeField, ReadOnly] private int ActiveEnemiesCount { get; set; }
    [field: SerializeField] private Timer EnemyManagerTimers { get; set; }

    [field: Header("EnemyAttacking")]
    /*[field: SerializeField, ReadOnly] private int AttackPoints { get; set; }
    [field: SerializeField] private int MaxAttackPoints { get; set; } = 10;
    [field: SerializeField] private float AttackPointRefreshRate { get; set; } = 2;
    [field: SerializeField, ReadOnly] private bool CanEnemyAttack { get; set; } = true;
    [field: SerializeField] private float AttackDelay { get; set; } = 0.5f;*/
    [field: SerializeField] public int CurrentAttackers { get; private set; } = 0;
    [SerializeField] private int maxAttackers;
    [field: SerializeField] ElementType debugElementForAttacksList { get; set; }
    [field: SerializeField] bool debugUpdateAttacksList { get; set; }
    [field: SerializeField] bool debugEmptyAttacksList { get; set; }
    [field: SerializeField] private List<ElementType> AttacksList { get; set; }

    [field: Header("Spawning")]
    [field: SerializeField] private GameObject[] enemyPrefabs;
    [field: SerializeField] private List<Transform> SpawnLocations { get; set; }
    [field: SerializeField] private List<Transform> SpawnLocationsCopy { get; set; }
    [field: SerializeField] bool debugTestSpawn { get; set; }
    [field: SerializeField] bool debugSpawnAll { get; set; }
    [field: SerializeField] bool debugShowStats { get; set; }
    [field: SerializeField] bool SimulateFloorsCleared { get; set; }
    [field: SerializeField] bool SimulateHardLevel { get; set; }
    [field: SerializeField] bool debugSetLevelsCleared { get; set; }
    [field: SerializeField] int debugLevelsCleared { get; set; }
    [field: SerializeField, ReadOnly] int FloorsCleared { get; set; }
    [field: SerializeField, ReadOnlyOnPlay] int RhinoSpawnChance { get; set; } = 25;
    [field: SerializeField, ReadOnlyOnPlay] int SnakeSpawnChance { get; set; } = 25;
    [field: SerializeField, ReadOnlyOnPlay] int CheetahSpawnChance { get; set; } = 25;
    [field: SerializeField, ReadOnlyOnPlay] int LizardSpawnChance { get; set; } = 25;
    [field: SerializeField, ReadOnlyOnPlay] int Tier1SpawnChance { get; set; } = 100;
    [field: SerializeField, ReadOnlyOnPlay] int Tier2SpawnChance { get; set; } = 0;
    [field: SerializeField, ReadOnlyOnPlay] int Tier3SpawnChance { get; set; } = 0;

    private Pool<Enemy> rhinoPool;
    private Pool<Enemy> snakePool;
    private Pool<Enemy> cheetahPool;
    private Pool<Enemy> lizardPool;

    [SerializeField]
    private List<Enemy> enemyList = new List<Enemy>();
    [field: SerializeField] bool debugKillEnemies { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        EnemyManagerTimers = GameManager.Instance.TimerManager.GenerateTimers(typeof(EnemyManagerTimer), gameObject);
        //EnemyManagerTimers.times[(int)EnemyManagerTimer.attackDelayTimer].OnTimeIsZero += EnableAttack;
        //EnemyManagerTimers.times[(int)EnemyManagerTimer.attackPointRefeshTimer].OnTimeIsZero += RefreshAttackPoint;

        GameManager.Instance.PoolingManager.FindPool(enemyPrefabs[0], out rhinoPool);
        GameManager.Instance.PoolingManager.FindPool(enemyPrefabs[1], out snakePool);
        GameManager.Instance.PoolingManager.FindPool(enemyPrefabs[2], out cheetahPool);
        GameManager.Instance.PoolingManager.FindPool(enemyPrefabs[3], out lizardPool);

        SetSpawnLocations(SpawnLocations);
    }

    // Update is called once per frame
    void Update()
    {
        if (debugTestSpawn)
        {
            debugTestSpawn = false;
            SpawnEnemy();
        }
        if (debugSpawnAll)
        {
            debugSpawnAll = false;
            StartEnemySpawning(SpawnLocations);
            DisplayEnemyNumberStats();
            DisplayEnemyElementStats();
            DisplayEnemyTierStats();
        }
        if (debugShowStats)
        {
            debugShowStats = false;
            DisplayEnemyNumberStats();
            DisplayEnemyElementStats();
            DisplayEnemyTierStats();
        }
        if (debugSetLevelsCleared)
        {
            debugSetLevelsCleared = false;
            FloorsCleared = debugLevelsCleared;
            CalculateNewTierChances();
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
        if (debugKillEnemies)
        {
            debugKillEnemies = false;
            KillEnemies();
        }
        if (Input.GetKeyDown(KeyCode.Y))
        {
            KillEnemies();
        }
    }

    #region EnemyAttacking
    public bool CanAttack()
    {
        bool canAttack = CurrentAttackers < maxAttackers;
        if (canAttack)
            CurrentAttackers++;
        return canAttack;
    }

    public void DoneAttack()
    {
        CurrentAttackers--;
        if (CurrentAttackers < 0)
        {
            Debug.LogWarning("smth has gone wrong, the current amount of attackers was miscalculated, you can resume and continue or attempt to debug");
            Debug.Break();
            CurrentAttackers = 0;
        }
    }

    /*public void ManagerAttack()
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
    }*/

    private void RecalculateMaxAttackers()
    {
        maxAttackers = Mathf.CeilToInt(ActiveEnemiesCount * 0.5f);
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
    public void StartEnemySpawning(List<Transform> spawnPoints)
    {
        CalculateNewTierChances();
        SetSpawnLocations(spawnPoints);
        NumberOfEnemiesToSpawn = SelectNumberOfEnemiesToSpawnw();
        int temp = NumberOfEnemiesToSpawn;
        while (temp > 0)
        {
            SpawnEnemy();
            temp--;
        }
        if (enemyList.Count() != NumberOfEnemiesToSpawn) Debug.Log("Did Not Spawn the Correct Amount of Enemies. enemyList.Count() = " + enemyList.Count() + ", NumberOfEnemiesToSpawn = " + NumberOfEnemiesToSpawn); 
    }

    private void SetSpawnLocations(List<Transform> spawnLocations)
    {
        SpawnLocations = spawnLocations;
        SpawnLocationsCopy = SpawnLocations;
    }
    private int SelectNumberOfEnemiesToSpawnw()
    {
        int minSpawns = 2;
        int maxSpawns = 3;
        switch (LevelGenerator.Instance.floorsCleared)
        {
            case 0:
                minSpawns = 2;
                maxSpawns = 3;
                break;
            case 1:
                minSpawns = 2;
                maxSpawns = 4;
                break;
            case 2:
                minSpawns = 3;
                maxSpawns = 4;
                break;
            case 3:
                minSpawns = 3;
                maxSpawns = 5;
                break;
            case 4:
                minSpawns = 4;
                maxSpawns = 5;
                break;
            case 5:
                minSpawns = 4;
                maxSpawns = 6;
                break;
            case 6:
                minSpawns = 5;
                maxSpawns = 6;
                break;
        }

        int numberToSpawn;

        if (Level.Instance.isHard) numberToSpawn = maxSpawns;
        else numberToSpawn = Random.Range(minSpawns, maxSpawns + 1);

        return numberToSpawn;
    }

    private EnemyType SelectEnemyType()
    {
        int randomValue = Random.Range(1, 101);

        int rhinoChance = RhinoSpawnChance;
        int snakeChance = rhinoChance + SnakeSpawnChance;
        int cheetahChance = snakeChance + CheetahSpawnChance;
        int lizardChance = cheetahChance + LizardSpawnChance;
        int totalChance = RhinoSpawnChance + SnakeSpawnChance + CheetahSpawnChance + LizardSpawnChance;
        if (totalChance != 100) Debug.Log("Total Type Chance Does Not Equal 100%, totalChance = " + totalChance);

        if (randomValue <= rhinoChance) return EnemyType.Rhino; // Attempt to select an Enemy Type
        else if (rhinoChance < randomValue && randomValue <= snakeChance) return EnemyType.Snake;
        else if (snakeChance < randomValue && randomValue <= cheetahChance) return EnemyType.Cheetah;
        else if (cheetahChance < randomValue && randomValue <= lizardChance) return EnemyType.Lizard;
        else
        {
            Debug.LogWarning("Failed to Select Enemy Type. randomValue = " + randomValue + ", totalChance = " + totalChance);
            return EnemyType.TypeError;
        }
    }

    private ElementType SelectEnemyElement()
    {
        int noElementCount = 0;
        int fireCount = 0;
        int waterCount = 0;
        int shockCount = 0;
        int windCount = 0;

        foreach (ElementType element in AttacksList)
        {
            switch (element) //Count the elements of the previous 25 attacks
            {
                case ElementType.noElement: noElementCount++; break;
                case ElementType.fire: fireCount++; break;
                case ElementType.water: waterCount++; break;
                case ElementType.electric: shockCount++; break;
                case ElementType.wind: windCount++; break;
            }
        }
        int randomValue = Random.Range(1,101);

        int chanceMultiplier = 1;
        bool x = false;
        // x = Level.Instance.isHard;
        if (SimulateHardLevel) x = true;
        if (x) chanceMultiplier = 4;

        int fireChance = fireCount * chanceMultiplier; 
        int waterChance = fireChance + waterCount * chanceMultiplier;
        int shockChance = waterChance + shockCount * chanceMultiplier;
        int windChance = shockChance + windCount * chanceMultiplier;

        int totalChance = windChance;
        int correctChance = 25 * chanceMultiplier;
        if (totalChance != correctChance) Debug.Log("Total Element Chance Does Not Equal " + correctChance + "%, totalChance = " + totalChance);

        if (randomValue <= fireChance) return ElementType.fire; // Attempt to select an element
        else if (fireChance < randomValue && randomValue <= waterChance) return ElementType.water;      
        else if (waterChance < randomValue && randomValue <= shockChance) return ElementType.electric;
        else if (shockChance < randomValue && randomValue <= windChance) return ElementType.wind;
        else return ElementType.noElement; //If failed, return no element
    }

    private void CalculateNewTierChances()
    {
        int x = LevelGenerator.Instance.floorsCleared;
        if (SimulateFloorsCleared) x = FloorsCleared;

        switch (x)
        {
            case 0:
                UpdateTierChances(100, 0, 0);
                break;
            case 1:
                UpdateTierChances(85, 15, 0);
                break;
            case 2:
                UpdateTierChances(65, 30, 5);
                break;
            case 3:
                UpdateTierChances(35, 50, 15);
                break;
            case 4:
                UpdateTierChances(10, 65, 25);
                break;
            case 5:
                UpdateTierChances(0, 50, 50);
                break;
            case 6:
                UpdateTierChances(0, 25, 75);
                break;
        }
    }

    private void UpdateTierChances(int tier1Chance, int tier2Chance, int tier3Chance)
    {
        Tier1SpawnChance = tier1Chance;
        Tier2SpawnChance = tier2Chance;
        Tier3SpawnChance = tier3Chance;
    }

    private int SelectEnemyTier()
    {
        int randomValue = Random.Range(1, 101);

        int tier1Chance = Tier1SpawnChance;
        int tier2Chance = tier1Chance + Tier2SpawnChance;
        int tier3Chance = tier2Chance + Tier3SpawnChance;
        int totalChance = Tier1SpawnChance + Tier2SpawnChance + Tier3SpawnChance;
        if (totalChance != 100) Debug.Log("Total Tier Chance Does Not Equal 100%, totalChance = " + totalChance);

        if (randomValue <= tier1Chance) return 1; // Attempt to select an Enemy Tier
        else if (tier1Chance < randomValue && randomValue <= tier2Chance) return 2;
        else if (tier2Chance < randomValue && randomValue <= tier3Chance) return 3;
        else
        {
            
            Debug.LogWarning("Failed to Select Enemy Tier. randomValue = " + randomValue + ", totalChance = " + totalChance);
            return 0;
        }
    }

    private Vector2 SelectSpawnLocation()
    {
        int index = Random.Range(0, SpawnLocationsCopy.Count());
        Vector2 spawnLocation;

        if (SpawnLocationsCopy.Count > 0)
        {
            spawnLocation = SpawnLocationsCopy[index].position;
            SpawnLocationsCopy.RemoveAt(index);
            return spawnLocation;
        }
        else 
        {
            Debug.LogWarning("Ran Out of Spawn Locations. NumberOfEnemiesToSpawn = " + NumberOfEnemiesToSpawn + ", SpawnLocations.Count() = " + SpawnLocations.Count());
            return Vector2.zero;
        }
    }

    private void SpawnEnemy()
    {
        EnemyType type = SelectEnemyType();
        if (type == EnemyType.TypeError) return;

        Vector2 spawnLocation = SelectSpawnLocation();
        if (spawnLocation == Vector2.zero) return;

        int tier = SelectEnemyTier();
        if (tier == 0) return;

        ElementType enemyElement = SelectEnemyElement();

        Enemy temp;
        switch (type)
        {
            case EnemyType.Rhino:
                temp = rhinoPool.GetPooledObj();
                temp.Init(spawnLocation, enemyElement, tier);
                enemyList.Add(temp);
                break;
            case EnemyType.Snake:
                temp = snakePool.GetPooledObj();
                temp.Init(spawnLocation, enemyElement, tier);
                enemyList.Add(temp);
                break;
            case EnemyType.Cheetah:
                temp = cheetahPool.GetPooledObj();
                temp.Init(spawnLocation, enemyElement, tier);
                enemyList.Add(temp);
                break;
            case EnemyType.Lizard:
                temp = lizardPool.GetPooledObj();
                temp.Init(spawnLocation, enemyElement, tier);
                enemyList.Add(temp);
                break;
            default:
                temp = rhinoPool.GetPooledObj();
                temp.Init(spawnLocation, enemyElement, tier);
                enemyList.Add(temp);
                break;
        }
        temp.ChangeState();
        ActiveEnemiesCount++;
        RecalculateMaxAttackers();
    }
    #endregion

    public void DecrementActiveEnemyCounter()
    {
        ActiveEnemiesCount--;
        if (ActiveEnemiesCount <= 0 && Level.Instance)
        {
            Level.Instance.ClearLevel();
        }
    }
    [ContextMenu("KillAllEnemies")]
    public void KillEnemies()
    {
        foreach (Enemy enemy in enemyList)
        {
            enemy.OnDeath(true);
        }
        enemyList.Clear();
        ActiveEnemiesCount = 0;
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

    public void DisplayEnemyNumberStats()
    {
        int rhinoCount = 0;
        int snakeCount = 0;
        int cheetahCount = 0;
        int lizardCount = 0;

        foreach (Enemy enemy in enemyList)
        {
            switch (enemy.Type) //Count the elements of the previous 25 attacks
            {
                case EnemyType.Rhino: rhinoCount++; break;
                case EnemyType.Snake: snakeCount++; break;
                case EnemyType.Cheetah: cheetahCount++; break;
                case EnemyType.Lizard: lizardCount++; break;
            }
        }
        Debug.Log("Number of Enemies = " + enemyList.Count() + ". rhinoCount = " + rhinoCount + ", snakeCount = " + snakeCount + ", cheeetahCount = " + cheetahCount + ", lizardCount = " + lizardCount);
    }

    public void DisplayEnemyElementStats()
    {
        int noElementCount = 0;
        int fireCount = 0;
        int waterCount = 0;
        int shockCount = 0;
        int windCount = 0;

        foreach (Enemy enemy in enemyList)
        {
            switch (enemy.Element) //Count the elements of the previous 25 attacks
            {
                case ElementType.noElement: noElementCount++; break;
                case ElementType.fire: fireCount++; break;
                case ElementType.water: waterCount++; break;
                case ElementType.electric: shockCount++; break;
                case ElementType.wind: windCount++; break;
            }
        }
        Debug.Log("Number of Enemies = " + enemyList.Count() + ". noElementCount = " + noElementCount + ", fireCount = " + fireCount + ", waterCount = " + waterCount + ", shockCount = " + shockCount + ", windCount = " + windCount);
    }

    public void DisplayEnemyTierStats()
    {
        int tier1Count = 0;
        int tier2Count = 0;
        int tier3Count = 0;

        foreach (Enemy enemy in enemyList)
        {
            switch (enemy.Tier) //Count the elements of the previous 25 attacks
            {
                case 1: tier1Count++; break;
                case 2: tier2Count++; break;
                case 3: tier3Count++; break;
            }
        }
        Debug.Log("Number of Enemies = " + enemyList.Count() + ". tier1Count = " + tier1Count + ", tier2Count = " + tier2Count + ", tier3Count = " + tier3Count);
    }
}