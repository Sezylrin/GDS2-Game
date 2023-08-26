using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AbilityType
{
    blast,
    AOE,
    Projectile
}

[CreateAssetMenu(fileName = "Abilities", menuName = "ScriptableObjects/ElementalAbility")]
public class ElementalSO : ScriptableObject
{
    public ElementType elementType;

    public AbilityType type;

    public int castCost;

    public int pierceAmount;

    public float damage;
}
