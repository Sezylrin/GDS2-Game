using System;
using System.Collections;
using System.Collections.Generic;
using KevinCastejon.MoreAttributes;
using Tymski;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class LevelGenerator : MonoBehaviour
{
    public List<SceneReference> roomsListReference;
    public int roomsInPool;
    [ReadOnly] public List<SceneReference> roomPool;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        int[] ignoredRooms = new int[2];
        ignoredRooms[0] = 1;
        ignoredRooms[1] = 2;
        roomPool = CreateRoomPool(ignoredRooms);
        DebugRoomPool();
    }

    // Creates a list of Scenes randomly chosen from roomsListReference of size equal to roomsInPool.
    private List<SceneReference> CreateRoomPool()
    {
        List<SceneReference> roomsListClone = new List<SceneReference>(roomsListReference);

        // If there are less rooms than specified in the pool, take the remaining rooms
        if (roomsListClone.Count < roomsInPool)
        {
            Debug.Log("roomsInPool is larger than rooms available. Pool size is now: " + roomsListClone.Count);
            return roomsListClone;
        }

        List<SceneReference> newRoomPool = new List<SceneReference>();
        for (int roomIndex = 0; roomIndex < roomsInPool; roomIndex++)
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
        if (roomsListClone.Count < roomsInPool)
        {
            Debug.Log("roomsInPool is larger than rooms available. Pool size is now: " + roomsListClone.Count);
            return roomsListClone;
        }

        List<SceneReference> newRoomPool = new List<SceneReference>();
        for (int roomIndex = 0; roomIndex < roomsInPool; roomIndex++)
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
            if (roomsListClone.Count < roomsInPool)
            {
                Debug.Log("roomsInPool is larger than rooms available. Pool size is now: " + roomsListClone.Count);
                return roomsListClone;
            }

            List<SceneReference> newRoomPool = new List<SceneReference>();
            for (int roomIndex = 0; roomIndex < roomsInPool; roomIndex++)
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

    private void DebugRoomPool()
    {
        Debug.Log("There are " + roomPool.Count + " rooms in the pool");
        foreach (SceneReference room in roomPool)
        {
            Debug.Log(room.ScenePath);
        }
    }
}