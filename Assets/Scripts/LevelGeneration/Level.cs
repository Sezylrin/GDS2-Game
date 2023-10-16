using System;
using System.Collections;
using System.Collections.Generic;
using KevinCastejon.MoreAttributes;
using UnityEngine;
using Random = UnityEngine.Random;

public class Level : MonoBehaviour
{
    //TODO: Spawn Points
    private static Level _instance;
    public static Level Instance { get { return _instance; } }

    [SerializeField]
    private int baseEnemyPoints = 5;
    public int totalEnemyPoints { get; private set; }
    [SerializeField]
    private Transform playerSpawnPoint;
    public float swarmSpawnChance;
    public float miniBossSpawnChance;
    [SerializeField]
    private GameObject enemySpawnPointsContainer;
    [SerializeField]
    private GameObject swarmEnemySpawnPointsContainer;
    [SerializeField]
    private GameObject miniBossEnemySpawnPointsContainer;
    [SerializeField]
    private List<Transform> enemySpawnPoints;
    [SerializeField]
    private bool isFountain;
    public bool isHard { get; private set; }
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
        if (!GameObject.FindWithTag(Tags.T_RequiredCamera))
        {
            Debug.LogWarning("The Required Camera Prefab object is not in the scene, please put one in. The prefab is located in the camera folder inside prefabs folder");
            Debug.Break();
        }
    }

    private void Start()
    {
        ValidateHardLevel();
        ValidateTotalEnemyPoints();
        SpawnPlayer();
        GetEnemySpawnPoints();
        LevelGenerator.Instance.TriggerCrossFadeEnd();
        if (!isFountain)
            GameManager.Instance.EnemyManager.StartEnemySpawning(enemySpawnPoints);
        else
            ClearLevel();
    }

    private void ValidateHardLevel()
    {
        if (!LevelGenerator.Instance.hardNextLevel) return;
        LevelGenerator.Instance.hardNextLevel = false;
        isHard = true;
    }

    private void GetEnemySpawnPoints()
    {
        float rnd = Random.Range(0f, 100f);
        GameObject parentObject = null;
        if (rnd < swarmSpawnChance)
        {
            parentObject = swarmEnemySpawnPointsContainer;

        }
        else if (swarmSpawnChance < rnd && rnd < swarmSpawnChance + miniBossSpawnChance)
        {
            parentObject = miniBossEnemySpawnPointsContainer;
        }
        else
        {
            parentObject = enemySpawnPointsContainer;
        }
        Transform[] enemySpawnPointsArray = parentObject.GetComponentsInChildren<Transform>();
        foreach (var ESP in enemySpawnPointsArray)
        {
            if (ESP.gameObject != parentObject)
            {
                enemySpawnPoints.Add(ESP);
            }
        }
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
        totalEnemyPoints = Mathf.RoundToInt(baseEnemyPoints + LevelGenerator.Instance.difficulty);
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