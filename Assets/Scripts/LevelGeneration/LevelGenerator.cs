using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using KevinCastejon.MoreAttributes;
using Tymski;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

//TODO: Split Paths, Point Bucket, Async Loading, Locked Rooms
public class LevelGenerator : MonoBehaviour
{
    [Serializable]
    public enum Randomness
    {
        TrueRandom,
        NoDoubles,
        Looped
    }
    [Header("Parameters")]
    public Randomness randomness;
    public List<SceneReference> roomsListReference;
    public SceneReference FountainRoom;
    public int roomPoolSize;
    [Tooltip("Rooms between each fountain spawn, should be >= roomPoolSize")]
    public int fountainFrequency;
    [Header("ReadOnly")]
    [ReadOnly] public int activeLevelListIndex;
    private List<SceneReference> roomPool;
    private List<SceneReference> levelList;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        Generate();
        DebugLevelList();
        SceneManager.LoadScene(levelList[activeLevelListIndex]);
    }

    private void Generate()
    {
        roomPool = CreateRoomPool();
        levelList = CreateLevelList();
        activeLevelListIndex = 0;
    }

    private void DebugStart()
    {
        int[] ignoredRooms = new int[2];
        ignoredRooms[0] = 1;
        ignoredRooms[1] = 2;
        roomPool = CreateRoomPool(ignoredRooms);
        DebugRoomPool();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            LoadNextRoomInLevel();
        }
    }

    //TODO: Change to AsyncLoading
    private void LoadNextRoomInLevel()
    {
        // If the next level is not in list
        if (activeLevelListIndex + 1 >= levelList.Count)
        {
            return;
            //TODO: Create a new level and add/replace list
        }
        activeLevelListIndex++;
        SceneManager.LoadScene(levelList[activeLevelListIndex]);
    }

    // Creates a list of Scenes randomly chosen from roomsListReference of size equal to roomsInPool.
    private List<SceneReference> CreateRoomPool()
    {
        List<SceneReference> roomsListClone = new List<SceneReference>(roomsListReference);

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
        catch (IndexOutOfRangeException OOB)
        {
            Debug.LogError("Ignored rooms must be defined in descending order");
            throw OOB;
        }
    }

    private List<SceneReference> CreateLevelList()
    {
        if (!roomPool.Any())
        {
            Debug.LogWarning("roomPool is empty");
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
                levelList.Add(FountainRoom);
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
                levelList.Add(FountainRoom);
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
                levelList.Add(FountainRoom);
                break;
        }
        return levelList;
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
}