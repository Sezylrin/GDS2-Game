using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartDungeonInteraction : InteractionBase
{
    // Start is called before the first frame update
    public override void Interact()
    {
        GameManager.Instance.LevelGenerator.EnterDoorCentre();
        Destroy(this);
    }
}