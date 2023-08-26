using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AbilityType
{
    blast,
    AOE,
    Projectile
}


public class ElementalSO : ScriptableObject
{
    public ElementType elementType;

    public AbilityType type;

    public int castCost;

    public int pierceAmount;

    public float damage;

    public float lifeTime;
}
[CreateAssetMenu(fileName = "Projectile Ability", menuName = "ScriptableObjects/ProjectileAbility")]
public class ProjectileElementSO : ElementalSO
{
    public float speed;
}

[CreateAssetMenu(fileName = "AOE Ability", menuName = "ScriptableObjects/AoeAbility")]
public class AoeElementSO : ElementalSO
{
    public float radius;
}

[CreateAssetMenu(fileName = "Burst Ability", menuName = "ScriptableObjects/BurstAbility")]
public class BurstElementSO : ElementalSO
{
    public float distance;
}
