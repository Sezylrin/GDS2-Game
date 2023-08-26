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





