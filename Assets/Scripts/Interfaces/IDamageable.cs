using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    public float Hitpoints { get; set; }
    public Rigidbody2D rb { get; }
    public void SetHitPoints();
    public void TakeDamage(float amount, int staggerPoints, ElementType type, int tier, ElementType typeTwo = ElementType.noElement);
    public void TakeDamage(float amount, int staggerPoints, ElementType type, ElementType typeTwo = ElementType.noElement);
    public void OnDeath();
    public void AddForce(Vector2 force);
}
