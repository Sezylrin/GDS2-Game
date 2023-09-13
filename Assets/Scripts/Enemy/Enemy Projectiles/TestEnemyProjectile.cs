using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEnemyProjectile : EnemyProjectile
{
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);
    }
}
