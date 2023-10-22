using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using KevinCastejon.MoreAttributes;
using Tymski;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class LevelGenerator : MonoBehaviour
{
    private static LevelGenerator _instance;
    public static LevelGenerator Instance { get { return _instance; }}


    [Serializable]
    public enum Randomness
    {
        TrueRandom,
        NoDoubles,
        Looped
    }
    [Header("Parameters")]
    public Randomness randomness;
    public SceneReference fountainRoom;
    public List<SceneReference> roomsListReference;
    public int roomPoolSize = 3;
    [Tooltip("Levels between each fountain spawn, should be >= roomPoolSize")]
    public int fountainFrequency = 5;
    // [Tooltip("The amount that the difficulty will increase when moving through the left/easier room")]
    // public int difficultyIncreaseLeft = 1;
    // [Tooltip("The amount that the difficulty will increase when moving through the right/harder room")]
    // public int difficultyIncreaseRight = 2;
    [field: SerializeField]
    public int floorsToWin { get; private set; }
    // [SerializeField]
    // private int startDifficulty = 5;
    public bool hardNextLevel { get; private set; }

    // [field: SerializeField, ReadOnly]
    // public int difficulty { get; private set; }
    private List<SceneReference> roomPool = new List<SceneReference>();
    private List<SceneReference> levelList = new List<SceneReference>();
    [field: SerializeField, ReadOnly, Header("ReadOnly")]
    public int activeLevelListIndex { get; private set; }
    // Calculates floorsCleared, replaced with floorsClearedOverride if debugMode is enabled
    public int floorsCleared => !debugMode ? Mathf.FloorToInt((float)(activeLevelListIndex + 1) / (fountainFrequency + 1)) : floorsClearedOverride;
    [ReadOnly]
    public int lastFloorOnExit;

    [Header("Debug")]
    public bool debugMode;
    public bool debugNextLevel;
    public int floorsClearedOverride;
    
    private void Awake()
    {
        // Singleton pattern
        if (_instance && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    private void Start()
    {
        New();
        // DebugLevelList();
        // SceneManager.LoadScene(levelList[activeLevelListIndex]);
    }

    public void New()
    {
        lastFloorOnExit = floorsCleared;
        // difficulty = startDifficulty;
        levelList = new List<SceneReference>();
        AddNewFloor();
        activeLevelListIndex = -1;
    }

    private void Update()
    {
        // if (Input.GetKeyDown(KeyCode.Space))
        // {
        //     LoadNextLevel();
        // }
        // if (Input.GetKeyDown(KeyCode.Alpha1))
        // {
        //     DebugLoadLevelX(1);
        // }
        // if (Input.GetKeyDown(KeyCode.Alpha2))
        // {
        //     DebugLoadLevelX(2);
        // }
        // if (Input.GetKeyDown(KeyCode.Alpha3))
        // {
        //     DebugLoadLevelX(3);
        // }
        // if (Input.GetKeyDown(KeyCode.D))
        // {
        //     Debug.Log(floorsCleared);
        // }
    }

    private void OnValidate()
    {
        if (!debugMode) return;
        if (debugNextLevel)
        {
            debugNextLevel = false;
            GameManager.Instance.EnemyManager.KillEnemies();
            LoadNextLevel();
        }
    }

    //TODO: Change to AsyncLoading
    private void LoadNextLevel()
    {
        if (DungeonCleared())
        {
            //TODO: Win
            return;
        }
        // If the next level is not in list
        while (activeLevelListIndex + 1 >= levelList.Count)
        {
            AddNewFloor();
        }
        // ALWAYS update the tracker before loading the scene
        activeLevelListIndex++;
        StartCoroutine(LoadLevel(levelList[activeLevelListIndex]));
    }

    private bool DungeonCleared() => Mathf.FloorToInt((float)(activeLevelListIndex + 2)/(fountainFrequency + 1)) >= floorsToWin;

    IEnumerator LoadLevel(string scenePath)
    {
        GameManager.Instance.sceneLoader.CrossFadeAnimator.SetTrigger("Start");

        yield return new WaitForSeconds(GameManager.Instance.sceneLoader.CrossFadeTime);

        SceneManager.LoadScene(scenePath);
    }

    public void TriggerCrossFadeEnd()
    {
        GameManager.Instance.sceneLoader.CrossFadeAnimator.SetTrigger("End");
    }

    private void DebugLoadLevelX(int x)
    {
        SceneManager.LoadScene(roomsListReference[x-1]);
    }

    public void EnterDoorLeft()
    {
        // difficulty += difficultyIncreaseLeft;
        LoadNextLevel();
    }

    public void EnterDoorRight()
    {
        // difficulty += difficultyIncreaseRight;
        hardNextLevel = true;
        LoadNextLevel();
    }

    public void EnterDoorCentre()
    {
        LoadNextLevel();
    }

    private void AddNewFloor()
    {
        roomPool = CreateRoomPool();
        if (!levelList.Any())
        {
            levelList = CreateLevelList();
        }
        else
        {
            levelList?.AddRange(CreateLevelList());
        }
    }

    // Creates a list of Scenes randomly chosen from roomsListReference of size equal to roomsInPool.
    private List<SceneReference> CreateRoomPool()
    {
        List<SceneReference> roomsListClone = new List<SceneReference>(roomsListReference);

        // If there are less rooms than specified in the pool, take the remaining rooms
        if (roomsListClone.Count < roomPoolSize)
        {
            Debug.LogWarning("roomsInPool is larger than rooms available. Pool size is now: " + roomsListClone.Count);
            return roomsListClone;
        }

        List<SceneReference> newRoomPool = new List<SceneReference>();
        for (int roomIndex = 0; roomIndex < roomPoolSize; roomIndex++)
        {
            int addedRoomIndex = Random.Range(0, roomsListClone.Count);
            newRoomPool.Add(roomsListClone[addedRoomIndex]);
            roomsListClone.RemoveAt(addedRoomIndex);
        }
        return newRoomPool;
    }

    // Creates a list of Scenes randomly chosen from roomsListReference of size equal to roomsInPool, ignoring Scenes passed in parameter.
    // Overload taking a List of Scenes to ignore.
    private List<SceneReference> CreateRoomPool(List<SceneReference> ignoredRooms)
    {
        List<SceneReference> roomsListClone = new List<SceneReference>(roomsListReference);
        roomsListClone.RemoveAll(ignoredRooms.Contains);

        // If there are less rooms than specified in the pool, take the remaining rooms
        if (roomsListClone.Count < roomPoolSize)
        {
            Debug.Log("roomsInPool is larger than rooms available. Pool size is now: " + roomsListClone.Count);
            return roomsListClone;
        }

        List<SceneReference> newRoomPool = new List<SceneReference>();
        for (int roomIndex = 0; roomIndex < roomPoolSize; roomIndex++)
        {
            int addedRoomIndex = Random.Range(0, roomsListClone.Count);
            newRoomPool.Add(roomsListClone[addedRoomIndex]);
            roomsListClone.RemoveAt(addedRoomIndex);
        }
        return newRoomPool;
    }

    private List<SceneReference> CreateRoomPool(int[] ignoredRooms)
    {
        try
        {
            for (int i = 1; i < ignoredRooms.Length; i++)
            {
                if (ignoredRooms[i] > ignoredRooms[i - 1])
                    Debug.LogError("Ignored rooms must be defined in descending order");
            }

            List<SceneReference> roomsListClone = new List<SceneReference>(roomsListReference);

            foreach (int index in ignoredRooms)
            {
                roomsListClone.RemoveAt(index);
            }

            // If there are less rooms than specified in the pool, take the remaining rooms
            if (roomsListClone.Count < roomPoolSize)
            {
                Debug.Log("roomsInPool is larger than rooms available. Pool size is now: " + roomsListClone.Count);
                return roomsListClone;
            }

            List<SceneReference> newRoomPool = new List<SceneReference>();
            for (int roomIndex = 0; roomIndex < roomPoolSize; roomIndex++)
            {
                int addedRoomIndex = Random.Range(0, roomsListClone.Count);
                newRoomPool.Add(roomsListClone[addedRoomIndex]);
                roomsListClone.RemoveAt(addedRoomIndex);
            }
            return newRoomPool;
        }
        catch (IndexOutOfRangeException outOfBounds)
        {
            Debug.LogError("Ignored rooms must be defined in descending order");
            throw outOfBounds;
        }
    }

    private List<SceneReference> CreateLevelList()
    {
        if (!roomPool.Any())
        {
            Debug.LogError("roomPool is empty, cannot add floor");
            return null;
        }
        List<SceneReference> levelList = new List<SceneReference>();
        int roomsCreated = 0;
        switch (randomness)
        {
            case Randomness.TrueRandom:
                while (roomsCreated < fountainFrequency)
                {
                    levelList.Add(roomPool[Random.Range(0,roomPool.Count)]);
                    roomsCreated++;
                }
                levelList.Add(fountainRoom);
                break;
            case Randomness.NoDoubles:
                SceneReference lastRoomAdded = new SceneReference();
                while (roomsCreated < fountainFrequency)
                {
                    if (lastRoomAdded.ScenePath != "") roomPool.Remove(lastRoomAdded);
                    SceneReference roomToAdd = roomPool[Random.Range(0, roomPool.Count)];
                    levelList.Add(roomToAdd);
                    roomsCreated++;
                    if (lastRoomAdded.ScenePath != "" && !roomPool.Contains(lastRoomAdded)) roomPool.Add(lastRoomAdded);
                    lastRoomAdded = roomToAdd;
                }
                levelList.Add(fountainRoom);
                break;
            case Randomness.Looped:
                int index = 0;
                while (roomsCreated < fountainFrequency)
                {
                    if (index >= roomPool.Count) index = 0;
                    levelList.Add(roomPool[index]);
                    roomsCreated++;
                    index++;
                }
                levelList.Add(fountainRoom);
                break;
        }
        return levelList;
    }

    public void ResetHardNextLevel()
    {
        hardNextLevel = false;
    }

    private void DebugRoomPool()
    {
        Debug.Log("There are " + roomPool.Count + " rooms in the pool");
        foreach (SceneReference room in roomPool)
        {
            Debug.Log(room.ScenePath);
        }
    }

    private void DebugLevelList()
    {
        Debug.Log("There are " + levelList.Count + " rooms in the pool");
        foreach (SceneReference room in levelList)
        {
            Debug.Log(room.ScenePath);
        }
    }

    private void DebugStart()
    {
        int[] ignoredRooms = new int[2];
        ignoredRooms[0] = 1;
        ignoredRooms[1] = 2;
        roomPool = CreateRoomPool(ignoredRooms);
        DebugRoomPool();
    }
}