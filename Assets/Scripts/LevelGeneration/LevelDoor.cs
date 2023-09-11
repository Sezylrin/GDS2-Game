using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelDoor : MonoBehaviour
{
    [Serializable]
    public enum DoorPosition
    {
        Left,
        Right,
        Centre
    }
    public DoorPosition doorPosition;

    private void OnEnable()
    {
        Level.OnLevelClear += OpenDoor;
    }

    private void OnDisable()
    {
        Level.OnLevelClear -= OpenDoor;
    }

    private void Start()
    {
        RemoveUnneededDoors();
    }

    private void OnTriggerEnter2D(Collider2D otherCollider)
    {
        if (!otherCollider.CompareTag("Player")) return;
        if (!Level.Instance.isCleared) return;
        ExitLevel();
    }

    private void OpenDoor()
    {
        //TODO: Animate
    }

    private void ExitLevel()
    {
        switch (doorPosition)
        {
            case DoorPosition.Left:
                LevelGenerator.Instance.EnterDoorLeft();
                break;
            case DoorPosition.Right:
                LevelGenerator.Instance.EnterDoorRight();
                break;
            case DoorPosition.Centre:
                LevelGenerator.Instance.EnterDoorCentre();
                break;
        }
    }

    private void RemoveUnneededDoors()
    {
        // if level before fountain
        if (LevelGenerator.Instance.activeLevelListIndex % (LevelGenerator.Instance.fountainFrequency + 1) >= LevelGenerator.Instance.fountainFrequency - 1)
        {
            if (doorPosition is DoorPosition.Left or DoorPosition.Right)
            {
                Destroy(gameObject);
            }
        }
        else
        {
            if (doorPosition is DoorPosition.Centre)
            {
                Destroy(gameObject);
            }
        }
    }
}