using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Consume : InteractionBase
{
    [field: SerializeField] private Enemy enemy;

    public override void Interact()
    {
        GameManager.Instance.PCM.system.FullHeal(); // Full heals -- TEMP
        enemy.OnDeath();
    }
}
