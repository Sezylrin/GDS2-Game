using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartDungeonInteraction : InteractionBase
{
    public int floorToSpawn = 1;
    public GameObject openSprite;
    private bool isOpen;

    public void Start()
    {
        isOpen = GameManager.Instance.LevelGenerator.highestFloor >= floorToSpawn-1;
        openSprite.SetActive(isOpen);
    }

    public override void Interact()
    {
        if (!isOpen) return;
        GameManager.Instance.LevelGenerator.StartDungeonOnFloor(floorToSpawn);
        Destroy(this);
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);
        UI.SetActive(isOpen);
    }
}