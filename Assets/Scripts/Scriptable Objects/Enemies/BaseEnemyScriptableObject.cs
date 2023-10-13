using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Enemy SO", menuName = "ScriptableObjects/Enemies")]
public class BaseEnemyScriptableObject : ScriptableObject
{
    public int[] maxHealth;
    public float[] speed;
    public int[] minSouls;
    public int[] maxSouls;
    public float effectDuration;
    [Space(20)]
    public float attackKnockback;
    public float[] attackCooldown;
    [Space(20)]
    public int[] attack1Damage;
    public float attack1Duration;
    public float windup1Duration;
    [Space(20)]
    public int[] attack2Damage;
    public float attack2Duration;
    public float windup2Duration;
    [Space(20)]
    public int[] attack3Damage;
    public float attack3Duration;
    public float windup3Duration;
    [Space(20)]
    public int basePointsToStagger = 300;
    public float staggerMinDuration = 1.5f;
    public float staggerMaxDuration = 3;
    public float staggerDelayDuration = 3;
    public float staggerDecayAmount = 4;
    public float staggerDecayRate = 0.25f;
    public int damageToReachMaxDuration;
}
