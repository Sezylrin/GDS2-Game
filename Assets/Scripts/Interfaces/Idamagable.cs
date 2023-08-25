using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    public float Hitpoints { get; set; }

    public void SetHitPoints();
    public void TakeDamage(float amount);
    public void OnDeath();

}
