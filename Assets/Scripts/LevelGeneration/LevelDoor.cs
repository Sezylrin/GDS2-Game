using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelDoor : InteractionBase
{
    private bool doorAlreadyUsed;

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

    public override void Interact()
    {
        if (Level.Instance.isCleared && !doorAlreadyUsed)
        {
            doorAlreadyUsed = true;
            ExitLevel();
        }
    }
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (!Level.Instance.isCleared)
            return;
        if (collision.CompareTag(Tags.T_Player))
        {
            GameManager.Instance.SetInteraction(this);
            if (UI)
                UI.SetActive(true);
        }
    }

    protected override void OnTriggerExit2D(Collider2D collision)
    {
        if (!Level.Instance.isCleared)
            return;
        if (collision.CompareTag(Tags.T_Player))
        {
            GameManager.Instance.RemoveInteraction(this);
            if (UI)
                UI.SetActive(false);
        }
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