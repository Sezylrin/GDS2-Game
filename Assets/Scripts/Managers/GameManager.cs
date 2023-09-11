using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public SceneLoader sceneLoader { get; private set; }
    [field: SerializeField] public EnemyManager EnemyManager { get; private set; }

    public int Souls { get; set; }

    private void Awake()
    {
        if (Instance != null)
        {
            DestroyImmediate(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void AddSouls(int souls)
    {
        Souls += souls;
    }
}