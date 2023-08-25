using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Idamageable
{
    public float hitpoints { get; set; }

    public void setHitPoints();
    public void TakeDamage(float amount);
    public void OnDeath();

}
