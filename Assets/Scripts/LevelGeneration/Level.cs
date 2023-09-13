using System;
using System.Collections;
using System.Collections.Generic;
using KevinCastejon.MoreAttributes;
using UnityEngine;

public class Level : MonoBehaviour
{
    //TODO: Spawn Points
    private static Level _instance;
    public static Level Instance { get { return _instance; } }

    [SerializeField]
    private int baseEnemyPoints = 5;
    public int totalEnemyPoints { get; private set; }
    [SerializeField]
    private float enemyPointMultiplier = 1.0f;
    [SerializeField]
    private Transform playerSpawnPoint;
    [SerializeField]
    private List<Transform> enemySpawnPoints;
    [SerializeField]
    private bool isFountain = false;
    [Header("Debug")]
    public bool debugClearLevel;
    [field: SerializeField, ReadOnly]
    public bool isCleared { get; private set; }
    public static event Action OnLevelClear;
    private void Awake()
    {
        // Singleton pattern
        if (_instance != null && _instance != this)
        {
            DestroyImmediate(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    private void Start()
    {
        ValidateTotalEnemyPoints();
        SpawnPlayer();
        GetEnemySpawnPoints();
        LevelGenerator.Instance.TriggerCrossFadeEnd();
        if (!isFountain)
            GameManager.Instance.EnemyManager.StartEnemySpawning(enemySpawnPoints, totalEnemyPoints);
        else
            ClearLevel();
    }

    private void GetEnemySpawnPoints()
    {
        //TODO: Get List of enemy spawn points
    }

    public void ClearLevel()
    {
        isCleared = true;
        OnLevelClear?.Invoke();
    }

    private void OnValidate()
    {
        if (!debugClearLevel) return;
        debugClearLevel = false;
        ClearLevel();
    }

    private void ValidateTotalEnemyPoints()
    {
        totalEnemyPoints = Mathf.RoundToInt((baseEnemyPoints + LevelGenerator.Instance.difficulty) * enemyPointMultiplier);
    }

    public void OverrideTotalEnemyPoints(int newTotalEnemyPoints)
    {
        totalEnemyPoints = newTotalEnemyPoints;
    }

    private void SpawnPlayer()
    {
        if (Player.Instance.transform.position !=
            playerSpawnPoint.position)
        {
            Player.Instance.transform.position = playerSpawnPoint.position;
        }
    }
}