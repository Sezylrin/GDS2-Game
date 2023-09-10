using System;
using System.Collections;
using System.Collections.Generic;
using KevinCastejon.MoreAttributes;
using UnityEngine;

public class Level : MonoBehaviour
{
    private static Level _instance;
    public static Level Instance { get { return _instance; } }

    [SerializeField]
    private int baseEnemyPoints = 5;
    public int totalEnemyPoints { get; private set; }
    public int enemiesRemaining { get; private set; }
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
        enemiesRemaining = 0;
        ValidateTotalEnemyPoints();
    }

    private void Update()
    {
        if (isCleared) return;
        if (CalculateIsCleared()) ClearLevel();
    }

    private bool CalculateIsCleared()
    {
        return totalEnemyPoints <= 0 && enemiesRemaining <= 0;
    }

    private void ClearLevel()
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

    public void UseEnemyPoints(int pointsToUse)
    {
        totalEnemyPoints -= pointsToUse;
    }

    public void AddEnemy()
    {
        enemiesRemaining++;
    }

    private void ValidateTotalEnemyPoints()
    {
        totalEnemyPoints = baseEnemyPoints + LevelGenerator.Instance.difficulty;
    }

    public void OverrideTotalEnemyPoints(int newTotalEnemyPoints)
    {
        totalEnemyPoints = newTotalEnemyPoints;
    }
}