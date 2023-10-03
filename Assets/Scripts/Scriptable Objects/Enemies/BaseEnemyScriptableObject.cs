using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Enemy SO", menuName = "ScriptableObjects/Enemies")]
public class BaseEnemyScriptableObject : ScriptableObject
{
    public int maxHealth;
    public float speed;
    public int souls;
    public float effectDuration = 5;
    [Space(20)]
    public float attackKnockback = 5;
    public float attackCooldown = 10;
    [Space(20)]
    public int attack1Damage;
    public float attack1Duration;
    public float windup1Duration;
    [Space(20)]
    public int attack2Damage;
    public float attack2Duration;
    public float windup2Duration;
    [Space(20)]
    public int attack3Damage;
    public float attack3Duration;
    public float windup3Duration;
    [Space(20)]
    [Range(0, 100)] public int consumableHealthPercentThreshold = 25;
    [Range(0, 100)] public int percentToHealOnConsume = 10;
    [Space(20)]
    public int pointsToStagger = 100;
    public float staggerDuration = 3;
    public float staggerDelayDuration = 3;
    public float staggerDecayAmount = 4;
    public float staggerDecayRate = 0.25f;
}
